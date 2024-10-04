using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    // Start is called before the first frame update

    [SerializeField] GameObject menuActive, menuPause;

    float timeScaleOrig; // original timeScale

    public bool isPaused;
    public bool getIsPaused() //getter for our is paused bool
    {
        return isPaused;
    }
    public void setIsPaused(bool paused)  // setter for is paused bool 
    {
        isPaused = paused;
    }
    void Start()
    {
        instance = this;
        timeScaleOrig = Time.timeScale; // setting the original time scale to reset after pause
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            else if (menuActive == menuPause) 
            {
                stateUnpause();
            }
        }
    }
    public void statePause()
    {
        isPaused = !isPaused; // toggles on and off
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused; // toggles on and off
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }
}
