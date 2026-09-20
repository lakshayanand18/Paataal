using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class spawn : MonoBehaviour
{
    public int waveList;
    public GameObject enemy;
    public int spawnRadius;
    public float timeBeforeNewWave;
    

    private void Start()
    {
        //Initial Wave
        for (int i = 1; i <= waveList; i++)
        {
            Vector3 position = transform.position + Random.insideUnitSphere * spawnRadius; 
            Instantiate(enemy, position, Quaternion.identity);
        }
    }

    private void Update()
    {
        if(timeBeforeNewWave < 2f)
            timeBeforeNewWave = 5f;
        
        // Incremental Wave Spawn
        
        if (Time.time > timeBeforeNewWave)
        {
            for (int i = 1; i <= waveList; i++)
            {
                Vector3 position = transform.position + Random.insideUnitSphere * spawnRadius;
                Instantiate(enemy, position, Quaternion.identity);
            }
            timeBeforeNewWave += Time.timeSinceLevelLoad;
        }
    }
}