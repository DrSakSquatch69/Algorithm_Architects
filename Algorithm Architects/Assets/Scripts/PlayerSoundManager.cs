using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public static PlayerSoundManager Instance;

    [SerializeField] AudioSource footstepSource;
    [SerializeField] AudioSource wallRun;
    [SerializeField] AudioSource doubleJump;
    [SerializeField] AudioSource landing;
    [SerializeField] AudioSource takingDamage;
    [SerializeField] AudioSource Sliding;
    [SerializeField] AudioSource Bounce;
    [SerializeField] AudioSource BulletDamage;
    [SerializeField] AudioSource MeleeDamage;
    [SerializeField] AudioSource ChaserDamage;
    [SerializeField] AudioSource ButterDamage;
    [SerializeField] AudioSource StationaryDamage;

    [SerializeField] float runPitch;
    [SerializeField] float walkPitch;
    [SerializeField] float crouchPitch;
    [SerializeField] float crouchVolume;

    private bool hasLanded = false;
    private float audioOrigVolume;
    private float audioOrigPitch;
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
    public void PlayWalking()
    {
        footstepSource.pitch = walkPitch;
        footstepSource.volume = audioOrigVolume;
        if(!footstepSource.isPlaying)
        {
            footstepSource.Play();
        }
    }
    public void StopWalking()
    {
        if(footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
    }

    public void PlayCrouch()
    {
        audioOrigVolume = footstepSource.volume;
        audioOrigPitch = footstepSource.pitch;
        footstepSource.pitch = crouchPitch;
        footstepSource.volume = crouchVolume;
        footstepSource.Play();
    }

    public void StopCrouch()
    {
        footstepSource.Stop();
        footstepSource.volume = audioOrigVolume;
        footstepSource.pitch = audioOrigPitch;
    }
    public void PlayRun()
    {
        footstepSource.pitch = runPitch;
        footstepSource.Play();
    }

    public void StopRun()
    {
        footstepSource.Stop();
        footstepSource.pitch = audioOrigPitch;
    }
    public void PlayWallRun()
    {
        if(!wallRun.isPlaying)
        {
            wallRun.Play();
        }
    }

    public void stopWallRun()
    {
        if(wallRun.isPlaying)
        {
            wallRun.Stop();
        }
    }

    public void PlayDoubleJump()
    {
        if(!doubleJump.isPlaying)
        {
            doubleJump.Play();
        }
    }

    public void PlayLanding(bool isGrounded)
    {
        if (isGrounded&& !hasLanded)
        {
            landing.Play();
            hasLanded = true;
        }
        else if (!isGrounded)
        {
            hasLanded= false;
        }
    }

    public void PlayTakingDamage()
    {
        if(!takingDamage.isPlaying)
        {
            takingDamage.Play();
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

    public void PlayBounce()
    {
        if (!Bounce.isPlaying)
        {
            Bounce.Play();
        }
    }    

    public void PlayBulletDMG()
    {
        if(!BulletDamage.isPlaying)
        {
            BulletDamage.Play();
        }
    }

    public void PlayMeleeDMG()
    {
        if(!MeleeDamage.isPlaying)
        {
            MeleeDamage.Play();
        }
    }

    public void PlayChaserDMG()
    {
        if(!ChaserDamage.isPlaying)
        {
            ChaserDamage.Play();
        }
    }

    public void PlayButterDMG()
    {
        if (!ButterDamage.isPlaying)
        {
            ButterDamage.Play();
        }
    }

    public void PlayStationaryDMG()
    {
        if(!StationaryDamage.isPlaying)
        {
            StationaryDamage.Play();
        }
    }
}
