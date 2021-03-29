using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls player health, powerups
/// </summary>
public class PlayerController : MonoBehaviour
{
    public int health = 10;
    private GameObject healthTxt;
    private bool canTakeDamage = true;
    private int carDamage = 1;
    private float waitBetweenDamage = 0.5f;

    public GameObject heartIcon;//prefab
    private GameObject[] heartIcons;
    // Start is called before the first frame update
    void Start()
    {
        healthTxt = GameObject.FindGameObjectWithTag("health");

        setupHealth();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void setupHealth()
    {
        if(heartIcons!=null&&heartIcons.Length>0)
        foreach (GameObject heart in heartIcons)
        {
            Destroy(heart);
        }

        Array.Resize(ref heartIcons,health);
        for(int i=0; i < health - 1; i++)
        {
            heartIcons[i] = Instantiate(heartIcon, healthTxt.transform);
            heartIcons[i].GetComponent<RectTransform>().localPosition =new Vector3(i*15, 0, 0);

        }
    }

    private IEnumerator takeDamage(int damage)
    {
        health -= damage;
        if (health > 1)
            setupHealth();
        else
            Debug.Log("Game OVERRRRRRRRRRR");

        yield return new WaitForSeconds(waitBetweenDamage);
        canTakeDamage = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "damageable")
        {
            if (canTakeDamage)
            {
                canTakeDamage = false;
                StartCoroutine(takeDamage(carDamage));
            }
            
        }
            
    }
}
