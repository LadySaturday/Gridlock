using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
/// <summary>
/// Spawn particles and act prooperly as a powerup (speed, health, invis)
/// </summary>
public class Powerup : MonoBehaviour
{
    
    public CarController carController;
    public PlayerController playerController;
    public GameObject particles;
    private AudioSource audio;

    private void Start()
    {
        audio = transform.parent.GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.tag == "Player")
        {
            audio.Play();
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
                case "heart":
                    playerController.gainHealth();
                    break;
                default://nothin
                    break;

                    
            }
            Instantiate(particles, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
