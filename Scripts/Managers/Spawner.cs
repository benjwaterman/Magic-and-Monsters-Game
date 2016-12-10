using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject[] Enemy_Prefabs;
    public float SpawnTimer;
    public int TotalActiveSpawns;
    public int MaxNumberOfSpawns;

    private float timeSinceLastSpawn = 0;
    private int numberOfSpawns = 0;
    private List<GameObject> enemyList = new List<GameObject>();
    private Vector3 spawnLocation;

    void Start()
    {
        spawnLocation = transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
            spawnLocation = hit.point;
    }

    void Update()
    {
        if (numberOfSpawns < MaxNumberOfSpawns)
        {
            if (timeSinceLastSpawn >= SpawnTimer)
            {
                SpawnEnemy();
                timeSinceLastSpawn = 0;
            }
            else
                timeSinceLastSpawn += Time.deltaTime;
        }
    }

    void SpawnEnemy()
    {
        if(enemyList.Count < TotalActiveSpawns )
        {
            GameObject newEnemy = (GameObject)Instantiate(Enemy_Prefabs[Random.Range(0, Enemy_Prefabs.Length)], spawnLocation, Quaternion.identity);
            newEnemy.GetComponent<EnemyController>().ParentSpawner = this;
            enemyList.Add(newEnemy);

            numberOfSpawns++;
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemyList.Remove(enemy);
    }
}
