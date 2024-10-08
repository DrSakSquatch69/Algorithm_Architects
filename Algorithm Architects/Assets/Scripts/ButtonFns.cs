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

    public void MainGame()
    {

        //if in unity editor it just quits, but if in actual game, then it loads the main game
    #if UNITY_EDITOR
        quit();
    #else
         SceneManager.LoadScene("Main Game Scene");
        resume();;
    #endif
    }
}
