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
    private GameObject [] waypointsGO;
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
        //Debug.Log(currentWaypoint);
        

    }

    //switch followed waypoints
    public void UpdateCurrentPath()
    {
        Array.Resize(ref waypoints, currentPath.transform.childCount);
        for (int i = 0; i < currentPath.transform.childCount; i++)
        {
            waypoints[i] = currentPath.transform.GetChild(i);
        }

        agent.SetDestination(ClosestWaypointOnPath());
        Debug.Log("New path: "+currentPath.name);
    }
   

    // Update is called once per frame
    void Update()
    {
        updateTarget();
    }

    private Vector3 ClosestWaypointOnPath()
    {
        //find closest waypoint on the current path
        foreach (Transform waypoint in waypoints)
        {
            if (Vector3.Distance(transform.position, waypoint.position) < Vector3.Distance(transform.position, waypoints[currentWaypoint].position))
                currentWaypoint++;
        }
        return waypoints[currentWaypoint].position;
    }

    void updateTarget()
    {
        if(Vector3.Distance(transform.position, waypoints[currentWaypoint].position) <= agent.stoppingDistance)
        {
            //reached waypoint
            currentWaypoint++;
            if (currentWaypoint > waypoints.Length - 1)
                currentWaypoint = 0;
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }
    
}
