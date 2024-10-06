using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    //Field for Health
    [SerializeField] int HealthPoints;

    //Fields for movement
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    //Fields for shooting
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    //Value must be below the normal size for it to be a crouch
    [SerializeField] float crouchSizeYAxis;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;

    bool isSprinting;
    bool isShooting;
    bool isCrouching;

    public LayerMask whatIsWall;
    bool isWallRight, isWallLeft;
    [SerializeField] Transform orientation;
    [SerializeField] int wallRunGrav;
    [SerializeField] int wallRunSpeed;
    int origGrav;
    int origSpeed;
    bool isWallRunning;

    //stores the normal Y size of the player capsule
    float normYSize;

    // Start is called before the first frame update
    void Start()
    {
        //initiates the normal size
        normYSize = transform.localScale.y;
        jumpCount = 0;
        origGrav = gravity;
        origSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        //Used to see where the player is looking
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        movement();
        sprint();
        CheckForWall();
        WallRunInput();
    }

    void movement()
    {
        //checks if the player is on the ground, if yes then reset jump count and player velocity
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        //if the player is crouching, then dive the speed by 3, to make the player move slower
        if (isCrouching)
        {
            controller.Move(moveDir * (speed / 3) * Time.deltaTime);
        }
        else if (!isCrouching)
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

        if(Input.GetButtonDown("Crouch") && isSprinting != true)
        {
            isCrouching = !isCrouching;
            crouch();
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void crouch()
    {
        if (isCrouching)
        {
            //changes the player Y size to the crouch size
            transform.localScale =  new Vector3 (1, crouchSizeYAxis, 1);

        }else if (!isCrouching)
        {
            //changes the player Y size to the normal size
            transform.localScale = new Vector3(1, normYSize, 1);
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {
            // Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        HealthPoints -= amount;

        if(HealthPoints <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    void WallRunInput()
    {
        if(isWallRight && !isCrouching)
        {
            StartWallRun();
        }
        if (isWallLeft && !isCrouching)
        {
            StartWallRun();
        }
    }

    void StartWallRun()
    {
        jumpCount = 0;
        speed = wallRunSpeed;

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
        speed = origSpeed;
    }

    void CheckForWall()
    {
        //Uses a raycast to check for wall on left and wall on right
        isWallRight = Physics.Raycast(transform.position, orientation.right, 1f, whatIsWall);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 1f, whatIsWall);

        if(!isWallLeft && !isWallRight)
        {
            StopWallRun();
        }

    }
}
