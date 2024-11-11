using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.VisualScripting;
using UnityEngine.UI;

public class StrawberryAI : MonoBehaviour
{
    enum damageTypes { bullet, chaser, stationary, butter, melee, bouncing, fire, tomato, cabbage, toxic }

    [SerializeField] int viewAngle;
    float angleToPlayer;

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPosition;
    [SerializeField] Transform shootPosition;
    [SerializeField] GameObject bullet;
    [SerializeField] float firerate;
    [SerializeField] int rotateSpeed;
    [SerializeField] float spinSpeed = 360f; // Speed of spin in degrees per second
    [SerializeField] float burstDuration = 2f; // Duration of the burst
    [SerializeField] float verticalAmplitude = 10f; // The range of up-and-down movement in degrees
    [SerializeField] float verticalFrequency = 2f;  // How quickly the bullets move up and down

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
        //enemyHpBar = Instantiate(enemyHp, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
        //enemyHpBar.transform.SetParent(gameManager.instance.enemyHpParent.transform);
        gameManager.instance.updateGameGoal(1);

        ignoreMask = LayerMask.GetMask("Enemy");
        updateEnemyUI();
    }

    void Update()
    {
        updateEnemyUI();

        // Set the destination to the player's position
        if (agent != null)
        {
            agent.SetDestination(gameManager.instance.getPlayer().transform.position);
        }

        // Always try to shoot if the player is within a certain distance
        float distanceToPlayer = Vector3.Distance(transform.position, gameManager.instance.getPlayer().transform.position);
        if (distanceToPlayer <= agent.stoppingDistance + 5f) // Adjust the range as needed
        {
            faceTarget();

            if (!isShooting)
            {
                StartCoroutine(SpinAndShoot());
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
                enemy1.GetComponent<StrawberryAI>().SetRespawnCount(currentRespawnCount + 1);
                enemy2.GetComponent<StrawberryAI>().SetRespawnCount(currentRespawnCount + 1);
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

    IEnumerator SpinAndShoot()
    {
        isShooting = true;
        float timeElapsed = 0f;
        int bulletCount = 200;
        float timeBetweenShots = burstDuration / bulletCount;

        while (timeElapsed < burstDuration)
        {
            // Rotate the strawberry horizontally
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);

            // Calculate the vertical angle using a sine wave for up-and-down motion
            float verticalAngle = Mathf.Sin(timeElapsed * verticalFrequency) * verticalAmplitude;

            // Create a rotation that includes both the spin and the vertical oscillation
            Quaternion bulletRotation = Quaternion.Euler(verticalAngle, transform.eulerAngles.y, 0);

            // Shoot a bullet in the calculated direction
            Instantiate(bullet, shootPosition.position, shootPosition.rotation);

            // Wait before shooting the next bullet
            yield return new WaitForSeconds(timeBetweenShots);

            timeElapsed += timeBetweenShots;
        }

        isShooting = false;
    }
}
