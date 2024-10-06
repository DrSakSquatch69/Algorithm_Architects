using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPosition;
    [SerializeField] Transform shootPosition;
    [SerializeField] GameObject bullet;
    [SerializeField] float firerate;
    [SerializeField] int rotateSpeed;

    [SerializeField] int HP;
    //[SerializeField] GameObject enemyPrefab; // Refrence to the enemy prefab
    //[SerializeField] int maxRespawns = 1; // adjust to limit the amound of respawns the enemy has


    Color colorOrig;
    Vector3 playerDirection;

    bool isShooting;
    bool playerSighted;

   // int currentRespawnCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        //stores the original color
        colorOrig = model.material.color;
        gameManager.instance.updateGameGoal(1);

    }

    // Update is called once per frame
    void Update()
    {
        playerDirection = gameManager.instance.getPlayer().transform.position - transform.position;
        agent.SetDestination(gameManager.instance.getPlayer().transform.position);

        if(playerSighted)
        {
            if(agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }

            if(!isShooting)
            {
                StartCoroutine(Shoot());
            }
        }
    }

    //public void SetRespawnCount(int respawnCount)
    //{
    //    currentRespawnCount = respawnCount;
    //}

    public void takeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(flashColor());

        //when hp is zero or less, it destroys the object
        if (HP <= 0)
        {
            //// Check if enemy can respawn
            //if (currentRespawnCount < maxRespawns)
            //{
            //    //gameManager.instance.updateGameGoal(2);

            //    //Creates two new enemies when this one dies
            //    GameObject enemy1 = Instantiate(enemyPrefab, transform.position + Vector3.right, Quaternion.identity); // offset position so theyre not stacked
            //    GameObject enemy2 = Instantiate(enemyPrefab, transform.position + Vector3.left, Quaternion.identity); // offset position so theyre not stacked

            //    // Set the respawn count of the new enemies to be 1 more than the current enemy
            //    enemy1.GetComponent<EnemyAI>().SetRespawnCount(currentRespawnCount + 1);
            //    enemy2.GetComponent<EnemyAI>().SetRespawnCount(currentRespawnCount + 1);

            //    //Increment the game goal by 1 for each new enemy
            //    gameManager.instance.updateGameGoal(2);

            //    // Increase the respawn count
            //    //currentRespawnCount++;
            //}
            //else
            //{
            //    // No more respawns allowed, decrement the game goal
            //    gameManager.instance.updateGameGoal(-1);
            //}
            gameManager.instance.updateGameGoal(-1);
            // Destroys current enemy
            Destroy(gameObject);
        }
    }

    //Sends feedback to the user that they are doing damage
    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    void faceTarget()
    {
        Quaternion rotate = Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rotate, Time.deltaTime * rotateSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSighted = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSighted = false;
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPosition.position, transform.rotation);
        yield return new WaitForSeconds(firerate);
        isShooting = false;
    }

}


