using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSplitBullet : MonoBehaviour
{
    public float speed = 10f;
    public float splitDistance = 5f;
    public GameObject splitBulletPrefab;
    private Vector3 startPosition;
    private bool hasSplit = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Move the bullet forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Check if the bullet has reached the split distance
        if (!hasSplit && Vector3.Distance(startPosition, transform.position) >= splitDistance)
        {
            Split();
            hasSplit = true;
        }
    }

    void Split()
    {
        // Instantiate the split bullets at 90-degree angles to the current direction
        Instantiate(splitBulletPrefab, transform.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 90, 0)));
        Instantiate(splitBulletPrefab, transform.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, -90, 0)));
    }

    void OnCollisionEnter(Collision collision)
    {
        // Destroy the bullet on collision
        Destroy(gameObject);
    }
}
