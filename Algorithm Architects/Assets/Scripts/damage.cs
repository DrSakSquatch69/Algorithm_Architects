using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class damage : MonoBehaviour
{
    [SerializeField] enum damageTypes { bullet, chaser, stationary, butter }
    [SerializeField] damageTypes type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] int bulletSpeed;
    [SerializeField] int despawnTimer;
    [SerializeField] float butterSlowAmount;
    [SerializeField] float damageInterval;

    float playerSpeedChange;
    IDamage dmg;

    // Start is called before the first frame update
    void Start()
    {
        if (type == damageTypes.bullet || type == damageTypes.butter)
        {
            //rb.velocity = transform.forward * bulletSpeed;
            rb.velocity = (gameManager.instance.getPlayer().transform.position - transform.position) * bulletSpeed;
            Destroy(gameObject, despawnTimer);
        }

        else if (type == damageTypes.chaser)
        {
            Destroy(gameObject, despawnTimer);
        }

        playerSpeedChange = gameManager.instance.getOriginalPlayerSpeed();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        dmg = other.GetComponent<IDamage>();

        if (dmg != null && type != damageTypes.stationary)
        {
            dmg.takeDamage(damageAmount, -(transform.position - other.transform.position).normalized * (damageAmount / 2));

            if (type == damageTypes.butter)
            {
                //Marks off the player as getting buttered and then sets the speed change
                gameManager.instance.setIsButtered(true);
                playerSpeedChange /= butterSlowAmount;
                gameManager.instance.setPlayerSpeed(playerSpeedChange);
                playerSpeedChange = gameManager.instance.getOriginalPlayerSpeed();
            }
        }
        else if(type == damageTypes.stationary)
        {
            if(dmg != null)
            {
                InvokeRepeating("ApplyStationaryDamageFog", 0f, damageInterval);
            }
        }

        if (type == damageTypes.bullet || type == damageTypes.chaser || type == damageTypes.butter)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(type == damageTypes.stationary)
        {
            CancelInvoke("ApplyStationaryDamageFog");
        }
    }

    private void ApplyStationaryDamageFog()
    {
        if(dmg != null)
        {
            dmg.takeDamage(damageAmount, Vector3.zero);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (type == damageTypes.chaser)
        {
            rb.velocity = (gameManager.instance.getPlayer().transform.position - transform.position).normalized * bulletSpeed * Time.deltaTime;
        }
    }
}
