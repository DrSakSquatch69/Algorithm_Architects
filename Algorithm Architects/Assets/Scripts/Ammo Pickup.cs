using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    bool itemIsPickedUp;
    float yPOS;
    bool floatUp;

    private void Start()
    {
        floatUp = true;
    }

    void Update()
    {
        transform.Rotate(0, 0.5f, 0);

        if ((floatUp))
        {
            StartCoroutine(floatingUp());
        }else if (!floatUp)
        {
            StartCoroutine(floatingDown());
        }

    }

    IEnumerator floatingUp()
    {
        transform.Translate(Vector3.up * 0.3f * Time.deltaTime);
        yield return new WaitForSeconds(2);
        floatUp = false;
    }

    IEnumerator floatingDown()
    {
        transform.Translate(-(Vector3.up * 0.3f * Time.deltaTime));
        //transform.position.y -= 0.5 * Time.deltaTime;
        yield return new WaitForSeconds(2);
        floatUp = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !itemIsPickedUp)
        {
         
        }
    }
}
