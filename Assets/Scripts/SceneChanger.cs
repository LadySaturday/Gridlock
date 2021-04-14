using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public AudioSource sfx;

    public void buttonClicked(string name)
    {
        sfx.Play();
        SceneManager.LoadScene(name);
    }
}
