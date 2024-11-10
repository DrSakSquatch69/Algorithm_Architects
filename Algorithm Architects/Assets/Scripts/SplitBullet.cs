using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
//
public class SplitBullet : MonoBehaviour
{
    enum damageTypes { bullet, chaser, stationary, butter, melee, bouncing, fire, tomato, cabbage, toxic, TSplit }

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

    public Image enemyHp;
    Image enemyHpBar;
    public bool isImgOn;

    void Start()
    {
        colorOrig = model.material.color;
        hpOrig = HP;
        render = GetComponent<Renderer>();
        enemyHpBar = Instantiate(enemyHp, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
        enemyHpBar.transform.SetParent(gameManager.instance.enemyHpParent.transform);
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

                if (!isShooting)
                {
                    StartCoroutine(Shoot());
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
        float dist = Vector3.Distance(transform.position, gameManager.instance.getPlayer().transform.position);

        if (render.isVisible && dist <= renderDistance)
        {
            enemyHpBar.enabled = true;
            isImgOn = true;
            enemyHpBar.fillAmount = (float)HP / hpOrig;
            enemyHpBar.transform.position = Camera.main.WorldToScreenPoint(headPosition.position);
            dist = 1 / dist * 10f;
            dist = Mathf.Clamp(dist, minHPSize, maxHPSize);
            enemyHpBar.transform.localScale = new Vector3(dist, dist, 0);
        }
        else
        {
            enemyHpBar.enabled = false;
            isImgOn = false;
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
        Vector3 startPosition = bullet.transform.position;
        Vector3 forwardDirection = bullet.transform.forward;

        // Travel straight for the set distance
        while (Vector3.Distance(startPosition, bullet.transform.position) < travelDistance)
        {
            bullet.transform.position += forwardDirection * travelSpeed * Time.deltaTime;
            yield return null;
        }

        // Destroy the original bullet after traveling the distance
        Destroy(bullet);

        // Define the split directions for T-shape
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
