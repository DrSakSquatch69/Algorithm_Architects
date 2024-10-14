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


    float playerSpeedChange;



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

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(damageAmount);

            if (type == damageTypes.butter)
            {
                //Marks off the player as getting buttered and then sets the speed change
                gameManager.instance.setIsButtered(true);
                playerSpeedChange /= butterSlowAmount;
                gameManager.instance.setPlayerSpeed(playerSpeedChange);
                playerSpeedChange = gameManager.instance.getOriginalPlayerSpeed();
            }
        }

        if (type == damageTypes.bullet || type == damageTypes.chaser || type == damageTypes.butter)
        {
            Destroy(gameObject);
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
