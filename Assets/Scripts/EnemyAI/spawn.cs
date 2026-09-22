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
    private float newWaveTime = 0f;

    private void Start()
    {
        if (newWaveTime == timeBeforeNewWave)
        {
            newWaveTime = timeBeforeNewWave;
        }
        //Initial Wave
        for (int i = 1; i <= waveList; i++)
        {
            Vector3 position = transform.position + Random.insideUnitSphere * spawnRadius; 
            Instantiate(enemy, position, Quaternion.identity);
        }
    }

    private void Update()
    {
        // Incremental Wave Spawn
        
        if (Time.timeSinceLevelLoad > newWaveTime)
        {
            for (int i = 1; i <= waveList; i++)
            {
                Vector3 position = transform.position + Random.insideUnitSphere * spawnRadius;
                Instantiate(enemy, position, Quaternion.identity);
            }
            newWaveTime += timeBeforeNewWave;
        }
    }
}