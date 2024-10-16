using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoManager : MonoBehaviour
{
    public float delay = 3.0f; // time to show the logo

    // Start is called before the first frame update
    void Start()
    {
        Invoke("LoadNextScene", delay);
    }

    // this function calls after the delay
    void LoadNextScene()
    {
        SceneManager.LoadScene("Main Menu"); // Replace with the actual scene name...is it main menu?
    }
}
