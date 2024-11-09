using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//commment
public class pickUp : MonoBehaviour
{
    [SerializeField] gunStats gun;
    bool floatUp;

    bool itemIsPickedUp;
    private void Start()
    {
        gun.ammo = gun.magSize;
        floatUp = true;

    }

    void Update()
    {
        if (!gameManager.instance.isPaused)
        {


            transform.Rotate(0, 0.5f, 0);

            if ((floatUp))
            {
                StartCoroutine(floatingUp());
            }
            else if (!floatUp)
            {
                StartCoroutine(floatingDown());
            }
        }

    }

    IEnumerator floatingUp()
    {
        transform.Translate(Vector3.up * 0.2f * Time.deltaTime);
        yield return new WaitForSeconds(2);
        floatUp = false;
    }

    IEnumerator floatingDown()
    {
        transform.Translate(-(Vector3.up * 0.2f * Time.deltaTime));
        //transform.position.y -= 0.5 * Time.deltaTime;
        yield return new WaitForSeconds(2);
        floatUp = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !itemIsPickedUp && !gameManager.instance.isPaused)
        {
            gameManager.instance.turnOnOffInteract.SetActive(true);
            gameManager.instance.interact.text = "Press E to Pickup";
            gameManager.instance.playerScript.isInteractable = true;

            if (gameManager.instance.playerScript.isInteract)
            {
                itemIsPickedUp = true;
                gameManager.instance.playerScript.isInteract = false;
                gameManager.instance.playerScript.isInteractable = false;
                gameManager.instance.playerScript.getGunStats(gun);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !itemIsPickedUp)
        {
            gameManager.instance.turnOnOffInteract.SetActive(false);
            gameManager.instance.playerScript.isInteractable = false;
        }
    }
}
