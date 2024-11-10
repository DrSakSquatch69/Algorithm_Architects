using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
    public static MenuMusicManager Instance;
    [SerializeField] AudioSource MainMenuAmbient;
    [SerializeField] public AudioSource PlayButtonClick;
    [SerializeField] AudioSource QuitButtonClick;
    public bool isMainMenu;
    [SerializeField] AudioClip playButtonClip;
    public float getPlayButtonLength()
    {
        return playButtonClip.length;
    }
    void Start()
    {
        if (Instance == null)
        {
            Instance = this; // makes sure we only have one in the scene

        }
        else
        {
            Destroy(this);
        }
        if(isMainMenu) { MainMenuAmbient.Play(); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopAmbientSound()
    {
        MainMenuAmbient.Stop();
    }
    public void PlayButtonSound()
    {
        if (!PlayButtonClick.isPlaying) { PlayButtonClick.Play(); }
    }
    public void QuitButtonSound()
    {
        if (!QuitButtonClick.isPlaying) { QuitButtonClick.Play(); }
    }

}
