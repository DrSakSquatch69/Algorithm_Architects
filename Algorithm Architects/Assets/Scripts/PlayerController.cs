using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    public static PlayerController instance;

    bool isGrounded;
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
    bool isAirborne;

    //Fields for shooting
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int magSize;
    [SerializeField] List<GameObject> gunList = new List<GameObject>();
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject meleeModel;
    [SerializeField] GameObject muzzleFlash;
    public ParticleSystem hitEffect;
    public AudioSource audioSource;
    private GameObject currentGun;
    private gunStats currentGunStats;
    private int gunListIndexCurr;

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
    bool canMelee;
    bool inMotion;

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
        canMelee = true;
        gameManager.instance.setOriginalPlayerSpeed(speed);
        gameManager.instance.setPlayerSpeed(speed);
        gameManager.instance.setSound(audioSource);


        updatePlayerUI();
        isSpawnProtection = true;
        StartCoroutine(spawnProtection());

    }

    IEnumerator spawnProtection()
    {
        yield return new WaitForSeconds(1);
        isSpawnProtection = false;
    }

    void Update()
    {
        if (isGrounded)
        {
            if (isAirborne)
            {
                soundManager.PlayLanding(isGrounded);
                isAirborne = false;
            }
        }
        else
        {
            isAirborne = true;
            soundManager.PlayLanding(isGrounded);
        }


        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);


        Movement();
        sprint();
        CheckForWall();
        WallRunInput();
        CheckForBouncePad();
        CheckForMud();
        isMoving();
        CheckForGround();
        RayTextUpdate();
        selectGun();

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

        if (Input.GetButton("Fire1") && gameManager.instance.getIsPaused() != true && !isShooting && !isReloading && gunList.Count != 0) //added code so it doesnt shoot when clicking in menu
        {
            if (currentGunStats.isMelee)
            {
                StartCoroutine(meleeHit());
            }
            else
            {
                StartCoroutine(shoot());
            }
        }

        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = !isCrouching;
            crouch();
        }

        if (Input.GetButton("Reload") && !isReloading && currentGunStats.ammo != currentGunStats.magSize)
        {
            StartCoroutine(reload());
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
    public void SwitchGun(GameObject newGun)
    {
        foreach (GameObject gun in gunList)
        {
            gun.SetActive(false);
        }
        newGun.SetActive(true);
        currentGun = newGun;
        UpdateGunStats(newGun.GetComponent<gun>().Gun);
    }
    public void AddGunToInventory(GameObject gun)
    {
        gunList.Add(gun);
    }

    public void UpdateGunStats(gunStats gunStats)
    {
        currentGunStats = gunStats;
        getGunStats(gunStats);
    }
    void selectGun()
    {
        if (gunList.Count > 0)
        {
            if (Input.GetButtonUp("Next Weapon") && gunListIndexCurr < gunList.Count - 1) { gunListIndexCurr++; }
            else if (Input.GetButtonUp("Next Weapon") && gunListIndexCurr >= gunList.Count -1) { gunListIndexCurr = 0; }
            foreach (GameObject gun in gunList) { gun.SetActive(false); }
            gunList[gunListIndexCurr].SetActive(true);
            currentGun = gunList[gunListIndexCurr];
            UpdateGunStats(currentGun.GetComponent<gun>().Gun);
        }
    }

    public void getGunStats(gunStats gun)
    {
        shootDamage = gun.shootDamage;
        shootRate = gun.shootRate;
        shootDist = gun.shootDist;
        magSize = gun.magSize;

        //    gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        //    gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }
    //void changeGun()
    //{
    //    shootDamage = gunList[selectGunPos].shootDamage;
    //    shootRate = gunList[selectGunPos].shootRate;
    //    shootDist = gunList[selectGunPos].shootDist;

    //    if (gunList[selectGunPos].isMelee)
    //    {
    //        gameManager.instance.turnOnOffAmmoText.SetActive(false);
    //        gunModel.SetActive(false);
    //        meleeModel.SetActive(true);
    //        meleeModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectGunPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
    //        meleeModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectGunPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    //    }
    //    else
    //    {
    //        gameManager.instance.turnOnOffAmmoText.SetActive(true);
    //        meleeModel.SetActive(false);
    //        gunModel.SetActive(true);
    //        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectGunPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
    //        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectGunPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    //    }
    //    updatePlayerUI();
    //}

    IEnumerator muzzleFlashOnOff()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.SetActive(false);
    }

    IEnumerator shoot()
    {
        if (currentGun.GetComponent<gun>().Gun.ammo > 0)
        {
            isShooting = true;

            StartCoroutine(muzzleFlashOnOff());
            PlayerSoundManager.Instance.playShootSound(currentGun.GetComponent<gun>().Gun.shootSound);
            currentGun.GetComponent<gun>().Gun.ammo--;
            updatePlayerUI();

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
            {
                // Debug.Log(hit.collider.name);

                IDamage dmg = hit.collider.GetComponent<IDamage>();
                DestroyableBullet damage = hit.collider.GetComponent<DestroyableBullet>();

                if (currentGun.GetComponent<gun>().Gun.hitEffect != null)
                {
                    Instantiate(currentGun.GetComponent<gun>().Gun.hitEffect, hit.point, Quaternion.identity);
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
        if (!isSpawnProtection)
        {
            if (type == damageType.bullet) { soundManager.PlayBulletDMG(); }
            else if (type == damageType.chaser) { soundManager.PlayChaserDMG(); }
            else if (type == damageType.melee) { soundManager.PlayMeleeDMG(); }
            else if (type == damageType.butter) { soundManager.PlayButterDMG(); }
            else if (type == damageType.stationary) { soundManager.PlayStationaryDMG(); }

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

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HealthPoints / maxHP;
        if (currentGun != null)
        {
            gameManager.instance.UpdateAmmoCounter(currentGun.GetComponent<gun>().Gun.ammo, currentGun.GetComponent<gun>().Gun.ammoremaining);
        }
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
        if (currentGun.GetComponent<gun>().Gun.ammoremaining <= 0) //if no more remaining ammo, then let player know that they have no ammo
        {
            isReloading = true;
            gameManager.instance.NoAmmoOnOff();
            yield return new WaitForSeconds(0.5f);
            isReloading = false;
            gameManager.instance.NoAmmoOnOff();

        }
        else if (!isReloading && currentGun.GetComponent<gun>().Gun.ammo <= 0 && currentGun.GetComponent<gun>().Gun.ammoremaining >= currentGun.GetComponent<gun>().Gun.magSize) //if ammo is zero or less than and ammo remaining is more than mag size, then set ammo to magsize, and subtract magsize from ammo remaining
        {
            isReloading = true;
            gameManager.instance.reloadingOnOff();
            soundManager.PlayReload();
            yield return new WaitForSeconds(0.5f);
            currentGun.GetComponent<gun>().Gun.ammo = currentGun.GetComponent<gun>().Gun.magSize;
            currentGun.GetComponent<gun>().Gun.ammoremaining -= currentGun.GetComponent<gun>().Gun.magSize;
            isReloading = false;
            gameManager.instance.reloadingOnOff();
        }
        else if (!isReloading && currentGun.GetComponent<gun>().Gun.ammo >= 0 && currentGun.GetComponent<gun>().Gun.ammoremaining >= currentGun.GetComponent<gun>().Gun.magSize) //if there is still ammo in the mag then subtact that number to the magsize and use that difference to take away remaining ammo
        {
            isReloading = true;
            gameManager.instance.reloadingOnOff();
            soundManager.PlayReload();
            yield return new WaitForSeconds(0.5f);
            currentGun.GetComponent<gun>().Gun.ammoremaining -= currentGun.GetComponent<gun>().Gun.magSize - currentGun.GetComponent<gun>().Gun.ammo;
            currentGun.GetComponent<gun>().Gun.ammo += currentGun.GetComponent<gun>().Gun.magSize - currentGun.GetComponent<gun>().Gun.ammo;
            isReloading = false;
            gameManager.instance.reloadingOnOff();
        }
        else if (!isReloading && currentGun.GetComponent<gun>().Gun.ammo >= 0 && currentGun.GetComponent<gun>().Gun.ammoremaining <= currentGun.GetComponent<gun>().Gun.magSize) //checks if ammo left is greater than zero and the remaining ammo is less the a mag
        {
            if ((currentGun.GetComponent<gun>().Gun.ammo + currentGun.GetComponent<gun>().Gun.ammoremaining) <= currentGun.GetComponent<gun>().Gun.magSize) //if the ammo left is equal to the mag size then add them together
            {
                isReloading = true;
                gameManager.instance.reloadingOnOff();
                soundManager.PlayReload();
                yield return new WaitForSeconds(0.5f);
                currentGun.GetComponent<gun>().Gun.ammo += currentGun.GetComponent<gun>().Gun.ammoremaining;
                currentGun.GetComponent<gun>().Gun.ammoremaining -= currentGun.GetComponent<gun>().Gun.ammoremaining;
                isReloading = false;
                gameManager.instance.reloadingOnOff();
            }
            else if ((currentGun.GetComponent<gun>().Gun.ammo + currentGun.GetComponent<gun>().Gun.ammoremaining) > currentGun.GetComponent<gun>().Gun.magSize) //if the ammo left is greater than the mag size then add how much it takes to fill the mag and subtract that number from the remaing ammo
            {
                isReloading = true;
                gameManager.instance.reloadingOnOff();
                soundManager.PlayReload();
                yield return new WaitForSeconds(0.5f);
                currentGun.GetComponent<gun>().Gun.ammoremaining -= currentGun.GetComponent<gun>().Gun.magSize - currentGun.GetComponent<gun>().Gun.ammo;
                currentGun.GetComponent<gun>().Gun.ammo += currentGun.GetComponent<gun>().Gun.magSize - currentGun.GetComponent<gun>().Gun.ammo;
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

        PlayerSoundManager.Instance.playShootSound(currentGun.GetComponent<gun>().Gun.shootSound);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {
            // Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            DestroyableBullet damage = hit.collider.GetComponent<DestroyableBullet>();


            if (currentGun.GetComponent<gun>().Gun.hitEffect != null)
            {
                Instantiate(currentGun.GetComponent<gun>().Gun.hitEffect, hit.point, Quaternion.identity);
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
}

