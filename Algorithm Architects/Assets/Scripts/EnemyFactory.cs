using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
//
public class EnemyFactory : MonoBehaviour
{
    
    public int currWave;
    public int maxWaves;
    public Wave[] waves;
    [SerializeField] float countdown;

    // Start is called before the first frame update
    void Awake()
    {
        maxWaves = waves.Length - 1;
        currWave = 0;
        gameManager.instance.setCurrWave(currWave);
        gameManager.instance.setLastWave(currWave == maxWaves);
        StartCoroutine(SpawnWave());
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;

        if(gameManager.instance.GetEnemyCountCurrent() == 0 && countdown <= 0 && currWave < maxWaves)
        {
            currWave++;
            gameManager.instance.setCurrWave(currWave);
            gameManager.instance.setLastWave(currWave == maxWaves);
            StartCoroutine(SpawnWave());
        }

    }

    public IEnumerator SpawnWave()
    {
        for (int i = 0; i < waves[currWave].enemies.Length;)
        {
            Instantiate(waves[currWave].enemies[i], transform);
            yield return new WaitForSeconds(waves[currWave].nextEnemyTime);
            ++i;
        
        }
    }

    [System.Serializable]
    public class Wave
    {
        public GameObject[] enemies;
        public float nextEnemyTime;
    }
}
