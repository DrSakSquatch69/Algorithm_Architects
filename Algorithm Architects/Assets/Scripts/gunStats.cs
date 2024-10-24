using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject gunModel;
    public int shootDamage;
    public float shootRate;
    public int shootDist;
    public int magSize;
    public int ammoremaining;
    public int ammo;
    public bool isMelee;

    public ParticleSystem hitEffect;
    public AudioClip shootSound;
}
