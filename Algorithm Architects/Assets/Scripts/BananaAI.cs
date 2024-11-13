using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BananaAI : MonoBehaviour, IDamage
{
    [SerializeField] int viewAngle;
    float angleToPlayer;

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPosition;
    [SerializeField] Transform shootPosition;
    [SerializeField] GameObject bullet;
    [SerializeField] float firerate;
    [SerializeField] int rotateSpeed;
    [SerializeField] int maxBurstAmount;
    [SerializeField] float timeBetweenShots;
    public int damageAmount;  // Bullet damage
    [SerializeField] float travelDistance = 10f; // Adjustable travel distance
    [SerializeField] float travelSpeed = 20f;    // Adjustable travel speed
    int burstAmount;

    int hpOrig;
    [SerializeField] int HP;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int maxRespawns = 0;

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

    int currentRespawnCount = 1;

    [SerializeField] Slider enemyHpBar;
    public bool isSliderOn;

    void Start()
    {
        colorOrig = model.material.color;
        hpOrig = HP;
        render = GetComponent<Renderer>();
        gameManager.instance.updateGameGoal(1);

        ignoreMask = LayerMask.GetMask("Enemy");
        updateEnemyUI();
    }

    void Update()
    {
        updateEnemyUI();
        agent.SetDestination(gameManager.instance.getPlayer().transform.position);

        if (playerSighted && canSeePlayer())
        {
            // Engage the player with rapid fire when in sight
            if (!isShooting)
            {
                StartCoroutine(Shoot());
            }
        }
    }
    bool canSeePlayer()
    {
        playerDirection = gameManager.instance.getPlayer().transform.position - headPosition.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);
        Debug.DrawRay(headPosition.position, playerDirection);

        RaycastHit hit;
        if (Physics.Raycast(headPosition.position, playerDirection, out hit, 1000, ~ignoreMask))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }
                return true;
            }
        }
        return false;
    }

    public void takeDamage(int amount, Vector3 dir, damageType type)
    {
        HP -= amount;
        updateEnemyUI();

        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            if (currentRespawnCount < maxRespawns)
            {
                GameObject enemy1 = Instantiate(enemyPrefab, transform.position + Vector3.right, Quaternion.identity);
                GameObject enemy2 = Instantiate(enemyPrefab, transform.position + Vector3.left, Quaternion.identity);

                enemy1.GetComponent<AppleAI>().SetRespawnCount(currentRespawnCount + 1);
                enemy2.GetComponent<AppleAI>().SetRespawnCount(currentRespawnCount + 1);

                gameManager.instance.updateGameGoal(+1);
            }
            else
            {
                gameManager.instance.updateGameGoal(-1);
            }

            Destroy(gameObject);
        }
    }

      public void updateEnemyUI()
    {
        float dist = Vector3.Distance(transform.position, gameManager.instance.getPlayer().transform.position);  //get the distance between the player and enemy

        // Debug log to check the objects
        Debug.Log("gameManager.instance: " + (gameManager.instance == null ? "null" : "initialized"));
        Debug.Log("gameManager.instance.getPlayer(): " + (gameManager.instance.getPlayer() == null ? "null" : "initialized"));
        Debug.Log("enemyHpBar: " + (enemyHpBar == null ? "null" : "initialized"));
        Debug.Log("Camera.main: " + (Camera.main == null ? "null" : "initialized"));

        if (dist <= renderDistance)
        {
            if (enemyHpBar != null)
            {
                enemyHpBar.gameObject.SetActive(true);
                enemyHpBar.value = (float)HP / hpOrig;
                if (Camera.main != null)
                {
                    enemyHpBar.transform.rotation = Camera.main.transform.rotation;
                }
                else
                {
                   Debug.Log("Camera.main is null.");
                }
                isSliderOn = true;
            }
            else
            {
                Debug.Log("enemyHpBar is null.");
            }
        }
        else
        {
            if (enemyHpBar != null)
            {
                enemyHpBar.gameObject.SetActive(false);
            }
            isSliderOn = false;
        }
    }

    public void SetRespawnCount(int respawnCount)
    {
        currentRespawnCount = respawnCount;
    }

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

        // Instantiate the initial bullet
        GameObject initialBullet = Instantiate(bullet, shootPosition.position, transform.rotation);
        burstAmount++;

        // Start the coroutine to handle the bullet travel and split
        StartCoroutine(BulletTravelAndSplit(initialBullet));

        if (burstAmount != maxBurstAmount)
        {
            yield return new WaitForSeconds(timeBetweenShots);
            StartCoroutine(Shoot());
        }
        else
        {
            yield return new WaitForSeconds(firerate);
            isShooting = false;
            burstAmount = 0;
        }
    }

    IEnumerator BulletTravelAndSplit(GameObject bullet)
    {
        if (bullet == null)
        {
            Debug.Log("Bullet is null at the start of the coroutine.");
            yield break;  // Exit early if the bullet is already null
        }

        Vector3 startPosition = bullet.transform.position;
        Vector3 forwardDirection = bullet.transform.forward;

        // Travel straight for the set distance
        while (bullet != null && Vector3.Distance(startPosition, bullet.transform.position) < travelDistance)
        {
            // Ensure that the bullet is still valid
            if (bullet == null)
            {
                Debug.Log("Bullet was destroyed during movement.");
                yield break;  // Exit if the bullet is destroyed
            }

            bullet.transform.position += forwardDirection * travelSpeed * Time.deltaTime;
            yield return null;
        }

        // Now that the bullet has traveled the set distance, split and destroy the bullet
        if (bullet != null)
        {
            Destroy(bullet);  // Destroy the bullet after it has traveled the required distance
        }

        // Check if the bullet is destroyed after moving the set distance
        if (bullet == null)
        {
            Debug.Log("Bullet was destroyed after traveling the set distance, exiting coroutine.");
            yield break;  // Exit if the bullet is destroyed
        }

        // Split bullet handling (instantiate new bullets for the split)
        Vector3 middleDirection = forwardDirection;
        Vector3 leftDirection = Quaternion.Euler(0, -22, 0) * forwardDirection;
        Vector3 leftDirection2 = Quaternion.Euler(0, -45, 0) * forwardDirection;
        Vector3 rightDirection = Quaternion.Euler(0, 22, 0) * forwardDirection;
        Vector3 rightDirection2 = Quaternion.Euler(0, 45, 0) * forwardDirection;

        // Instantiate the split bullets
        GameObject middleBullet = Instantiate(this.bullet, bullet.transform.position, Quaternion.identity);
        GameObject leftBullet = Instantiate(this.bullet, bullet.transform.position, Quaternion.identity);
        GameObject leftBullet2 = Instantiate(this.bullet, bullet.transform.position, Quaternion.identity);
        GameObject rightBullet = Instantiate(this.bullet, bullet.transform.position, Quaternion.identity);
        GameObject rightBullet2 = Instantiate(this.bullet, bullet.transform.position, Quaternion.identity);

        // Ensure the bullets were instantiated correctly
        if (middleBullet == null || leftBullet == null || leftBullet2 == null || rightBullet == null || rightBullet2 == null)
        {
            Debug.Log("One or more split bullets were not instantiated correctly.");
            yield break;  // Exit if any split bullet was not instantiated correctly
        }

        // Assign velocities directly to ensure proper movement
        middleBullet.GetComponent<Rigidbody>().velocity = middleDirection * travelSpeed;
        leftBullet.GetComponent<Rigidbody>().velocity = leftDirection * travelSpeed;
        leftBullet2.GetComponent<Rigidbody>().velocity = leftDirection2 * travelSpeed;
        rightBullet.GetComponent<Rigidbody>().velocity = rightDirection * travelSpeed;
        rightBullet2.GetComponent<Rigidbody>().velocity = rightDirection2 * travelSpeed;

        // Set the damage amount for the split bullets
        middleBullet.GetComponent<damage>().damageAmount = damageAmount;
        leftBullet.GetComponent<damage>().damageAmount = damageAmount;
        leftBullet2.GetComponent<damage>().damageAmount = damageAmount;
        rightBullet.GetComponent<damage>().damageAmount = damageAmount;
        rightBullet2.GetComponent<damage>().damageAmount = damageAmount;
    }
}
