using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//commment
public class PowerupPickUp : MonoBehaviour
{
    [SerializeField] float speedBoost;
    [SerializeField] int speedBoostTimer;
    [SerializeField] float protectionTime;
    [SerializeField] int jumpBoost;
    [SerializeField] int jumpBoostTimer;
    bool floatUp;
    bool inSpeedRange;
    bool inJumpRange;
    bool inProtectRange;

    [SerializeField] bool flipRotation;

    bool inRange;
    bool itemIsPickedUp;
    private void Start()
    {
       
        floatUp = true;

        if (flipRotation)
        {
            transform.eulerAngles = new Vector3(-90f, 0f, 0f);
        }

    }

    void Update()
    {
        if (!gameManager.instance.isPaused)
        {

            if (flipRotation)
            {
                transform.Rotate(0, 0, 0.5f);
            }
            else
            {
                transform.Rotate(0, 0.5f, 0);
            }

            if ((floatUp))
            {
                StartCoroutine(floatingUp());
            }
            else if (!floatUp)
            {
                StartCoroutine(floatingDown());
            }
        }

        if (gameManager.instance.playerScript.isInteract && inRange)
        {
            itemIsPickedUp = true;
            gameManager.instance.playerScript.isInteract = false;
            gameManager.instance.playerScript.isInteractable = false;
            gameManager.instance.turnOnOffInteract.SetActive(false);
        
            if(gameObject.CompareTag("Speed"))
            {
                StartCoroutine(SpeedBoost());
            }

            if(gameObject.CompareTag("Protect"))
            {

            }

            if(gameObject.CompareTag("Jump"))
            {

            }

            Destroy(gameObject);
        }

    }

    IEnumerator floatingUp()
    {
        if (flipRotation)
        {
            transform.Translate(Vector3.forward * 0.2f * Time.deltaTime);
            yield return new WaitForSeconds(2);
            floatUp = false;
        }
        else
        {
            transform.Translate(Vector3.up * 0.2f * Time.deltaTime);
            yield return new WaitForSeconds(2);
            floatUp = false;
        }
    }

    IEnumerator floatingDown()
    {
        if (flipRotation)
        {
            transform.Translate(-(Vector3.forward * 0.2f * Time.deltaTime));
            yield return new WaitForSeconds(2);
            floatUp = true;
        }
        else
        {
            transform.Translate(-(Vector3.up * 0.2f * Time.deltaTime));
            yield return new WaitForSeconds(2);
            floatUp = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !itemIsPickedUp && !gameManager.instance.isPaused)
        {
            gameManager.instance.turnOnOffInteract.SetActive(true);
            gameManager.instance.interact.text = "Press E to Pickup";
            gameManager.instance.playerScript.isInteractable = true;
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !itemIsPickedUp)
        {
            gameManager.instance.turnOnOffInteract.SetActive(false);
            gameManager.instance.playerScript.isInteractable = false;
            inRange = false;
        }
    }

    IEnumerator SpeedBoost()
    {
        gameManager.instance.setPlayerSpeed(speedBoost * gameManager.instance.getPlayerSpeed());
        yield return new WaitForSeconds(speedBoostTimer);
        gameManager.instance.setPlayerSpeed(gameManager.instance.getOriginalPlayerSpeed());
    }

    IEnumerator JumpBoost()
    {
        gameManager.instance.setPlayerJumpSpeed(jumpBoost * gameManager.instance.getPlayerJumpSpeed());
        yield return new WaitForSeconds(jumpBoostTimer);
        gameManager.instance.setPlayerJumpSpeed(gameManager.instance.getOriginalPlayerJumpSpeed());
    }
}
