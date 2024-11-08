using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    public static PlayerController instance;

    bool isGrounded;
    [SerializeField] CharacterController controller;
    public PlayerSoundManager soundManager;
    [SerializeField] LayerMask ignoreMask;
    //Field for Health
    public int HealthPoints;
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
    bool isAirborne;

    [SerializeField] int fireDotDamage;
    [SerializeField] int bleedDotDamage;
    [SerializeField] float fireDotTimer;
    [SerializeField] int bleedDotTimer;
    [SerializeField] int fireDotRate;
    [SerializeField] int bleedDotRate;
    int fireDotTracker;
    int bleedDotTracker;

    //Fields for shooting
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int magSize;

    //Fields for weapons
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject meleeModel;
    [SerializeField] GameObject muzzleFlash;
    public ParticleSystem hitEffect;
    public AudioSource audioSource;
    int selectedGunPos;

    //Fields for Toxic Gas
    [SerializeField] float toxicGasDuration = 5f;
    [SerializeField] float poisonDamageInterval = 1f;
    [SerializeField] int poisonDamage = 2;
    [SerializeField] float visionBlurIntensity = 0.5f;
    public bool isInToxicGas;
    float toxicGasEndTime;
    
    //Value must be below the normal size for it to be a crouch
    [SerializeField] float crouchSizeYAxis;
    [SerializeField] float slideDistance;
    [SerializeField] float slideSpeedMod;
    [SerializeField] float slideDelay;
    [SerializeField] float crouchHeight;
    float normalHeight;

    bool isReloading;

    [SerializeField] float healDelayTime; //how long the player needs to not take damage
    [SerializeField] float healRate; //the healrate of the player
    bool isTakingDamage; //checks if the player is taking damage

    [SerializeField] int pushTimer;
    Vector3 moveDir;
    Vector3 playerVel;
    Vector3 pushDirection;

    [SerializeField] int rayTextDist;

    int jumpCount;
    float startTimer;
    float holdingSprintTime;

    bool isSprinting;
    bool isShooting;
    bool isCrouching;
    bool isSliding;
    bool canSlide;
    bool crouching;
    bool inMotion;
    public bool isAmmoPickup;

    //bouncepad fields
    public LayerMask bouncePad;
    public LayerMask enemy;
    public LayerMask toxicCloud;
    [SerializeField] float bouncePadForce;
    bool isBouncePad;

    //Mud fields
    public LayerMask mud;
    [SerializeField] float mudSpeedMod;
    bool isMud;

    public LayerMask whatIsWall;
    bool isWallRight, isWallLeft;
    public Transform playerCam;
    [SerializeField] Transform orientation;
    [SerializeField] int wallRunGrav;
    [SerializeField] int wallRunSpeed;
    public float wallRunCameraTilt, maxWallRunCameraTilt;
    Vector3 rot;
    float desiredX;
    int origGrav;
    bool isWallRunning;
    bool isSpawnProtection;

    //used to see if the player is grounded in debug mode
    
    //stores the normal Y size of the player capsule
    float normYSize;

    // Start is called before the first frame update

    public PlayerSoundManager GetSoundManager() { return soundManager; }
    void Start()
    {
        instance = this;
        gameManager.instance.setPlayerScript(instance);

        //initiates the normal size
        normYSize = transform.localScale.y;
        jumpCount = 0;
        origGrav = gravity;
        HealthPoints = maxHP;
        normalHeight = controller.height;
        originalSpeed = speed;
        gameManager.instance.setOriginalPlayerSpeed(speed);
        gameManager.instance.setPlayerSpeed(speed);
        gameManager.instance.setSound(audioSource);


        updatePlayerUI();
        isSpawnProtection = true;
        StartCoroutine(spawnProtection()); 

        if(MainManager.Instance.GetGunList() != null)
        {
            LoadSetting();
        }
    }

    IEnumerator spawnProtection()
    {
        yield return new WaitForSeconds(1);
        isSpawnProtection = false;
    }

    void Update()
    {
        if (isGrounded || controller.isGrounded)
        {
            if (isAirborne && !isBouncePad)
            {
                soundManager.PlayLanding(isGrounded);
                isAirborne = false;
            }
        }
        else
        {
            isAirborne = true;
            soundManager.StopLanding();
        }


        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        rot = playerCam.transform.localRotation.eulerAngles;

        if (gameManager.instance.cameraController != null)
        {
            desiredX = rot.y + gameManager.instance.cameraController.mouseX;
        }

        if (!gameManager.instance.isPaused)
        {
        Movement();
        selectGun();
        }
        sprint();
        CheckForWall();
        WallRunInput();
        CheckForBouncePad();
        CheckForMud();
        isMoving();
        CheckForGround();
        RayTextUpdate();
        PickupAmmo();

        if (inMotion && isGrounded)
        {
            if (isSprinting && !soundManager.runningPlaying())
            {
                soundManager.PlayRun();
                //  Debug.Log("Run is playing");
            }
            else if (isCrouching && !soundManager.crouchedPlaying())
            {
                soundManager.PlayCrouch();
                // Debug.Log("Crouch is playing");
            }
            else if (!isSprinting && soundManager.runningPlaying())
            {
                soundManager.StopRun();
                //  Debug.Log("run stopped playing");
            }
            else if (!isCrouching && soundManager.crouchedPlaying())
            {
                soundManager.StopCrouch();
                //Debug.Log("crouch stopped playing");
            }
            else if (!isSprinting && !isCrouching && !soundManager.walkingPlaying())
            {
                soundManager.PlayWalking();
                //Debug.Log("walking replayed");
            }
        }
        else if (!inMotion || !isGrounded)
        {
            if (soundManager.runningPlaying() || soundManager.crouchedPlaying() || soundManager.walkingPlaying())
            {
                soundManager.StopRun();
                soundManager.StopWalking();
                soundManager.StopCrouch();
            }
        }


    }

    void isMoving()
    {
        if (moveDir != Vector3.zero)
            inMotion = true;
        else if (moveDir == Vector3.zero)
            inMotion = false;
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
        if (crouching && !isSliding)
        {
            controller.Move(moveDir * (speed / 3) * Time.deltaTime);
        }
        else if (isWallRunning)
        {
            controller.Move(moveDir * wallRunSpeed * Time.deltaTime);

            if (isWallRight)
            {
                if (wallRunCameraTilt < maxWallRunCameraTilt)
                {
                    wallRunCameraTilt += Math.Abs(Time.deltaTime * maxWallRunCameraTilt * 2);
                }
                playerCam.transform.localRotation = Quaternion.Euler(0, 0, wallRunCameraTilt);
            }
            else if (isWallLeft)
            {
                if (wallRunCameraTilt > -maxWallRunCameraTilt)
                {
                    wallRunCameraTilt -= Math.Abs(Time.deltaTime * maxWallRunCameraTilt * 2);
                }
                playerCam.transform.localRotation = Quaternion.Euler(0, 0, wallRunCameraTilt);
            }
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
            if (jumpCount == 0)
                soundManager.PlayFirstJump();
            else
                soundManager.PlayDoubleJump();

            jumpCount++;
            playerVel.y = jumpSpeed;

        }

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move((playerVel + pushDirection) * Time.deltaTime);

        if (Input.GetButton("Fire1") && !gameManager.instance.getIsPaused() && !isShooting && !isReloading && gunList.Count != 0) //added code so it doesnt shoot when clicking in menu
        {
            if (gunList[selectedGunPos].isMelee)
            {
                StartCoroutine(meleeHit());
            }
            else
            {
                StartCoroutine(shoot());
            }
        }

        if (Input.GetButtonDown("Crouch") && (isGrounded || controller.isGrounded))
        {
            isCrouching = !isCrouching;
            crouch();
        }

        if (gunList != null && gunList.Count > 0)
        {
            if (Input.GetButton("Reload") && !isReloading && gunList[selectedGunPos].ammo != gunList[selectedGunPos].magSize)
            {
                StartCoroutine(reload());
            }
        }

        if (Input.GetButtonDown("FlashLight"))
        {

            isFlashlight = !isFlashlight;
            if (isFlashlight) { soundManager.PlayFlashlightOn(); }
            else { soundManager.PlayFlashlightOff(); }
            flashLight.SetActive(isFlashlight);
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
        else
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
    
    IEnumerator muzzleFlashOnOff()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.SetActive(false);
    }

    IEnumerator shoot()
    {
        if (gunList[selectedGunPos].ammo > 0)
        {
            isShooting = true;

            StartCoroutine(muzzleFlashOnOff());
            PlayerSoundManager.Instance.playShootSound(gunList[selectedGunPos].shootSound);
            gunList[selectedGunPos].ammo--;
            updatePlayerUI();

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
            {
                // Debug.Log(hit.collider.name);

                IDamage dmg = hit.collider.GetComponent<IDamage>();
                DestroyableBullet damage = hit.collider.GetComponent<DestroyableBullet>();

                if (gunList[selectedGunPos].hitEffect != null)
                {
                    Instantiate(gunList[selectedGunPos].hitEffect, hit.point, Quaternion.identity);
                }

                if (dmg != null)
                {
                    dmg.takeDamage(shootDamage, Vector3.zero, damageType.bullet);
                    StartCoroutine(gameManager.instance.ActivateDeactivateHitMarker());
                }

                //This is for detroying the chaser bullet
                if (damage != null)
                {
                    damage.takeDamage(shootDamage, Vector3.zero, damageType.bullet);
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

    public void takeDamage(int amount, Vector3 dir, damageType type)
    {

        if (!isSpawnProtection && type != damageType.fire)
        {
            if (type == damageType.bullet) { soundManager.PlayBulletDMG(); }
            else if (type == damageType.chaser) { soundManager.PlayChaserDMG(); }
            else if (type == damageType.melee) { soundManager.PlayMeleeDMG(); }
            else if (type == damageType.butter) { soundManager.PlayButterDMG(); }
            else if (type == damageType.stationary) { soundManager.PlayStationaryDMG(); }
            else if (type == damageType.toxic) { if (!isInToxicGas) { EnterToxicGas(); } StartCoroutine(ToxicGasDoT()); }

            HealthPoints -= amount;
            pushDirection = dir;
            StartCoroutine(gameManager.instance.hitFlash());
            updatePlayerUI();
            isTakingDamage = true;
            StartCoroutine(healDelay());

            if (HealthPoints <= 0)
            {
                soundManager.PlayDeathSound();
                gameManager.instance.youLose();
            }
        }
        else
        {
            return;
        }
    }

    public void DoT()
    {
        //Debug.Log("DOT CALLED");
        //If player has no spawn protection and is set on fire from a fire bullet then call the coroutine that does the damage
        if (!isSpawnProtection && gameManager.instance.getIsOnFire())
        {
            StartCoroutine(FireDoT());
        }

        else if(!isSpawnProtection && gameManager.instance.getIsCabbaged())
        {
            StartCoroutine(BleedDoT());
        }
    }

    IEnumerator FireDoT()
    {
        //While the player has not taken the proper amount of damage
        while (fireDotTracker < fireDotRate)
        {
            //Makes sure the player does not die from the damage over time and makes the player unable to be set on fire again if at 1 HP
            if (HealthPoints < fireDotDamage)
            {
                HealthPoints = 1;
                yield break;
            }

            HealthPoints -= fireDotDamage;
            StartCoroutine(gameManager.instance.hitFlash());
            updatePlayerUI();
            isTakingDamage = true;
            ++fireDotTracker;

            //Waits until the timer is up to do another instance of damage
            yield return new WaitForSeconds(fireDotTimer);
        }

        //When the proper amount of damage has been done then the player is no longer on fire and the tracker is reset
        if (fireDotTracker >= fireDotRate || HealthPoints == 1)
        {
            gameManager.instance.setIsOnFire(false);
            fireDotTracker = 0;
            StartCoroutine(healDelay());
        }
    }

    IEnumerator BleedDoT()
    {
        while(bleedDotTracker < bleedDotRate)
        {
            if(HealthPoints < bleedDotDamage)
            {
                HealthPoints = 1;
                yield break;
            }

            HealthPoints -= bleedDotDamage;
            StartCoroutine (gameManager.instance.hitFlash());
            updatePlayerUI();
            isTakingDamage = true;
            ++bleedDotTracker;

            yield return new WaitForSeconds(bleedDotTimer);
        }

        if (bleedDotTracker >= bleedDotRate || HealthPoints == 1)
        {
            gameManager.instance.setIsCabbaged(false);
            bleedDotTracker = 0;
            StartCoroutine(healDelay());
            
        }
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HealthPoints / maxHP;
        if (gunList.Count != 0)
        {
            gameManager.instance.UpdateAmmoCounter(gunList[selectedGunPos].ammo, gunList[selectedGunPos].ammoremaining);
        }
    }

    void WallRunInput()
    {
        if (isWallRight && !isCrouching && (!controller.isGrounded && !isGrounded))
        {

            StartWallRun();
        }
        if (isWallLeft && !isCrouching && (!controller.isGrounded && !isGrounded))
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

            if (wallRunCameraTilt > 0)
            {
                wallRunCameraTilt -= Math.Abs(Time.deltaTime * maxWallRunCameraTilt * 2);
                playerCam.transform.localRotation = Quaternion.Euler(0, 0, wallRunCameraTilt);
            }
            if (wallRunCameraTilt < 0)
            {
                wallRunCameraTilt += Math.Abs(Time.deltaTime * maxWallRunCameraTilt * 2);
                playerCam.transform.localRotation = Quaternion.Euler(0, 0, wallRunCameraTilt);
            }
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
            soundManager.StopLanding();
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
        }
        else if (!isMud && !isSlowedByButter && speed == originalSpeed / mudSpeedMod) //if no mud then reset player speed and grant the ability back to sprint
        {
            speed = originalSpeed;
            isSprinting = false;
            cantSprint = false;
        }
    }

    void CheckForGround()
    {
        isGrounded = Physics.Raycast(transform.position, -orientation.up, 1.2f); //A raycast to detect mud

    }

    IEnumerator reload()
    {
        if (gunList[selectedGunPos].ammoremaining <= 0) //if no more remaining ammo, then let player know that they have no ammo
        {
            isReloading = true;
            gameManager.instance.NoAmmoOnOff();
            yield return new WaitForSeconds(0.5f);
            isReloading = false;
            gameManager.instance.NoAmmoOnOff();

        }
        else if (!isReloading && gunList[selectedGunPos].ammo <= 0 && gunList[selectedGunPos].ammoremaining >= gunList[selectedGunPos].magSize) //if ammo is zero or less than and ammo remaining is more than mag size, then set ammo to magsize, and subtract magsize from ammo remaining
        {
            isReloading = true;
            gameManager.instance.reloadingOnOff();
            soundManager.PlayReload();
            yield return new WaitForSeconds(0.5f);
            gunList[selectedGunPos].ammo = gunList[selectedGunPos].magSize;
            gunList[selectedGunPos].ammoremaining -= gunList[selectedGunPos].magSize;
            isReloading = false;
            gameManager.instance.reloadingOnOff();
        }
        else if (!isReloading && gunList[selectedGunPos].ammo >= 0 && gunList[selectedGunPos].ammoremaining >= gunList[selectedGunPos].magSize) //if there is still ammo in the mag then subtact that number to the magsize and use that difference to take away remaining ammo
        {
            isReloading = true;
            gameManager.instance.reloadingOnOff();
            soundManager.PlayReload();
            yield return new WaitForSeconds(0.5f);
            gunList[selectedGunPos].ammoremaining -= gunList[selectedGunPos].magSize - gunList[selectedGunPos].ammo;
            gunList[selectedGunPos].ammo += gunList[selectedGunPos].magSize - gunList[selectedGunPos].ammo;
            isReloading = false;
            gameManager.instance.reloadingOnOff();
        }
        else if (!isReloading && gunList[selectedGunPos].ammo >= 0 && gunList[selectedGunPos].ammoremaining <= gunList[selectedGunPos].magSize) //checks if ammo left is greater than zero and the remaining ammo is less the a mag
        {
            if ((gunList[selectedGunPos].ammo + gunList[selectedGunPos].ammoremaining) <= gunList[selectedGunPos].magSize) //if the ammo left is equal to the mag size then add them together
            {
                isReloading = true;
                gameManager.instance.reloadingOnOff();
                soundManager.PlayReload();
                yield return new WaitForSeconds(0.5f);
                gunList[selectedGunPos].ammo += gunList[selectedGunPos].ammoremaining;
                gunList[selectedGunPos].ammoremaining -= gunList[selectedGunPos].ammoremaining;
                isReloading = false;
                gameManager.instance.reloadingOnOff();
            }
            else if ((gunList[selectedGunPos].ammo + gunList[selectedGunPos].ammoremaining) > gunList[selectedGunPos].magSize) //if the ammo left is greater than the mag size then add how much it takes to fill the mag and subtract that number from the remaing ammo
            {
                isReloading = true;
                gameManager.instance.reloadingOnOff();
                soundManager.PlayReload();
                yield return new WaitForSeconds(0.5f);
                gunList[selectedGunPos].ammoremaining -= gunList[selectedGunPos].magSize - gunList[selectedGunPos].ammo;
                gunList[selectedGunPos].ammo += gunList[selectedGunPos].magSize - gunList[selectedGunPos].ammo;
                isReloading = false;
                gameManager.instance.reloadingOnOff();
            }
        }
        updatePlayerUI();
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

    IEnumerator meleeHit()
    {
        isShooting = true;

        PlayerSoundManager.Instance.playShootSound(gunList[selectedGunPos].shootSound);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {
            // Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            DestroyableBullet damage = hit.collider.GetComponent<DestroyableBullet>();


            if (gunList[selectedGunPos].hitEffect != null)
            {
                Instantiate(gunList[selectedGunPos].hitEffect, hit.point, Quaternion.identity);
            }

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage, Vector3.zero, damageType.bullet);
                StartCoroutine(gameManager.instance.ActivateDeactivateHitMarker());
            }

            //This is for detroying the chaser bullet
            if (damage != null)
            {
                damage.takeDamage(shootDamage, Vector3.zero, damageType.bullet);
                StartCoroutine(gameManager.instance.ActivateDeactivateHitMarker());
            }
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void RayTextUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, rayTextDist, bouncePad))
        {
            gameManager.instance.rayText.enabled = true;
            gameManager.instance.rayText.text = "Bounce Pad";
        }
        else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, rayTextDist, enemy))
        {
            gameManager.instance.rayText.enabled = true;
            gameManager.instance.rayText.text = "Enemy";
        }
        else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, rayTextDist, toxicCloud))
        {
            gameManager.instance.rayText.enabled = true;
            gameManager.instance.rayText.text = "Toxic Cloud";
        }
        else
        {
            gameManager.instance.rayText.text = "";
            gameManager.instance.rayText.enabled = false;
        }
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        selectedGunPos = gunList.Count - 1;

        changeGun();
            
        //MainManager.Instance.SetGunList(gunList);
        //gunModel.transform.position += gun.placement;
        //gunModel.transform.eulerAngles += gun.rotation;
    }

    void selectGun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGunPos < gunList.Count - 1)
        {
            selectedGunPos++;
            changeGun();
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGunPos > 0)
        {
            selectedGunPos--;
            changeGun();
        }
    }

    void changeGun()
    {
        shootDamage = gunList[selectedGunPos].shootDamage;
        shootDist = gunList[selectedGunPos].shootDist;
        shootRate = gunList[selectedGunPos].shootRate;
        magSize = gunList[selectedGunPos].magSize;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGunPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGunPos].gunModel.GetComponent <MeshRenderer>().sharedMaterial;

        updatePlayerUI();
        MainManager.Instance.SetSelectedGunPos(selectedGunPos);
        //gunModel.transform.position = gunList[selectedGunPos].placement;
        //gunModel.transform.eulerAngles = gunList[selectedGunPos].rotation;
    }


    void LoadSetting()
    {
        if (MainManager.Instance.GetGunList().Count != 0)
        { 
            gunList = MainManager.Instance.GetGunList();
            selectedGunPos = MainManager.Instance.GetSelectedGunPOS();
            Debug.Log("Settings Loaded for Player");
            changeGun();
        }
    }

    public void EnterToxicGas()
    {
        isInToxicGas = true;
        toxicGasEndTime = Time.time + toxicGasDuration;
        StartCoroutine(ApplyToxicGasEffects());
    }

    public void ExitToxicGas()
    {
        isInToxicGas = false;
        // Reset blur effect
        gameManager.instance.DisableBlur();
        StopCoroutine(ApplyToxicGasEffects());
    }

    IEnumerator ApplyToxicGasEffects()
    {
        while (isInToxicGas && Time.time < toxicGasEndTime)
        {
            HealthPoints -= poisonDamage;
            gameManager.instance.EnableBlur(visionBlurIntensity); // Apply blur while in toxic gas
            updatePlayerUI();
            yield return new WaitForSeconds(poisonDamageInterval);
        }
        ExitToxicGas(); // Exit when the effect is over
    }

    public IEnumerator ToxicGasDoT()
    {
        while (isInToxicGas)
        {
            HealthPoints -= 1; // Apply poison damage over time
            updatePlayerUI();
            yield return new WaitForSeconds(1f); // Adjust interval as needed
            if (HealthPoints <= 0)
            {
                soundManager.PlayDeathSound();
                gameManager.instance.youLose();
                yield break; // Stops the coroutine if the player dies
            }
        }
    }

    void PickupAmmo()
    {
        if (isAmmoPickup && gunList.Count != 0)
        {
            gunList[selectedGunPos].ammoremaining += gunList[selectedGunPos].magSize * 2;
            updatePlayerUI();
        }
        isAmmoPickup = false;
    }
}

