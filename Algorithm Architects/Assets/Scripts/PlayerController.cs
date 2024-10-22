using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SearchService;

public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] PlayerSoundManager soundManager;
    [SerializeField] LayerMask ignoreMask;
    //Field for Health
    int HealthPoints;
    [SerializeField] int maxHP;

    //Fields for flashlight
    [SerializeField] GameObject flashLight;
    bool isFlashlight;

    //Fields for movement
    [SerializeField] float speed;
    [SerializeField] float sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] float butterSlowTimer;
    float originalSpeed;
    bool isSlowedByButter;
    bool cantSprint;

    //Fields for shooting
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    public ParticleSystem hitEffect;
    public AudioSource audioSource;

    //Value must be below the normal size for it to be a crouch
    [SerializeField] float crouchSizeYAxis;
    [SerializeField] float slideDistance;
    [SerializeField] float slideSpeedMod;
    [SerializeField] float slideDelay;
    [SerializeField] float crouchHeight;
    float normalHeight;


    int ammo;
    [SerializeField] int ammoremaining;
    [SerializeField] int magSize;
    bool isReloading;

    [SerializeField] float healDelayTime; //how long the player needs to not take damage
    [SerializeField] float healRate; //the healrate of the player
    bool isTakingDamage; //checks if the player is taking damage

    [SerializeField] int pushTimer;
    Vector3 moveDir;
    Vector3 playerVel;
    Vector3 pushDirection;

    int jumpCount;
    float startTimer;
    float holdingSprintTime;

    bool isSprinting;
    bool isShooting;
    bool isCrouching;
    bool isSliding;
    bool canSlide;
    bool crouching;

    //bouncepad fields
    public LayerMask bouncePad;
    [SerializeField] float bouncePadForce;
    bool isBouncePad;

    //Mud fields
    public LayerMask mud;
    [SerializeField] float mudSpeedMod;
    bool isMud;

    public LayerMask whatIsWall;
    bool isWallRight, isWallLeft;
    [SerializeField] Transform orientation;
    [SerializeField] int wallRunGrav;
    [SerializeField] int wallRunSpeed;
    int origGrav;
    bool isWallRunning;
    bool isSpawnProtection;

    //used to see if the player is grounded in debug mode

    //stores the normal Y size of the player capsule
    float normYSize;

    // Start is called before the first frame update
    void Start()
    {
        //initiates the normal size
        normYSize = transform.localScale.y;
        jumpCount = 0;
        origGrav = gravity;
        ammo = magSize;
        HealthPoints = maxHP;
        normalHeight = controller.height;
        originalSpeed = speed;
        gameManager.instance.setOriginalPlayerSpeed(speed);
        gameManager.instance.setPlayerSpeed(speed);
        gameManager.instance.setSound(audioSource);


        gameManager.instance.UpdateAmmoCounter(ammo, ammoremaining);
        updatePlayerUI();
        isSpawnProtection = true;
        StartCoroutine(spawnProtection());

    }

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
        CheckForMud();
    }

    bool isMoving()
    {
        return controller.velocity.magnitude > 0.1f;
    }
    void Movement()
    {
        pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushTimer * Time.deltaTime);

        if (gameManager.instance.getIsButtered() && !isSpawnProtection)
        {
            StartCoroutine(ButterSlow());
        }

        //checks if the player is on the ground, if yes then reset jump count and player velocity
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        //if the player is crouching, then dive the speed by 3, to make the player move slower
        if(crouching && !isSliding)
        {
            controller.Move(moveDir * (speed / 3) * Time.deltaTime);
        }
        else if (isWallRunning)
        {
            controller.Move(moveDir * wallRunSpeed * Time.deltaTime);
        }
        else if (isSliding)
        {
            //starts the slide
            StartCoroutine(Slide());
        }
        else if (!crouching)
        {
            controller.Move(moveDir * speed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move((playerVel + pushDirection) * Time.deltaTime);

        if (Input.GetButton("Fire1") && gameManager.instance.getIsPaused() != true && !isShooting && !isReloading) //added code so it doesnt shoot when clicking in menu
        {
            StartCoroutine(shoot());
        }

        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = !isCrouching;
            crouch();
        }

        if (Input.GetButton("Reload") && !isReloading && ammo != magSize)
        {
            StartCoroutine(reload());
        }

        if (Input.GetButtonDown("FlashLight"))
        {
            isFlashlight = !isFlashlight;
            flashLight.SetActive(isFlashlight);
        }

        if (controller.isGrounded && isMoving())
        {
            if (isCrouching && !isSliding)
                soundManager.PlayCrouch();
            else if (!isCrouching)
                soundManager.StopCrouch();

            if (isMoving() && !isSprinting)
                soundManager.PlayWalking();
            else if (!isMoving())
                soundManager.StopWalking();

            if (isSprinting && !isCrouching && !isSliding)
                soundManager.PlayRun();
            else if (!isSprinting)
                soundManager.StopRun();
        }
    }

    IEnumerator Slide()
    {
        if (canSlide && holdingSprintTime >= slideDelay)
        {
            //slide speed
            soundManager.playSliding();
            controller.Move(moveDir * (speed * slideSpeedMod) * Time.deltaTime);
            //slide duration
            yield return new WaitForSeconds(slideDistance);
            isCrouching = false;
            isSliding = false;
            //calls crouch to bring player back to normal size
            crouch();
            holdingSprintTime = 0;
            soundManager.stopSliding();
        }
    }

    IEnumerator slidingDelay()
    {
        canSlide = false;
        yield return new WaitForSeconds(slideDelay);
        canSlide = true;
    }

    void sprint()
    {
        if (cantSprint)
        {
            return;
        }
        else {
            StartCoroutine(slidingDelay());


            if (Input.GetButtonDown("Sprint"))
            {
                startTimer = Time.time; //Timer for slide delays
                speed *= sprintMod;
                isSprinting = true;
            }
            else if (Input.GetButtonUp("Sprint"))
            {
                // speed /= sprintMod;
                speed = originalSpeed;
                isSprinting = false;
                holdingSprintTime = 0;
            }
        }

    }

    void crouch()
    {

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
        else if (!isCrouching)
        {
            //changes the player Y size to the normal size
            controller.height = normalHeight;
            crouching = false;
        }
    }

    IEnumerator shoot()
    {
        StartCoroutine(gameManager.instance.MuzzleFlash());
        gameManager.instance.getSound().Play();

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


                Instantiate(hitEffect, hit.point, Quaternion.identity);

                if (dmg != null)
                {
                    dmg.takeDamage(shootDamage, Vector3.zero);
                    StartCoroutine(gameManager.instance.ActivateDeactivateHitMarker());
                }

                if (damage != null)
                {
                    damage.takeDamage(shootDamage, Vector3.zero);
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

    public void takeDamage(int amount, Vector3 dir)
    {
        if (!isSpawnProtection)
        {
            HealthPoints -= amount;
            pushDirection = dir; 
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

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HealthPoints / maxHP;
    }

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

    void StartWallRun()
    {
        jumpCount = 0;
        soundManager.PlayWallRun();
        //Resets fall speed when player first starts wall running
        if (!isWallRunning)
        {
            playerVel = Vector3.zero;
        }

        gravity = wallRunGrav;
        isWallRunning = true;
    }

    void StopWallRun()
    {
        isWallRunning = false;
        gravity = origGrav;
        soundManager.stopWallRun();
    }

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
            soundManager.PlayBounce();
        }
    }

    void CheckForMud()
    {
        isMud = Physics.Raycast(transform.position, -orientation.up, 1.7f, mud); //A raycast to detect mud

        if (isMud) //if there is mud then slow down player and remove the ability to sprint
        {
            cantSprint = true;
            isSprinting = false;
            speed = originalSpeed / mudSpeedMod;
        }else if(!isMud && !isSlowedByButter && speed == originalSpeed / mudSpeedMod) //if no mud then reset player speed and grant the ability back to sprint
        {
            speed = originalSpeed;
            isSprinting = false;
            cantSprint = false;
        }
    }

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
            }else if((ammo + ammoremaining) > magSize) //if the ammo left is greater than the mag size then add how much it takes to fill the mag and subtract that number from the remaing ammo
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
    IEnumerator healDelay()
    {
        yield return new WaitForSeconds(healDelayTime); //Waits before calling the healplayer function
        isTakingDamage = false;
        StartCoroutine(healPlayer());
    }

     IEnumerator ButterSlow()
    {
        //Sets the speed to the slowed down speed, starts the timer, marks the player as no longer buttered, and then gives the player their speed back
        cantSprint = true;
        isSlowedByButter = true;
        isSprinting = false;
        speed = gameManager.instance.getPlayerSpeed();
        yield return new WaitForSeconds(butterSlowTimer);
        gameManager.instance.setIsButtered(false);
        gameManager.instance.setPlayerSpeed(originalSpeed);
        isSlowedByButter = false;
        cantSprint = false;
    }
}

