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
        if (other.CompareTag("Player") && !itemIsPickedUp)
        {
            itemIsPickedUp = true;
            gameManager.instance.playerScript.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
