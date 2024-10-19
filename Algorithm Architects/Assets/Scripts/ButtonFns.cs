using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; //need this to access scene manager
using UnityEngine.Audio;


        

public class ButtonFns : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    //[SerializeField] AudioSource audioSource;

    private void Start()
    {
       // gameManager.instance.setSound(audioSource);
    }

    public void resume() // resume fn
    {
        gameManager.instance.stateUnpause(); //just call gamemanager and call our unpause state fn
    }
    public void restart() // restart fn
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //we have to access scene manager and load scene. have to call scenemanager to also get active scene
        resume(); // unpause
    }
    public void quit() // quit Fn
    {
#if UNITY_EDITOR //C sharp if statement
        UnityEditor.EditorApplication.isPlaying = false; // if in the editor we need to access the editor application and quit the game through here
#else
        Application.Quit(); //if not in editor just quit application
#endif
    }

    public void settings() //Tells gameManager the settings menu is up and brings the menu up
    {
        gameManager.instance.setInSettings(true);
        gameManager.instance.settingsMenuUp();
    }

    public void sensitivitySlider(float sensitivity) //Gets the slider info to send to gameManager which sends it to camera controller
    {
        //Debug.Log(sensitivity);
        gameManager.instance.setSens(sensitivity);
    }

    public void volumeSlider(float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
       // audioSource.Play(); //Issue was I was never playing the audio clip in the first place so set it to play when the player shoots
    }


    public void NextLevel()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1); //Gets the index of the current level and loads the next scene after it
        
        resume();
    }

    public void PlayGame() //i had to make this for the main menu, because the nextlevel function would crash the game, due to resume being called
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void PlayTutorial() //goes to the tutorial
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
        gameManager.instance.stateUnpauseMainMenu();
    }
}
