using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICarBehaviour : MonoBehaviour
{
    /// <summary>
    /// logic:
    /// 
    /// 
    /// at intersection, car makes decision. Stay on this path or change paths?
    /// 
    /// States should be: Go, Stop, Slow
    /// 
    /// Go: car follows path via waypoint system
    /// Slow: car is slowing down for the car in front of it or to turn
    /// Stop: car is stopped, making decision for turning until car can go
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
    // Start is called before the first frame update
    void Start()
    {

        paths = GameObject.FindGameObjectsWithTag("path");
        agent = GetComponent<NavMeshAgent>();


        


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

    //switch followed waypoints
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
               // var v = this.transform.position - waypoints[i].position;
                //check if the transform is behind
                //if (Vector3.Dot(v, transform.forward) > 0)
               // {
                //    Debug.Log("DOT:" +Vector3.Dot(transform.forward,v));
                    currentWaypoint = i;
                    minDIst = checkDist;
               // }
                
                
            }
                
        }

       // Debug.Log(waypoints[currentWaypoint].name +" is "+minDIst);
       

        return waypoints[currentWaypoint].position;
    }

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

                //If detected front vehicle max speed is lower than ego vehicle, then decrease ego vehicle max speed
                if (otherVehicle.speed < agent.speed && dotFront > .8f)
                {
                    
                    agent.speed = (otherVehicle.speed - 0.5f);
                }

                //If the two vehicles are too close, and facing the same direction, brake the ego vehicle
                if (hitDist < emergencyBrakeDist && dotFront > .8f)
                {
                    stateMachine.TransitionTo("Stop");
                }

                if (otherCar.stateMachine.CurrentState == otherCar.stateMachine.GetState("Go"))
                {
                    Debug.Log("They are going");
                    if (this.stateMachine.CurrentState != this.stateMachine.GetState("Go"))
                    {
                        
                        //canRaycast = false;
                        Invoke("CanRaycast", 0.75f);//this is a terrible solution. 
                                                //This causes issues because there is no obstacle detection for a full second. 
                                                //find another solution
                    }
                        
                }
                    

                //If the two vehicles are getting close, slow down their speed
                /*
                else if (hitDist < slowDownDist)
                {
                    acc = .5f;
                    brake = 0f;
                    //wheelDrive.maxSpeed = Mathf.Max(wheelDrive.maxSpeed / 1.5f, wheelDrive.minSpeed);
                }*/
            }

            ///////////////////////////////////////////////////////////////////
            // Generic obstacles
            else
            {
                //Emergency brake if getting too close
                /*
                if (hitDist < emergencyBrakeThresh)
                {
                    acc = 0;
                    brake = 1;
                    wheelDrive.maxSpeed = Mathf.Max(wheelDrive.maxSpeed / 2f, wheelDrive.minSpeed);
                }

                //Otherwise if getting relatively close decrease speed
                else if (hitDist < slowDownThresh)
                {
                    acc = .5f;
                    brake = 0f;
                }*/
            }
        }
    }

    void CanRaycast()
    {
        //canRaycast = true;
        this.stateMachine.TransitionTo("Go");
        Debug.Log("Going");
    }

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
