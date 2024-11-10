using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class OrangeAI : MonoBehaviour, IDamage
{
    enum damageTypes { bullet, chaser, stationary, butter }

    [SerializeField] Animator orangeAnimator;
    [SerializeField] Renderer model;
    [SerializeField] Transform headPosition;
    [SerializeField] Transform shootPosition;
    [SerializeField] GameObject bullet;
    [SerializeField] float firerate;
    [SerializeField] int distancePlayerIsSeen;

    int hpOrig;                                 //Original HP
    [SerializeField] int HP;
    [SerializeField] GameObject enemyPrefab;    //Refrence to the enemy prefab
    [SerializeField] int maxRespawns = 0;       //limit the amound of respawns the enemy has

    [SerializeField] float minHPSize;
    [SerializeField] float maxHPSize;
    [SerializeField] float renderDistance;
    LayerMask ignoreMask;

    Color colorOrig;
    Vector3 playerDirection;
    GameObject playerObj;
    Renderer render;

    bool isShooting;
    bool playerSighted;
    bool PlayerTooClose;
    bool isOpen;
    bool isClosed;

    int currentRespawnCount = 1;
    //int activeEnemiesAI; //Used for tracking the active enemies 

    public Image enemyHp;
    Image enemyHpBar;
    public bool isImgOn;


    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        hpOrig = HP;                                //set original hp
        render = GetComponent<Renderer>();        //getting the renderer of the game object
        enemyHpBar = Instantiate(enemyHp, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
        enemyHpBar.transform.SetParent(gameManager.instance.enemyHpParent.transform);
        gameManager.instance.updateGameGoal(1);

        ignoreMask = LayerMask.GetMask("Enemy");
        updateEnemyUI();

        if(Vector3.Distance(transform.position, gameManager.instance.getPlayer().transform.position) <= 10)
        {
            PlayerTooClose = true;
            orangeAnimator.SetBool("isClosed", true);
            orangeAnimator.SetTrigger("Close");
        }
        else 
        { 
            PlayerTooClose = false;
            orangeAnimator.SetBool("isOpen", true);
            orangeAnimator.SetTrigger("Open");
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateEnemyUI();
        // activeEnemiesAI = GameObject.FindGameObjectsWithTag("Enemy").Length; //Checks for the current amount of remaining active enemies
        PlayerDist();

        if (canSeePlayer())
        {

        }
    }

    private void PlayerDist()
    {
        float dist = Vector3.Distance(transform.position, gameManager.instance.getPlayer().transform.position);
        if (dist < distancePlayerIsSeen && !PlayerTooClose)
        {
            isOpen = true;
            isClosed = false;
        }
        else
        {
            isOpen = false;
            isClosed = true;
        }
    }
    public void Damage(int amount)
    {
        HP -= amount;
        if(HP <= 0)
        {
            Destroy(gameObject);
        }
    }
    bool canSeePlayer()
    {
        playerDirection = gameManager.instance.getPlayer().transform.position - headPosition.position;
        RaycastHit hit;
        Debug.DrawRay(headPosition.position, playerDirection.normalized * distancePlayerIsSeen, Color.red);

        if (Physics.Raycast(headPosition.position, playerDirection, out hit, distancePlayerIsSeen, ~ignoreMask))
        {
            if (hit.collider.CompareTag("Player"))
            { 
                    if (!isShooting && isOpen)
                    {
                        StartCoroutine(Shoot());
                    }
                return true;
            }
        }
        return false;
    }

    public void updateEnemyUI()
    {
        float dist = Vector3.Distance(transform.position, gameManager.instance.getPlayer().transform.position);  //get the distance between the player and enemy

        if (render.isVisible && dist <= renderDistance)
        {                                                                                             //see if enemy model is on screen
            enemyHpBar.enabled = true;
            isImgOn = true;
            enemyHpBar.fillAmount = (float)HP / hpOrig;                                                                     //update enemy hp bar fill amount
            enemyHpBar.transform.position = Camera.main.WorldToScreenPoint(headPosition.position);                          //transform from screen space to world space, and always face the screen
            dist = 1 / dist * 10f;
            dist = Mathf.Clamp(dist, minHPSize, maxHPSize);                                                                            //set min and max for what dist can be
            enemyHpBar.transform.localScale = new Vector3(dist, dist, 0);                                        //set scale based on distance
        }
        else
        {
            enemyHpBar.enabled = false;                                                                         //turn off health bar if enemy is not on screen
            isImgOn = false;
        }
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerTooClose = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerTooClose = false;
        }
    }

    void Animate()
    {
        if(isOpen)
        {
            orangeAnimator.SetBool("isOpen", true);
            orangeAnimator.SetBool("isClose", false);
            orangeAnimator.SetTrigger("Open");
        }
        else if(isClosed)
        {
            orangeAnimator.SetBool("isOpen", false );
            orangeAnimator.SetBool("isClose", true );
            orangeAnimator.SetTrigger("Close");
        }
    }
    IEnumerator Shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPosition.position, transform.rotation);
        yield return new WaitForSeconds(firerate);
        isShooting = false;
    }
    
    public void takeDamage(int amount, Vector3 dir, damageType type)
    {
       
    }
}
