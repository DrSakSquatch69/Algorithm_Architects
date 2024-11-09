using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainController : MonoBehaviour
{
    public ParticleSystem rainParticleSystem; // Drag the Particle System here in the Inspector
    public float minRainInterval = 10f; // Minimum time between rains
    public float maxRainInterval = 30f; // Maximum time between rains
    public float rainDuration = 15f; // Duration of each rain event
    public float minRainIntensity = 50f; // Minimum emission rate
    public float maxRainIntensity = 1000f; // Maximum emission rate

    private ParticleSystem.EmissionModule emissionModule;

    void Start()
    {
        if (rainParticleSystem == null)
        {
            Debug.LogError("Rain Particle System is not assigned!");
            return;
        }

        // Get the Emission Module from the Particle System
        emissionModule = rainParticleSystem.emission;
        emissionModule.enabled = true; // Ensure the emission module is enabled
        rainParticleSystem.Stop();  // Makes sure the partical system is set to false on Awake
        StartCoroutine(RainCycle());
    }

    IEnumerator RainCycle()
    {
        while (true)
        {
            // Randomize the time until it rains
            float waitTime = Random.Range(minRainInterval, maxRainInterval);
            Debug.Log("Waiting for " + waitTime + " seconds before rain."); // Debug log
            yield return new WaitForSeconds(waitTime);

            // Set a random intensity for the rain
            float rainIntensity = Random.Range(minRainIntensity, maxRainIntensity);
            Debug.Log("Starting rain with intensity: " + rainIntensity); // Debug log
            StartCoroutine(FadeRainIntensity(rainIntensity, 2f)); // Fade to the new intensity over 2 seconds

            // Enable the rain
            if (!rainParticleSystem.isPlaying)
            {
                rainParticleSystem.Play();
                Debug.Log("Rain started.");
            }
            
            // Wait for the rain duration, then stop the rain
            yield return new WaitForSeconds(rainDuration);

            
            StartCoroutine(FadeRainIntensity(0f, 2f)); // Fade out the intensity over 2 seconds
            Debug.Log("Stopping rain."); // Debug log
            yield return new WaitForSeconds(2f); // Wait for fade out to complete
        }
    }

    IEnumerator FadeRainIntensity(float targetIntensity, float duration)
    {
        float startIntensity = emissionModule.rateOverTime.constant;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float currentIntensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / duration);
            emissionModule.rateOverTime = currentIntensity;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        emissionModule.rateOverTime = targetIntensity; // Ensure it sets to the target at the end
    }
}
