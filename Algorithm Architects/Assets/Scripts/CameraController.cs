using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;

    // Start is called before the first frame update
    void Start()
    {
        //Locks the cursor 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameManager.instance.setSens(sens);
    }

    // Update is called once per frame
    void Update()
    {
        sens = gameManager.instance.getSens(); //Gets the sens set from the settings menu

        // get input
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;

        // invert Y camera
        if (!invertY)
            rotX -= mouseY;
        else
            rotX += mouseX;

        // clamp the rotX on the x-axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // rotate the camera on the x-axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // rotate the player on the y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}

//override