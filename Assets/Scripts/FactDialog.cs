using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// show player fact dialogs
/// </summary>
public class FactDialog : MonoBehaviour
{
    public string fact;
    public Text text;
    public Text factsReadText;
    private int numFactsRead=0;
    public GameObject panel;
    private bool isActive=false;
    private bool factsOn=true;

    private void Start()
    {
        if (PlayerPrefs.GetInt("facts") == 0)
            factsOn = false;
        else
            factsOn = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(isActive)
            if(Input.anyKeyDown)
            {
                Time.timeScale = 1;
                panel.SetActive(false);
                Destroy(this.gameObject);
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(factsOn)
            if(other.tag=="Player")
            {
                numFactsRead++;
                isActive = true;
                Time.timeScale = 0;
                panel.SetActive(true);
                text.text = fact+" (press any key to continue)";
                factsReadText.text = (numFactsRead.ToString() + "/5 facts read");
            }
    }


}
