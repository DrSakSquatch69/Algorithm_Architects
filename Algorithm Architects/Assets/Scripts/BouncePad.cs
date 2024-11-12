using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    bool playerInRange;

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !gameManager.instance.isPaused)
        {
            gameManager.instance.playerScript.CheckForBouncePad();
            playerInRange = true;

            //Place animation code here
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
