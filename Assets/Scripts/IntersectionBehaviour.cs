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
    Queue<AICarBehaviour> carsWaiting=new Queue<AICarBehaviour>();
    Queue<AICarBehaviour> carsInIntersection = new Queue<AICarBehaviour>();
    //Queue<GameObject> carsWaiting=new Queue<GameObject>();
    private float decisionTime = 2;
    public GameObject[] paths;
    // Start is called before the first frame update
    void Start()
    {
        carsInIntersection = transform.GetChild(0).GetComponent<IntersectionHolder>().carsInIntersection;
        
        InvokeRepeating("nextCarGo", decisionTime,decisionTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "damageable")//will change later
        {
            AICarBehaviour car = other.gameObject.GetComponent<AICarBehaviour>();
            if (car != null)
            {
;               carsWaiting.Enqueue(car);
                car.stateMachine.TransitionTo("Stop");
                car.currentPath = paths[Random.Range(0, paths.Length)];//choose a new path for the car
               // car.UpdateCurrentPath();


            }
        }
        
    }


    //every x seconds, a car can go    
    void nextCarGo()
    {
        if (carsWaiting != null && carsWaiting.Count>0)//is a car waiting?
        {

            if (carsInIntersection.Count < 1 )//no cars in the intersection
            {
                //Debug.Log(carsInIntersection.Peek().gameObject.tag);
                carsWaiting.Peek().stateMachine.TransitionTo("Go");
                //foreach (AICarBehaviour car in carsWaiting)
                //{
                //car.stateMachine.TransitionTo("Go");
                //}
                carsWaiting.Dequeue();
            }/*
            else
            {
                Debug.Log("THeres a car in the intersection");
            }*/
           
        }
    }
    
}
