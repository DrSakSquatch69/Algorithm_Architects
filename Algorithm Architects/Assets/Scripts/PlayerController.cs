using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SearchService;

public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    //Field for Health
    int HealthPoints;
    [SerializeField] int maxHP;

    //Fields for movement
    [SerializeField] float speed;
    [SerializeField] float sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    //Fields for shooting
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    //Value must be below the normal size for it to be a crouch
    [SerializeField] float crouchSizeYAxis;
    [SerializeField] float slideDistance;
    [SerializeField] float slideSpeedMod;
    [SerializeField] float slideDelay;
    [SerializeField] float crouchHeight;
    float normalHeight;

    //Shooting Fields
    int ammo;
    [SerializeField] int ammoremaining;
    [SerializeField] int magSize;
    bool isReloading;
    bool isShooting;

    //Player Health Fields
    [SerializeField] float healDelayTime; //how long the player needs to not take damage
    [SerializeField] float healRate; //the healrate of the player
    bool isTakingDamage; //checks if the player is taking damage

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    float startTimer;

    //Sprint Fields
    float holdingSprintTime;
    bool isSprinting;

    //Crouching Fields
    bool isCrouching;
    bool isSliding;
    bool canSlide;
    bool crouching;

    //BouncePad Fields
    public LayerMask bouncePad;
    [SerializeField] float bouncePadForce;
    bool isBouncePad;

    //WallRunning Fields
    public LayerMask whatIsWall;
    bool isWallRight, isWallLeft;
    [SerializeField] Transform orientation;
    [SerializeField] int wallRunGrav;
    [SerializeField] int wallRunSpeed;
    int origGrav;
    bool isWallRunning;
    bool isSpawnProtection;

    //used to see if the player is grounded in debug mode

    // Start is called before the first frame update
    void Start()
    {
        //initiates the normal size
        jumpCount = 0;
        origGrav = gravity;
        ammo = magSize;
        HealthPoints = maxHP;
        normalHeight = controller.height;

        gameManager.instance.UpdateAmmoCounter(ammo, ammoremaining);
        updatePlayerUI();
        isSpawnProtection = true;
        StartCoroutine(spawnProtection());
    }

    //Function for spawn protection
    IEnumerator spawnProtection()
    {
        yield return new WaitForSeconds(1);
        isSpawnProtection = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Used to see where the player is looking
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        Movement();
        sprint();
        CheckForWall();
        WallRunInput();
        CheckForBouncePad();
    }

    //Function for player movement
    void Movement()
    {
        //checks if the player is on the ground, if yes then reset jump count and player velocity
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        //if the player is crouching, then dive the speed by 3, to make the player move slower
        if (crouching && !isSliding)
        {
            controller.Move(moveDir * (speed / 3) * Time.deltaTime);
        }
        //If player is wallrunning increase player speed
        else if (isWallRunning)
        {
            controller.Move(moveDir * wallRunSpeed * Time.deltaTime);
        }
        //If player is sliding, call the slide function
        else if (isSliding)
        {
            //starts the slide
            StartCoroutine(Slide());
        }
        //if player is not crouching, then player speed is set to default
        else if (!crouching)
        {
            controller.Move(moveDir * speed * Time.deltaTime);
        }

        //if player jumps, then increase players Y velocity
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);

        if (Input.GetButton("Fire1") && gameManager.instance.getIsPaused() != true && !isShooting && !isReloading) //added code so it doesnt shoot when clicking in menu
        {
            StartCoroutine(shoot());
        }

        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = !isCrouching;
            crouch();
        }

        if (Input.GetButton("Reload") && !isReloading)
        {
            StartCoroutine(reload());
        }
    }

    //Function for slide distance
    IEnumerator Slide()
    {
        if (canSlide && holdingSprintTime >= slideDelay)
        {
            //slide speed
            controller.Move(moveDir * (speed * slideSpeedMod) * Time.deltaTime);
            //slide duration
            yield return new WaitForSeconds(slideDistance);
            isCrouching = false;
            isSliding = false;
            //calls crouch to bring player back to normal size
            crouch();
            holdingSprintTime = 0;

        }
    }

    //function for delay between slides
    IEnumerator slidingDelay()
    {
        canSlide = false;
        yield return new WaitForSeconds(slideDelay);
        canSlide = true;
    }

    //sprint function
    void sprint()
    {
        StartCoroutine(slidingDelay());


        if (Input.GetButtonDown("Sprint"))
        {
            startTimer = Time.time; //Timer for slide delays
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
            holdingSprintTime = 0;
        }

    }

    //crouch function
    void crouch()
    {
        //if the player is crouching and not running or sliding then set the players height to crouch height
        if (isCrouching && !isSprinting && !isSliding)
        {
            //changes the player Y size to the crouch size
            controller.height = crouchHeight; //Change character controller instead of actual model size
            crouching = true;                                         //Make enemies easier to see (try making them bigger first)

        }
        //if sprinting then start slide
        else if (isCrouching && isSprinting && !isSliding)
        {
            holdingSprintTime = Time.time - startTimer;

            if (canSlide && holdingSprintTime >= slideDelay) //checking if the player can slide and also if they held the key for the correct amount of time
            {
                isSliding = true;
                controller.height = crouchHeight;
                canSlide = false;
            }
            else
            {
                isCrouching = false;
            }
        }
        //if the player is not crouching, then set the player height back to the normal height
        else if (!isCrouching)
        {
            //changes the player Y size to the normal size
            controller.height = normalHeight;
            crouching = false;
        }
    }

    //function for shooting
    IEnumerator shoot()
    {
        if (ammo > 0)
        {
            isShooting = true;
            ammo--;
            gameManager.instance.UpdateAmmoCounter(ammo, ammoremaining);

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
            {
                // Debug.Log(hit.collider.name);

                IDamage dmg = hit.collider.GetComponent<IDamage>();
                DestroyableBullet damage = hit.collider.GetComponent<DestroyableBullet>();

                if (dmg != null)
                {
                    dmg.takeDamage(shootDamage);
                    StartCoroutine(gameManager.instance.ActivateDeactivateHitMarker());
                }

                if (damage != null)
                {
                    damage.takeDamage(shootDamage);
                    StartCoroutine(gameManager.instance.ActivateDeactivateHitMarker());
                }
            }

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
        else
        {
            StartCoroutine(reload());
        }
    }

    //function for taking damage
    public void takeDamage(int amount)
    {
        if (!isSpawnProtection)
        {
            HealthPoints -= amount;
            StartCoroutine(gameManager.instance.hitFlash());
            updatePlayerUI();
            isTakingDamage = true;
            StartCoroutine(healDelay());

            if (HealthPoints <= 0)
            {
                gameManager.instance.youLose();
            }
        }
        else
        {
            return;
        }
    }

    //function for updating the player UI
    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HealthPoints / maxHP;
    }

    //function for the wallrun input
    void WallRunInput()
    {
        if (isWallRight && !isCrouching && !controller.isGrounded)
        {
            StartWallRun();
        }
        if (isWallLeft && !isCrouching && !controller.isGrounded)
        {
            StartWallRun();
        }
    }

    //function that starts the wallrun
    void StartWallRun()
    {
        jumpCount = 0;

        //Resets fall speed when player first starts wall running
        if (!isWallRunning)
        {
            playerVel = Vector3.zero;
        }

        gravity = wallRunGrav;
        isWallRunning = true;
    }

    //function that stops the wallrun
    void StopWallRun()
    {
        isWallRunning = false;
        gravity = origGrav;
    }

    //function that checks for a wall with a raycast
    void CheckForWall()
    {
        //Uses a raycast to check for wall on left and wall on right
        isWallRight = Physics.Raycast(transform.position, orientation.right, 1f, whatIsWall);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 1f, whatIsWall);

        if (!isWallLeft && !isWallRight)
        {
            StopWallRun();
        }

    }

    //function that checks for a bouncepad with a raycast
    void CheckForBouncePad()
    {
        //uses a raycast to see if there is a bounce pad underneath the player.
        isBouncePad = Physics.Raycast(transform.position, -orientation.up, 1.7f, bouncePad);

        //gave the player the option to not bounce if crouching
        if (isBouncePad && !crouching)
        {
            playerVel = Vector3.zero;
            playerVel.y = bouncePadForce;
            jumpCount = 0;
        }
    }

    //function for reloading
    IEnumerator reload()
    {
        if (ammoremaining <= 0) //if no more remaining ammo, then let player know that they have no ammo
        {
            isReloading = true;
            gameManager.instance.NoAmmoOnOff();
            yield return new WaitForSeconds(0.5f);
            isReloading = false;
            gameManager.instance.NoAmmoOnOff();

        }
        else if (!isReloading && ammo <= 0 && ammoremaining >= magSize) //if ammo is zero or less than and ammo remaining is more than mag size, then set ammo to magsize, and subtract magsize from ammo remaining
        {
            isReloading = true;
            gameManager.instance.reloadingOnOff();
            yield return new WaitForSeconds(0.5f);
            ammo = magSize;
            ammoremaining -= magSize;
            isReloading = false;
            gameManager.instance.reloadingOnOff();
        }
        else if (!isReloading && ammo >= 0 && ammoremaining >= magSize) //if there is still ammo in the mag then subtact that number to the magsize and use that difference to take away remaining ammo
        {
            isReloading = true;
            gameManager.instance.reloadingOnOff();
            yield return new WaitForSeconds(0.5f);
            ammoremaining -= magSize - ammo;
            ammo += magSize - ammo;
            isReloading = false;
            gameManager.instance.reloadingOnOff();
        }
        else if (!isReloading && ammo >= 0 && ammoremaining <= magSize) //checks if ammo left is greater than zero and the remaining ammo is less the a mag
        {
            if ((ammo + ammoremaining) <= magSize) //if the ammo left is equal to the mag size then add them together
            {
                isReloading = true;
                gameManager.instance.reloadingOnOff();
                yield return new WaitForSeconds(0.5f);
                ammo += ammoremaining;
                ammoremaining -= ammoremaining;
                isReloading = false;
                gameManager.instance.reloadingOnOff();
            }
            else if ((ammo + ammoremaining) > magSize) //if the ammo left is greater than the mag size then add how much it takes to fill the mag and subtract that number from the remaing ammo
            {
                isReloading = true;
                gameManager.instance.reloadingOnOff();
                yield return new WaitForSeconds(0.5f);
                ammoremaining -= magSize - ammo;
                ammo += magSize - ammo;
                isReloading = false;
                gameManager.instance.reloadingOnOff();
            }
        }
        gameManager.instance.UpdateAmmoCounter(ammo, ammoremaining);
    }

    //function that heals the player
    IEnumerator healPlayer()
    {
        if (!isTakingDamage && HealthPoints != maxHP) //if the player is not taking damage and is not at full health, then heal player
        {
            HealthPoints++;
            yield return new WaitForSeconds(healRate); //used to slowly heal the player
            StartCoroutine(healPlayer()); //restart function call
            updatePlayerUI(); //updates player ui
        }
    }

    //function that checks if the player hasnt received damage for a certain amount of time.
    IEnumerator healDelay()
    {
        yield return new WaitForSeconds(healDelayTime); //Waits before calling the healplayer function
        isTakingDamage = false;
        StartCoroutine(healPlayer());
    }
}

