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
    public  GameObject currentPath;
    private int currentWaypoint=0;

    public StateMachine stateMachine;
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
        Debug.Log("New path: " + currentPath.name);
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
            updateTarget();
        else
            agent.velocity = Vector3.zero;
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
                Debug.Log(checkDist+" is less than "+minDIst);
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

        Debug.Log(waypoints[currentWaypoint].name +" is "+minDIst);
       

        return waypoints[currentWaypoint].position;
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
