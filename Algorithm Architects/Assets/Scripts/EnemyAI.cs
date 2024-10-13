using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPosition;
    [SerializeField] Transform shootPosition;
    [SerializeField] GameObject bullet;
    [SerializeField] float firerate;
    [SerializeField] int rotateSpeed;

    int hpOrig;                                 //Original HP
    [SerializeField] int HP;
    [SerializeField] GameObject enemyPrefab;    //Refrence to the enemy prefab
    [SerializeField] int maxRespawns = 0;       //limit the amound of respawns the enemy has


    Color colorOrig;
    Vector3 playerDirection;
    GameObject playerObj;

    bool isShooting;
    bool playerSighted;

    int currentRespawnCount = 1;
    //int activeEnemiesAI; //Used for tracking the active enemies 

    public Image enemyHp;
    Image enemyHpBar;
    

    // Start is called before the first frame update
    void Start()
    {
        //stores the original color
        colorOrig = model.material.color;
        hpOrig = HP;                                //set original hp
        enemyHpBar = Instantiate(enemyHp, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
        gameManager.instance.updateGameGoal(1);

        updateEnemyUI();
    }

    // Update is called once per frame
    void Update()
    {
        updateEnemyUI();
        playerDirection = gameManager.instance.getPlayer().transform.position - transform.position;
        agent.SetDestination(new Vector3(gameManager.instance.getPlayer().transform.position.x, gameObject.transform.position.y, gameManager.instance.getPlayer().transform.position.z));
       // activeEnemiesAI = GameObject.FindGameObjectsWithTag("Enemy").Length; //Checks for the current amount of remaining active enemies
                                                                             

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

    

    public void takeDamage(int amount)
    {
        HP -= amount;
        updateEnemyUI();

        StartCoroutine(flashColor());

        //when hp is zero or less, it destroys the object
        if (HP <= 0)
        {
           // --activeEnemiesAI;
           // gameManager.instance.ActiveCheck(activeEnemiesAI);

            // Check if enemy can respawn
            if (currentRespawnCount < maxRespawns)
            {
                

                //Creates two new enemies when this one dies
                GameObject enemy1 = Instantiate(enemyPrefab, transform.position + Vector3.right, Quaternion.identity); // offset position so theyre not stacked
                GameObject enemy2 = Instantiate(enemyPrefab, transform.position + Vector3.left, Quaternion.identity); // offset position so theyre not stacked
                
                // Set the respawn count of the new enemies to be 1 more than the current enemy
                enemy1.GetComponent<EnemyAI>().SetRespawnCount(currentRespawnCount + 1);
                enemy2.GetComponent<EnemyAI>().SetRespawnCount(currentRespawnCount + 1);
                
                //Increment the game goal by 1 for each new enemy
                gameManager.instance.updateGameGoal(+1);
                
            }
            else 
            {
                // No more respawns allowed, decrement the game goal
                gameManager.instance.updateGameGoal(-1);
               
            }
            
            // Destroys current enemy
            Destroy(gameObject);
            
            //if (gameManager.instance.ActiveCheck(activeEnemiesAI))
            //{
            //    gameManager.instance.Waves();
            //}
        }
    }
   
    public void updateEnemyUI()
    {
        enemyHpBar.fillAmount = (float)HP / hpOrig;                                                                     //update enemy hp bar fill amount
        enemyHpBar.transform.position = Camera.main.WorldToScreenPoint(headPosition.position);                          //transform from screen space to world space, and always face the screen
        float dist = 1/Vector3.Distance(transform.position, gameManager.instance.getPlayer().transform.position) * 2f;  //get the distance between the player and enemy
        if(dist > 1f)                                                                                                   //so that dist does not get larger the further away you get
        {
            dist = 0.25f;
        }
        dist = Mathf.Clamp(dist, 0.25f, 1f);                                                                            //set min and max for what dist can be
        enemyHpBar.transform.localScale = new Vector3(dist, dist, 0);                                        //set scale based on distance
    }

    public void SetRespawnCount(int respawnCount)
    {
        currentRespawnCount = respawnCount;
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


