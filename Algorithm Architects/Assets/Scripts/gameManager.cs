using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance; //how we will access game manager
    // Start is called before the first frame update

    [SerializeField] GameObject menuActive, menuPause;

    float timeScaleOrig; // original timeScale
    public GameObject player; //player object so we can access our player through the game manager
 
    public bool isPaused; //variable to store wether we are paused or not
    public bool getIsPaused() //getter for our is paused bool
    {
        return isPaused; 
    }
    public void setIsPaused(bool paused)  // setter for is paused bool 
    {
        isPaused = paused;
    }
    void Awake() //awake always happens first  
    {
        instance = this;
        timeScaleOrig = Time.timeScale; // setting the original time scale to reset after pause
        player = GameObject.FindWithTag("Player"); //Tracks player's location
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null) //if active menu is null we are in game if not null we are in menu
            {
                statePause(); // calling Fn to create the paused state
                menuActive = menuPause; // setting active menu variable
                menuActive.SetActive(isPaused); //setting menu active via our variable
            }
            else if (menuActive != null) //if we have an active menu
            {
                stateUnpause(); //change game state
            }
        }
    }
    public void statePause() // changes game state to a paused state
    {
        isPaused = !isPaused; // toggles on and off
        Time.timeScale = 0; //sets the game time to zero so nothing can happen while paused
        Cursor.visible = true; //make cursor visible
        Cursor.lockState = CursorLockMode.Confined; //confine cursor to game screen
    }

    public void stateUnpause() //changes game state to un paused
    {
        isPaused = !isPaused; // toggles on and off
        Time.timeScale = timeScaleOrig; // sets our time scale to active using our variable we stored orig timescale in 
        Cursor.visible = false; //rendering cursor not visible
        Cursor.lockState = CursorLockMode.Locked; //locking the cursor
        menuActive.SetActive(false);//setting the active menu to inactive
        menuActive = null;//and changes our var back to null
    }
}
