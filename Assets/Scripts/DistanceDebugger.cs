using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceDebugger : MonoBehaviour
{

    public Transform car;
    public Transform waypoint;
    // Start is called before the first frame update
    void Start()
    {
        float dis = Vector3.Distance(car.position, waypoint.position);
        Debug.Log("Distance between car and waypoint:"+dis);

        dis = Vector3.Distance(waypoint.position,car.position);
        Debug.Log("Distance between waypoint and car:" + dis);

        dis = Vector3.Distance(car.localPosition, waypoint.position);
        Debug.Log("Distance between local car and waypoint:" + dis);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
