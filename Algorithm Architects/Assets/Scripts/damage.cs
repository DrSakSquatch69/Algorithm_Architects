using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class damage : MonoBehaviour
{
    //
    [SerializeField] damageType type;
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
        if (type == damageType.bullet || type == damageType.butter || type == damageType.bouncing || type == damageType.fire)
        {
            //rb.velocity = transform.forward * bulletSpeed;
            rb.velocity = (gameManager.instance.getPlayer().transform.position - transform.position) * bulletSpeed;
            Destroy(gameObject, despawnTimer);
        }

        else if (type == damageType.chaser)
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

        if (dmg != null && type != damageType.stationary)
        {
            dmg.takeDamage(damageAmount, -(transform.position - other.transform.position).normalized * (damageAmount / 2), type);

            if (type == damageType.butter)
            {
                //Marks off the player as getting buttered and then sets the speed change
                gameManager.instance.setIsButtered(true);
                playerSpeedChange /= butterSlowAmount;
                gameManager.instance.setPlayerSpeed(playerSpeedChange);
                playerSpeedChange = gameManager.instance.getOriginalPlayerSpeed();
            }

            if(type == damageType.fire)
            {
                gameManager.instance.setIsOnFire(true);
            }
        }
        else if(type == damageType.stationary)
        {
            if(dmg != null)
            {
                InvokeRepeating("ApplyStationaryDamageFog", 0f, damageInterval);
            }
        }
        
        if(type == damageType.bouncing)
        {
            if (dmg == null)
            {
                if (other.CompareTag("Wall"))
                {
                    transform.forward = -transform.forward;
                }
                else
                {
                    transform.forward = -transform.forward;
                    rb.velocity = new Vector3(rb.velocity.x, bulletSpeed, rb.velocity.z);
                }
                
            } else
            {
                Destroy(gameObject);
            }
        }

        if (type == damageType.bullet || type == damageType.chaser || type == damageType.butter || type == damageType.fire)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(type == damageType.stationary)
        {
            CancelInvoke("ApplyStationaryDamageFog");
        }
    }

    private void ApplyStationaryDamageFog()
    {
        if(dmg != null)
        {
            dmg.takeDamage(damageAmount, Vector3.zero, type);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (type == damageType.chaser)
        {
            rb.velocity = (gameManager.instance.getPlayer().transform.position - transform.position).normalized * bulletSpeed * Time.deltaTime;
        }
    }
}
