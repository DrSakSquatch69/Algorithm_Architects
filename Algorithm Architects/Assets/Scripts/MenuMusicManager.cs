using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
    public static MenuMusicManager Instance;
    [SerializeField] AudioSource Ambient;

    [SerializeField] public AudioSource PlayButtonClick;
    [SerializeField] public AudioClip playButtonClip;

    [SerializeField] public AudioSource QuitButtonClick;
    [SerializeField] List<AudioClip> QuitQuips;
    public AudioClip curQuitQuip;

    [SerializeField] public AudioSource LoseMenuUp;
    [SerializeField] List<AudioClip> LoseMenuUpClips;
    public AudioClip curLoseUpClip;

    [SerializeField] public AudioSource NextLevelButton;
    [SerializeField] List<AudioClip> NextLevelClips;
    public AudioClip curNextLevelClip;

    [SerializeField] public AudioSource OnPauseButton;
    [SerializeField] List<AudioClip> OnPauseQuips;
    public AudioClip curPauseQuip;

    [SerializeField] public AudioSource ResumeButtons;
    [SerializeField] List<AudioClip> ResumeClips;
    public AudioClip curResumeClip;

    [SerializeField] public AudioSource FinalWin;
    [SerializeField] AudioClip FinalWinClip;

    [SerializeField] public AudioSource LoseRestart;
    [SerializeField] public AudioClip LoseRestartClip;

    [SerializeField] public AudioSource NextLevMenuUp;
    [SerializeField] AudioClip NextLevMenUpClip;

    [SerializeField] public AudioSource SettingsUp;
    [SerializeField] AudioClip SettingClip;

    [SerializeField] public AudioSource WinMMBtn;
    [SerializeField] public AudioClip WinMMbtnClip;

    [SerializeField] public AudioSource WinRestart;
    [SerializeField] public AudioClip WinRestartClip;
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
        Ambient.Play();
    }
    public void PlayAmbient()
    {
        if (!Ambient.isPlaying) { Ambient.Play(); }
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void StopAmbientSound()
    {
        Ambient.Stop();
    }



    public void PlaySettings()
    {
        if (OnPauseButton.isPlaying || ResumeButtons.isPlaying || QuitButtonClick.isPlaying || NextLevelButton.isPlaying || LoseMenuUp.isPlaying)
        {
            OnPauseButton.Stop();
            ResumeButtons.Stop();
            QuitButtonClick.Stop();
            NextLevelButton.Stop();
            LoseMenuUp.Stop();
        }
       
        SettingsUp.Play();
    }

    public void PlayNextLevelMenUp()
    {
        if (!NextLevMenuUp.isPlaying) { NextLevMenuUp.Play(); }
    }


    public void PlayFinWin()
    {
        if (!FinalWin.isPlaying) { FinalWin.Play(); }
    }


    public void PlayPauseUp()
    {
        int randomIndex = Random.Range(0, OnPauseQuips.Count - 1);
        OnPauseButton.clip = OnPauseQuips[randomIndex];
        curPauseQuip = OnPauseQuips[randomIndex];

        if (SettingsUp.isPlaying || ResumeButtons.isPlaying || QuitButtonClick.isPlaying || NextLevelButton.isPlaying || LoseMenuUp.isPlaying)
        {
            NextLevelButton.Stop();
            ResumeButtons.Stop();
            QuitButtonClick.Stop();
            SettingsUp.Stop();
            LoseMenuUp.Stop();
        }

        OnPauseButton.Play();

    }


    public void PlayLoseUp()
    {
        int randomIndex = Random.Range(0, LoseMenuUpClips.Count - 1);
        LoseMenuUp.clip = LoseMenuUpClips[randomIndex];
        curLoseUpClip = LoseMenuUpClips[randomIndex];

        if (OnPauseButton.isPlaying || ResumeButtons.isPlaying || QuitButtonClick.isPlaying || SettingsUp.isPlaying)
        {
            OnPauseButton.Stop();
            ResumeButtons.Stop();
            QuitButtonClick.Stop();
            SettingsUp.Stop();
        }

        LoseMenuUp.Play();
    }
    public void PlayWinMainMenu()
    {
        WinMMBtn.Play();
    }
    public void PlayLoseRestart()
    {
        LoseRestart.Play();
    }

    public void PlayButtonSound()
    {
        PlayButtonClick.Play();
    }

    public void PlayNextLevel()
    {
        int randomIndex = Random.Range(0, NextLevelClips.Count - 1);
        NextLevelButton.clip = NextLevelClips[randomIndex];

        if (OnPauseButton.isPlaying || QuitButtonClick.isPlaying || ResumeButtons.isPlaying || SettingsUp.isPlaying)
        {
            OnPauseButton.Stop();
            ResumeButtons.Stop();
            QuitButtonClick.Stop();
            SettingsUp.Stop();
        }

        NextLevelButton.Play();
    }
    public void QuitButtonSound()
    {
        int randomIndex = Random.Range(0, QuitQuips.Count - 1);
        QuitButtonClick.clip = QuitQuips[randomIndex];

        if (OnPauseButton.isPlaying || ResumeButtons.isPlaying || SettingsUp.isPlaying || NextLevelButton.isPlaying || LoseMenuUp.isPlaying)
        {
            OnPauseButton.Stop();
            ResumeButtons.Stop();
            SettingsUp.Stop();
            NextLevelButton.Stop();
            LoseMenuUp.Stop();
        }

        QuitButtonClick.Play();
    }
    public void PlayWinRestart()
    {
        WinRestart.Play();

    }
    public void PlayResume()
    {

        int randomIndex = Random.Range(0, ResumeClips.Count - 1);
        ResumeButtons.clip = ResumeClips[randomIndex];

        if (OnPauseButton.isPlaying || SettingsUp.isPlaying || QuitButtonClick.isPlaying || LoseMenuUp.isPlaying)
        {
            OnPauseButton.Stop();
            LoseMenuUp.Stop();
            QuitButtonClick.Stop();
            SettingsUp.Stop();
        }

        ResumeButtons.Play();

    }
}
