/*The first script to be called in script execution order. Shows the title screen and starts the game whem play is pressed. Attached to the Welcome panel*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 0;
    }

    public void OnPlayClick()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
