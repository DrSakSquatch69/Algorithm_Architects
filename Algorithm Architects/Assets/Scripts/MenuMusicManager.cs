using System.Collections;
using System.Collections.Generic;
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
    public void PlayButtonSound()
    {
        if (!PlayButtonClick.isPlaying) { PlayButtonClick.Play(); }
    }
    public void QuitButtonSound()
    {
        int randomIndex = Random.Range(0, QuitQuips.Count - 1);
        QuitButtonClick.clip = QuitQuips[randomIndex];
        curQuitQuip = QuitQuips[randomIndex];
        QuitButtonClick.Play();
    }

    public void PlayWinRestart()
    {
        if(!WinRestart.isPlaying) { WinRestart.Play(); }
    }

    public void PlayWinMainMenu()
    {
        if(!WinMMBtn.isPlaying) { WinMMBtn.Play(); }   
    }

    public void PlaySettings()
    {
        if (!SettingsUp.isPlaying) { SettingsUp.Play(); }
    }

    public void PlayNextLevelMenUp()
    {
        if (!NextLevMenuUp.isPlaying) { NextLevMenuUp.Play(); }
    }

    public void PlayLoseRestart()
    {
        if (!LoseRestart.isPlaying) { LoseRestart.Play(); }
    }

    public void PlayFinWin()
    {
        if (!FinalWin.isPlaying) { FinalWin.Play(); }
    }

    public void PlayResume()
    {
        int randomIndex = Random.Range(0, ResumeClips.Count - 1);
        ResumeButtons.clip = ResumeClips[randomIndex];
        curResumeClip = ResumeClips[randomIndex];
        ResumeButtons.Play();
    }

    public void PlayPauseUp()
    {
        int randomIndex = Random.Range(0, OnPauseQuips.Count - 1);
        OnPauseButton.clip = OnPauseQuips[randomIndex];
        curPauseQuip = OnPauseQuips[randomIndex];
        OnPauseButton.Play();
    }

    public void PlayNextLevel()
    {
        int randomIndex = Random.Range(0, NextLevelClips.Count-1);
        NextLevelButton.clip = NextLevelClips[randomIndex];
        curNextLevelClip = NextLevelClips[randomIndex];
        NextLevelButton.Play();
    }

    public void PlayLoseUp()
    {
        int randomIndex = Random.Range(0, LoseMenuUpClips.Count - 1);
        LoseMenuUp.clip = LoseMenuUpClips[randomIndex];
        curLoseUpClip = LoseMenuUpClips[randomIndex];
        LoseMenuUp.Play();
    }
}
