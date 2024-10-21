using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public static PlayerSoundManager Instance;

    [SerializeField] AudioSource footstepSource;
    public AudioClip[] footstepClips;

    [SerializeField] AudioSource wallRun;
    [SerializeField] AudioSource doubleJump;
    [SerializeField] AudioSource landing;
    [SerializeField] AudioSource breathing;
    [SerializeField] AudioSource takingDamage;
    [SerializeField] AudioSource healthRecovery;
    [SerializeField] AudioSource interaction;
    [SerializeField] AudioSource Sliding;


    [SerializeField] float runPitch;
    [SerializeField] float walkPitch;
    [SerializeField] float crouchPitch;
    [SerializeField] float crouchVolume;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayFootsteps(bool isRunning, bool isCrouched)
    {
        footstepSource.pitch = isRunning ? runPitch : walkPitch;
        if(!footstepSource.isPlaying && isCrouched == false)
        {
            footstepSource.Play();
        }
        else if(isCrouched == true)
        {
            footstepSource.pitch = crouchPitch;
            footstepSource.volume = crouchVolume;
            footstepSource.Play();
        }
    }

    public void PlayWallRun()
    {
        if(!wallRun.isPlaying)
        {
            wallRun.Play();
        }
    }

    public void PlayDoubleJump()
    {
        if(!doubleJump.isPlaying)
        {
            doubleJump.Play();
        }
    }

    public void PlayLanding()
    {
        if(!landing.isPlaying)
        {
            landing.Play();
        }
    }
    
    public void PlayBreathing()
    {
        if(!breathing.isPlaying)
        {
            breathing.Play();
        }
    }

    public void PlayTakingDamage()
    {
        if(!takingDamage.isPlaying)
        {
            takingDamage.Play();
        }    
    }

    public void PlayHealthRecovery()
    {
        if(!healthRecovery.isPlaying)
        {
            healthRecovery.Play();
        }
    }

    public void playInteraction()
    {
        if(interaction.isPlaying)
        {
            interaction.Play();
        }
    }

    public void playSliding()
    {
        if(!Sliding.isPlaying)
        {
            Sliding.Play();
        }
    }
    public void stopSliding()
    {
        if(Sliding.isPlaying)
        {
            Sliding.Stop();
        }
    }
}
