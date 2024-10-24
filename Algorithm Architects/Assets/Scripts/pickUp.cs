using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//commments
public class pickUp : MonoBehaviour
{
    public PlayerController playerScript;

    [SerializeField] gunStats gun;
    public GameObject gunPOS;

    bool itemIsPickedUp;
    private void Start()
    {
        gun.ammo = gun.magSize;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !itemIsPickedUp)
        {
            itemIsPickedUp = true;
            playerScript.AddGunToInventory(gunPOS);
            playerScript.SwitchGun(gunPOS);
            Destroy(gameObject);
        }
    }
}
