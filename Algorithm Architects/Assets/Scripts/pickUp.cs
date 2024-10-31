using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//commment
public class pickUp : MonoBehaviour
{
    [SerializeField] gunStats gun;
 
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
            gameManager.instance.playerScript.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
