using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IntersectionBehaviour : MonoBehaviour
{
    /// <summary>
    /// every car that enters intersection will be stopped
    /// 1st in, 1st out )queue)
    /// make path decision and go
    /// </summary>
    Queue<AICarBehaviour> carsInIntersection;

    public GameObject[] paths;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        AICarBehaviour car = other.GetComponent<AICarBehaviour>();
        if (car != null)
        {
            car.stateMachine.TransitionTo("Stop");
            Debug.Log("Making new path");
            car.currentPath = paths[Random.Range(0, paths.Length)];//choose a new path for the car
            car.UpdateCurrentPath();
        }
        
        //carsInIntersection.Enqueue(car);
           //next step is to wait, start movement of the car, and dequeue
    }
    
}
