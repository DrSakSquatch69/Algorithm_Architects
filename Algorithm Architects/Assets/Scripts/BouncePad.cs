using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    bool playerInRange;

    private void Update()
    {
        if (playerInRange)
        {
            gameManager.instance.playerScript.CheckForBouncePad();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !gameManager.instance.isPaused)
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !gameManager.instance.isPaused)
        {
            playerInRange = false;
        }
    }
}
