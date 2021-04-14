using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class options : MonoBehaviour
{
    public AudioMixer mixer;


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
