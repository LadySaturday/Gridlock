using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICarBehaviour : MonoBehaviour
{
    /// <summary>
    /// logic:
    /// Cars detect cars ahead of them and enter states accordningly:
    /// Stop: stop movement unitl it's safe
    /// GO: proceed to next waypoint
    /// </summary>
    /// 
    private Transform[] waypoints;


    private GameObject[] paths;
    private NavMeshAgent agent;
    public  GameObject currentPath;//the path the car is currently on (the current waypoint system)
    private int currentWaypoint=0;

    public Transform raycastAnchor;//front of car
    
    public float raycastLength = 5;    
    public int raySpacing = 2;    
    public int raysNumber = 6;    
    public float emergencyBrakeDist = 2f;   
    public float slowDownDist = 4f;

    public float maxSpeed =10;
    public StateMachine stateMachine;
    private bool canRaycast=true;

    private AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();

        paths = GameObject.FindGameObjectsWithTag("path");
        agent = GetComponent<NavMeshAgent>();

        //create states
        stateMachine = new StateMachine();
        stateMachine.AddState(new StateMachine.State()
        {
            Name = "Go",
            OnEnter = () =>
            {
                UpdateCurrentPath();
                agent.isStopped = false;
                agent.speed = maxSpeed;
            },
            OnStay = () =>
            {
            },
            OnExit = () => { }

        });
        stateMachine.AddState(new StateMachine.State()
        {
            Name = "Stop",
            OnEnter = () =>
            {
                agent.isStopped=true;
               GetComponent<Rigidbody>().velocity = Vector3.zero;

            },
            OnStay = () =>
            {
            },
            OnExit = () => { }

        });


        stateMachine.TransitionTo("Go");   

    }
    //play audio on crash
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="Player")
            audio.Play();
    }
    //change path/ followed waypoints to other available path
    public void UpdateCurrentPath()
    {
       // Debug.Log("New path: " + currentPath.name);
        Array.Resize(ref waypoints, currentPath.transform.childCount);
        for (int i = 0; i < currentPath.transform.childCount; i++)
        {
            waypoints[i] = currentPath.transform.GetChild(i);
        }

        agent.SetDestination(ClosestWaypointOnPath());
        
        
    }
   

    // Update is called once per frame
    void Update()
    {
        if (!agent.isStopped)
        {
            updateTarget();
            
        }
            
        else
            agent.velocity = Vector3.zero;
        if(canRaycast)
            raycast();
    }

    //returns, from the current path, the closest waypoint
    private Vector3 ClosestWaypointOnPath()
    {
        
        currentWaypoint = 0;
        float minDIst = Vector3.Distance(this.transform.position, waypoints[currentWaypoint].position);
        //find closest waypoint on the current path
        for(int i=0; i<waypoints.Length;i++)
        { 
            float checkDist = Vector3.Distance(this.transform.position, waypoints[i].position);
            if (checkDist < minDIst)
            {
                 var v =   waypoints[i].position - this.transform.position;
                //check if the transform is behind
                if (Vector3.Dot(v, this.transform.forward) > 0)
                {
                   // Debug.Log("DOT:" +Vector3.Dot(transform.forward,v));
                    currentWaypoint = i;
                    minDIst = checkDist;
                }
                else
                {
                    currentWaypoint = i + 1;
                    if (currentWaypoint > waypoints.Length - 1)//reset to 0
                        currentWaypoint = 0;
                }
                
                
            }
                
        }

       // Debug.Log(waypoints[currentWaypoint].name +" is "+minDIst);
       

        return waypoints[currentWaypoint].position;
    }

    //shoots rays to detect obstacles
    void raycast()
    {
        float hitDist;
        GameObject obstacle = GetDetectedObstacles(out hitDist);

        
        //Check if we hit something
        if (obstacle != null)
        {

            NavMeshAgent otherVehicle = null;
            otherVehicle = obstacle.GetComponent<NavMeshAgent>();
            AICarBehaviour otherCar = obstacle.GetComponent<AICarBehaviour>();
            if (otherVehicle != null)
            {
                //Check if it's front vehicle
                float dotFront = Vector3.Dot(this.transform.forward, otherVehicle.transform.forward);

                //If detected front vehicle speed is lower, decrease max speed
                if (otherVehicle.speed < agent.speed && dotFront > .8f)
                {      
                    agent.speed = (otherVehicle.speed - 0.25f);
                }

                //If the two vehicles are too close, and facing the same direction, brake the ego vehicle
                if (hitDist < emergencyBrakeDist && dotFront > .8f)
                {
                    stateMachine.TransitionTo("Stop");
                }

                if (otherCar.stateMachine.CurrentState == otherCar.stateMachine.GetState("Go"))
                {
                   // Debug.Log("They are going");
                    if (this.stateMachine.CurrentState != this.stateMachine.GetState("Go"))
                    {
                       
                        Invoke("CanRaycast", 1f);
                    }
                        
                }
                else
                {
                    this.stateMachine.TransitionTo("Stop");
                }
                    
                
                 if (hitDist < slowDownDist)
                {
                    agent.speed = maxSpeed * 0.5f;
                }
            }

            
        }
    }

    void CanRaycast()
    {
        //canRaycast = true;
        this.stateMachine.TransitionTo("Go");
        //Debug.Log("Going");
    }

    //figure out if the obstacle is noteable
    GameObject GetDetectedObstacles(out float _hitDist)
    {
        GameObject detectedObstacle = null;
        float minDist = 1000f;

        float initRay = (raysNumber / 2f) * raySpacing;
        float hitDist = -1f;
        for (float a = -initRay; a <= initRay; a += raySpacing)
        {
            CastRay(raycastAnchor.transform.position, a, this.transform.forward, raycastLength, out detectedObstacle, out hitDist);

            if (detectedObstacle == null) continue;

            float dist = Vector3.Distance(this.transform.position, detectedObstacle.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                break;
            }
        }

        _hitDist = hitDist;
        return detectedObstacle;
    }


    void CastRay(Vector3 _anchor, float _angle, Vector3 _dir, float _length, out GameObject _outObstacle, out float _outHitDistance)
    {
        _outObstacle = null;
        _outHitDistance = -1f;

        //Draw raycast
        Debug.DrawRay(_anchor, Quaternion.Euler(0, _angle, 0) * _dir * _length, new Color(1, 0, 0, 0.5f));

        

        RaycastHit hit;
        if (Physics.Raycast(_anchor, Quaternion.Euler(0, _angle, 0) * _dir, out hit, _length))
        {
            if (hit.collider.gameObject.tag == "damageable")//only detect other cars
            {
                _outObstacle = hit.collider.gameObject;
                _outHitDistance = hit.distance;
            }
            
        }
    }

    //once the target is reached, a new target is set
    void updateTarget()
    {
        if(Vector3.Distance(transform.position, waypoints[currentWaypoint].position) <= agent.stoppingDistance)
        {
            //reached waypoint
            currentWaypoint++;
            if (currentWaypoint > waypoints.Length - 1)//reset to 0
                currentWaypoint = 0;
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }
    
}
