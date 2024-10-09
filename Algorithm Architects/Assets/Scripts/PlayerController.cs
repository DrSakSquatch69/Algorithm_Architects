using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    //Field for Health
    [SerializeField] int HealthPoints;

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

    int ammo;
    [SerializeField] int ammoremaining;
    [SerializeField] int magSize;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    float startTimer;
    float holdingSprintTime;

    bool isSprinting;
    bool isShooting;
    bool isCrouching;
    bool isSliding;
    bool canSlide;
    bool crouching;

    public LayerMask bouncePad;
    [SerializeField] float bouncePadForce;
    bool isBouncePad;

    public LayerMask whatIsWall;
    bool isWallRight, isWallLeft;
    [SerializeField] Transform orientation;
    [SerializeField] int wallRunGrav;
    [SerializeField] int wallRunSpeed;
    int origGrav;
    bool isWallRunning;

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

        gameManager.instance.UpdateAmmoCounter(ammo, ammoremaining);
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
        controller.Move(playerVel * Time.deltaTime);

        if (Input.GetButton("Fire1") && gameManager.instance.getIsPaused() != true && !isShooting) //added code so it doesnt shoot when clicking in menu
        {
            StartCoroutine(shoot());
        }

        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = !isCrouching;
            crouch();
        }
    }

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

    IEnumerator slidingDelay()
    {
        canSlide = false;
        yield return new WaitForSeconds(slideDelay);
        canSlide = true;
    }

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

    void crouch()
    {

        if (isCrouching && !isSprinting && !isSliding)
        {
            //changes the player Y size to the crouch size
            transform.localScale = new Vector3(1, crouchSizeYAxis, 1);
            crouching = true;

        }
        //if sprinting then start slide
        else if (isCrouching && isSprinting && !isSliding)
        {
            holdingSprintTime = Time.time - startTimer;

            if (canSlide && holdingSprintTime >= slideDelay) //checking if the player can slide and also if they held the key for the correct amount of time
            { 
                isSliding = true;
                transform.localScale = new Vector3(1, crouchSizeYAxis, 1);
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
            transform.localScale = new Vector3(1, normYSize, 1);
            crouching = false;
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;
        ammo--;
        gameManager.instance.UpdateAmmoCounter(ammo, ammoremaining);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {
            // Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
                StartCoroutine(gameManager.instance.ActivateDeactivateHitMarker());
            }
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        HealthPoints -= amount;
        StartCoroutine(gameManager.instance.hitFlash());

        if (HealthPoints <= 0)
        {
            gameManager.instance.youLose();
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
        }
    }
}

