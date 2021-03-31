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
    Queue<AICarBehaviour> carsInIntersection=new Queue<AICarBehaviour>();
    //Queue<GameObject> carsInIntersection=new Queue<GameObject>();
    private float decisionTime = 5;
    public GameObject[] paths;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("nextCarGo", decisionTime,decisionTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "damageable")//will change later
        {
            AICarBehaviour car = other.gameObject.GetComponent<AICarBehaviour>();
            if (car != null)
            {
;               carsInIntersection.Enqueue(car);
                car.stateMachine.TransitionTo("Stop");
                car.currentPath = paths[Random.Range(0, paths.Length)];//choose a new path for the car
               // car.UpdateCurrentPath();


            }
        }
        
        
        
           //next step is to wait, start movement of the car, and dequeue
    }


    //every x seconds, a car can go
    
    void nextCarGo()
    {
        if (carsInIntersection != null && carsInIntersection.Count>0)
        {
            carsInIntersection.Peek().stateMachine.TransitionTo("Go");
            carsInIntersection.Dequeue();
        }
    }
    
}
