using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitBullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private int damageAmount;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float travelDistance; // Distance before the bullet splits
    [SerializeField] private float despawnTimer;

    private Vector3 startPosition;

    void Start()
    {
        // Set the start position and initial velocity
        startPosition = transform.position;
        rb.velocity = transform.forward * bulletSpeed;

        // Start the coroutine for split bullet behavior
        StartCoroutine(BulletTravelAndSplit());
    }

    // Coroutine to handle the travel and splitting of the bullet
    IEnumerator BulletTravelAndSplit()
    {
        // Wait until the bullet reaches the specified travel distance
        while (Vector3.Distance(startPosition, transform.position) < travelDistance)
        {
            yield return null;
        }

        // Split the bullet into multiple directions
        Split();

        // Destroy the original bullet after splitting
        Destroy(gameObject);
    }

    // Method to split the bullet into multiple projectiles
    private void Split()
    {
        // Define the split directions
        Vector3 middleDirection = transform.forward;
        Vector3 leftDirection = Quaternion.Euler(0, -22, 0) * middleDirection;
        Vector3 leftDirection2 = Quaternion.Euler(0, -45, 0) * middleDirection;
        Vector3 rightDirection = Quaternion.Euler(0, 22, 0) * middleDirection;
        Vector3 rightDirection2 = Quaternion.Euler(0, 45, 0) * middleDirection;

        // Instantiate the split bullets
        CreateSplitBullet(middleDirection);
        CreateSplitBullet(leftDirection);
        CreateSplitBullet(leftDirection2);
        CreateSplitBullet(rightDirection);
        CreateSplitBullet(rightDirection2);
    }

    // Helper method to instantiate a split bullet
    private void CreateSplitBullet(Vector3 direction)
    {
        GameObject splitBullet = Instantiate(gameObject, transform.position, Quaternion.identity);
        Rigidbody splitRb = splitBullet.GetComponent<Rigidbody>();

        // Set the velocity and damage amount for the split bullet
        splitRb.velocity = direction * bulletSpeed;
        SplitBullet splitScript = splitBullet.GetComponent<SplitBullet>();
        splitScript.damageAmount = damageAmount;

        // Destroy the split bullet after the despawn timer
        Destroy(splitBullet, despawnTimer);
    }

    // Handle collision detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.CompareTag("Enemy")) return;

        IDamage dmg = other.GetComponent<IDamage>();

        // Apply damage if the collided object implements the IDamage interface
        if (dmg != null)
        {
            dmg.takeDamage(damageAmount, -(transform.position - other.transform.position).normalized * (damageAmount / 2), damageType.SplitBullet);
        }

        // Destroy the bullet on impact
        Destroy(gameObject);
    }
}