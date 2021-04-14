using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class options : MonoBehaviour
{
    public AudioMixer mixer;

    public void toggleFacts(bool factsOn)
    {
        if (factsOn)
            PlayerPrefs.SetInt("facts", 1);
        else
            PlayerPrefs.SetInt("facts", 0);
    }

    public void setMaster(float sliderValue)
    {
        mixer.SetFloat("masterVol", Mathf.Log10(sliderValue) * 20);
    }
    public void setSfx(float sliderValue)
    {
        mixer.SetFloat("sfxVol", Mathf.Log10(sliderValue) * 20);
    }
    public void setBgVol(float sliderValue)
    {
        mixer.SetFloat("bgCol", Mathf.Log10(sliderValue) * 20);
    }
  
}
