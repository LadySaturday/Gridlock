using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class Powerup : MonoBehaviour
{
    private CarController carController;
    public GameObject particles;
    // Start is called before the first frame update
    void Start()
    {
        carController = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.tag == "Player")
        {
            switch (gameObject.tag)
            {
                case "speed":
                    //speed boost
                    carController.speedBoost();
                    break;
                case "invisible":
                    //car can go through objects
                    carController.invisible();
                    break;
                default://nothin
                    break;

                    
            }
            Instantiate(particles, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
