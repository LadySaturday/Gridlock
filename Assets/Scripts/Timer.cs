using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


//quicky hacky thing to prep for playtest
public class Timer : MonoBehaviour
{

    public float timeLeft = 60f;//60 seconds to work for now
    public Text startText; 


    void Update()
    {
        timeLeft -= Time.deltaTime;
        startText.text = (timeLeft).ToString("0")+": seconds to get to work!";
        if (timeLeft < 0)
        {
            SceneManager.LoadScene("Lost");
        }
    }
}