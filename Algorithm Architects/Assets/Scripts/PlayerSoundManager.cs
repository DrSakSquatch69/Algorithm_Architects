using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public static PlayerSoundManager Instance;

    [SerializeField] AudioSource walking;
    [SerializeField] AudioSource running;
    [SerializeField] AudioSource crouched;
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
        
        if(!walking.isPlaying)
        {
            walking.Play();
        }
    }
    public void StopWalking()
    {
        if(walking.isPlaying)
        {
            walking.Stop();
        }
    }

    public void PlayCrouch()
    {
        if(crouched.isPlaying)
        {
            crouched.Play();
        }
    }

    public void StopCrouch()
    {
        crouched.Stop();
    }
    public void PlayRun()
    {
        if(running.isPlaying)
        {
            running.Play();
        }
    }

    public void StopRun()
    {
        running.Stop();
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
