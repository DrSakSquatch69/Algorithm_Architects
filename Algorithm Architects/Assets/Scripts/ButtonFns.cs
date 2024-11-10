using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; //need this to access scene manager
using UnityEngine.Audio;
using System;


//      

public class ButtonFns : MonoBehaviour
{
    //[SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider sensSlider;
    [SerializeField] AudioMixer musicMixer;
    [SerializeField] AudioMixer SFXMixer;
    [SerializeField] Slider SFXSliderSlide;
    [SerializeField] Slider MusicSliderSlide;
    [SerializeField] MenuMusicManager mMusicManager;
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            if (MainManager.Instance.GetSensitivity() != 0.0f && MainManager.Instance.GetSFXVolume() != -1 && MainManager.Instance.GetMusicsVolume() != -1)
            {
                sensSlider.value = MainManager.Instance.GetSensitivity();
                SFXSliderSlide.value = MainManager.Instance.GetSFXVolume();
                MusicSliderSlide.value = MainManager.Instance.GetMusicsVolume();
                SFXMixer.SetFloat("SFXVolume", Mathf.Log10(SFXSliderSlide.value) * 20);
                musicMixer.SetFloat("MusicVolume", Mathf.Log10(MusicSliderSlide.value) * 20);
            }
            else
            {
                Debug.Log("Went through else statement");
                sensSlider.value = 800;
                MainManager.Instance.SetSensitivity(sensSlider.value);
                SFXSliderSlide.value = 1;
                MainManager.Instance.SetSFXVolume(SFXSliderSlide.value);
                MusicSliderSlide.value = 1;
                MainManager.Instance.SetMusicVolume(MusicSliderSlide.value);
            }
        }
    }
    IEnumerator ResumeBtn()
    {
        mMusicManager.StopAmbientSound();
        mMusicManager.PlayResume();
        yield return new WaitForSeconds(3.25f);
        gameManager.instance.stateUnpause(); //just call gamemanager and call our unpause state fn
    }
    public void resume() // resume fn
    {
        StartCoroutine(ResumeBtn());
    }
 

    IEnumerator WinRestartBtn()
    {
        mMusicManager.StopAmbientSound();
        mMusicManager.PlayWinRestart();
        yield return new WaitForSeconds(mMusicManager.WinRestart.clip.length);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //we have to access scene manager and load scene. have to call scenemanager to also get active scene
        resume(); // unpause
    }
    public void Winrestart() // restart fn
    {
        StartCoroutine(WinRestartBtn());
    }


    IEnumerator QuitQuip()
    {
        mMusicManager.StopAmbientSound();
        mMusicManager.QuitButtonSound();
        yield return new WaitForSeconds(mMusicManager.curQuitQuip.length);
#if UNITY_EDITOR //C sharp if statement
        UnityEditor.EditorApplication.isPlaying = false; // if in the editor we need to access the editor application and quit the game through here
#else
        Application.Quit(); //if not in editor just quit application
#endif
    }
    public void quit() // quit Fn
    {
        StartCoroutine(QuitQuip());
    }


    public void settings() //Tells gameManager the settings menu is up and brings the menu up
    {
        mMusicManager.PlaySettings();
        gameManager.instance.setInSettings(true);
        gameManager.instance.settingsMenuUp();
    }

    public void sensitivitySlider(float sensitivity) //Gets the slider info to send to gameManager which sends it to camera controller
    {
        //Debug.Log(sensitivity);
        gameManager.instance.setSens(sensitivity);
        MainManager.Instance.SetSensitivity(sensitivity);
    }

    //public void volumeSlider(float volume)
    //{
    //    audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
    //   // audioSource.Play(); //Issue was I was never playing the audio clip in the first place so set it to play when the player shoots
    //}

    public void MusicSlider(float volume)
    {
        musicMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        MainManager.Instance.SetMusicVolume(volume);
    }

    public void SFXSlider(float volume)
    {
        SFXMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        MainManager.Instance.SetSFXVolume(volume);
    }


    IEnumerator NextLevelBtnPlay()
    {
        mMusicManager.StopAmbientSound();
        mMusicManager.PlayNextLevel();
        yield return new WaitForSeconds(mMusicManager.curNextLevelClip.length);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1); //Gets the index of the current level and loads the next scene after it
        resume();
    }
    public void NextLevelBtn()
    {
        StartCoroutine(NextLevelBtnPlay());
    }

    IEnumerator PlayButton()
    {
        mMusicManager.StopAmbientSound();
        mMusicManager.PlayButtonSound();
        yield return new WaitForSeconds(mMusicManager.playButtonClip.length);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void PlayGame() //i had to make this for the main menu, because the nextlevel function would crash the game, due to resume being called
    {
        StartCoroutine(PlayButton());
    }

    public void PlayTutorial() //goes to the tutorial
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator WinMMBtn()
    {
        mMusicManager.StopAmbientSound();
        mMusicManager.PlayWinMainMenu();
        yield return new WaitForSeconds(mMusicManager.WinMMbtnClip.length);
        SceneManager.LoadScene(0);
        gameManager.instance.stateUnpauseMainMenu();
    }
    public void ReturnToMainMenu()
    {
        StartCoroutine(WinMMBtn());
    }

    IEnumerator LoseRestartBtn()
    {
        mMusicManager.StopAmbientSound();
        mMusicManager.PlayLoseRestart();
        yield return new WaitForSeconds(mMusicManager.WinRestartClip.length);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //we have to access scene manager and load scene. have to call scenemanager to also get active scene
        resume(); // unpause
    }
    public void LoseRestart()
    {
        StartCoroutine(LoseRestartBtn());
    }
}
