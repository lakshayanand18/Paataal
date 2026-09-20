using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public GameObject fishPrefab;
    public int numberOfFish = 200;
    public Vector3 spawnLimits = new Vector3(10, 10, 10);
    
    public static float flockBounds = 15f; // Radius where fish will turn back to center
    public static FishBoid[] allFishes;

    void Start()
    {
        allFishes = new FishBoid[numberOfFish];

        for (int i = 0; i < numberOfFish; i++)
        {
            // Pick a random position within the spawn limits
            Vector3 randomPos = transform.position + new Vector3(
                Random.Range(-spawnLimits.x, spawnLimits.x),
                Random.Range(-spawnLimits.y, spawnLimits.y),
                Random.Range(-spawnLimits.z, spawnLimits.z)
            );
           Quaternion RandomRotation = Quaternion.Euler(randomPos);

            // Instantiate and add to our tracking array
            GameObject fishObject = Instantiate(fishPrefab, randomPos, RandomRotation);
            allFishes[i] = fishObject.GetComponent<FishBoid>();
        }
    }
}
