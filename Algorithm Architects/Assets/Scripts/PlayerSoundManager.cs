using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    [SerializeField] AudioSource footsteps;
    [SerializeField] AudioSource wallRun;
    [SerializeField] AudioSource doubleJump;
    [SerializeField] AudioSource landing;
    [SerializeField] AudioSource breathing;
    [SerializeField] AudioSource takingDamage;
    [SerializeField] AudioSource healthRecovery;
    [SerializeField] AudioSource interaction;

    void PlayFootsteps()
    {
        if(!footsteps.isPlaying)
        {
            footsteps.Play();
        }
    }

    void PlayWallRun()
    {
        if(!wallRun.isPlaying)
        {
            wallRun.Play();
        }
    }

    void PlayDoubleJump()
    {
        if(!doubleJump.isPlaying)
        {
            doubleJump.Play();
        }
    }

    void PlayLanding()
    {
        if(!landing.isPlaying)
        {
            landing.Play();
        }
    }
    
    void PlayBreathing()
    {
        if(!breathing.isPlaying)
        {
            breathing.Play();
        }
    }

    void PlayTakingDamage()
    {
        if(!takingDamage.isPlaying)
        {
            takingDamage.Play();
        }    
    }

    void PlayHealthRecovery()
    {
        if(!healthRecovery.isPlaying)
        {
            healthRecovery.Play();
        }
    }

    void playInteraction()
    {
        if(interaction.isPlaying)
        {
            interaction.Play();
        }
    }
}
