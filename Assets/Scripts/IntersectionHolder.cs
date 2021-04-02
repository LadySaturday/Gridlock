using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Prevents multiple cars form being in the intersection unless they are driving the same direction
/// </summary>
public class IntersectionHolder : MonoBehaviour
{
    public Queue<AICarBehaviour> carsInIntersection = new Queue<AICarBehaviour>();

    private void OnTriggerEnter(Collider other)
    {
        AICarBehaviour otherCar = other.GetComponent<AICarBehaviour>();
        if (otherCar != null)
        {
            carsInIntersection.Enqueue(otherCar);
            
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        AICarBehaviour otherCar = other.GetComponent<AICarBehaviour>();
        if (otherCar != null)
        {
            carsInIntersection.Dequeue();
        }
            
    }
}
