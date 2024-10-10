using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //need this to access scene manager

public class ButtonFns : MonoBehaviour
{
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

    public void NextLevel()
    {

        //if in unity editor it just quits, but if in actual game, then it loads the main game
#if  UNITY_EDITOR
        quit();
#else
        if (SceneManager.GetActiveScene().name == "Tutorial") //Checks to see what level the player is in, and then loads the next level.
        {
            SceneManager.LoadScene("Level 1 Growth");
        }
        else if(SceneManager.GetActiveScene().name == "Level 1 Growth")
        {
            SceneManager.LoadScene("Level 2 Growth");
        }
        else if (SceneManager.GetActiveScene().name == "Level 2 Growth")
        {
            SceneManager.LoadScene("Level 3 Growth");
        }
        else if (SceneManager.GetActiveScene().name == "Level 3 Growth")
        {
            SceneManager.LoadScene("Level 4 Growth");
        }
        else if (SceneManager.GetActiveScene().name == "Level 4 Growth")
        {
            SceneManager.LoadScene("Level 5 Growth");
        }
        else if (SceneManager.GetActiveScene().name == "Level 5 Growth")
        {
            SceneManager.LoadScene("Level 6 Growth");
        }
        else if (SceneManager.GetActiveScene().name == "Level 6 Growth")
        {
            SceneManager.LoadScene("Level 7 Growth");
        }
        else if (SceneManager.GetActiveScene().name == "Level 7 Growth")
        {
            SceneManager.LoadScene("Level 8 Growth");
        }
        else if (SceneManager.GetActiveScene().name == "Level 8 Growth")
        {
            SceneManager.LoadScene("Final Growth");
        }
        resume();;
#endif
 
    }
}
