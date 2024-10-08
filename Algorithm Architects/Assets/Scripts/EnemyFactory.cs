using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int intialPoolSize = 5;    //Initial number of enemies to create in the pool

    private Queue<GameObject> enemyPool = new Queue<GameObject>();

    // Singleton instance of the factory
    public static EnemyFactory instance;

    void Awake()
    { 
        //Ensure that only one instance of the EnemyFactory is up
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializePool();
    }


    

    //Initializ the pool with a set number of enemies
    private void InitializePool()
    {
        for (int i = 0; i < intialPoolSize; i++)
        {
            CreateNewEnemy();
        }
    }

    //Pull an enemy from the pool
    public GameObject GetEnemy(Vector3 position, Quaternion rotation)
    {
        if (enemyPool.Count > 0)
        {
            GameObject enemy = enemyPool.Dequeue();
            enemy.SetActive(true);
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            return enemy;
        }
        else
        {
            //If the pool is empty, create a new enemy
            return CreateNewEnemy(position, rotation);  
        }
    }

    //Return the enemy to pool to improve performance
    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }

    //Helper to create ne enemy
    private GameObject CreateNewEnemy(Vector3 position = default, Quaternion rotation = default)
    {
        GameObject newEnemy = Instantiate(enemyPrefab, position, rotation);
        newEnemy.SetActive(false); // Disable the enemy initially
        enemyPool.Enqueue(newEnemy);
        return newEnemy;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
