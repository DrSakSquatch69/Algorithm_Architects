using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunStats : ScriptableObject
{
    public GameObject gunModel;
    public int shootDamage;
    public int shootRate;
    public int shootDist;
    public int magSize;
    public int ammoremaining;
    public int ammo;

    public ParticleSystem hitEffect;
    public AudioClip shootSound;
}
