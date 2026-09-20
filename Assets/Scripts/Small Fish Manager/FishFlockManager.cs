using System.Collections.Generic;
using UnityEngine;

public class FishFlockManager : MonoBehaviour
{
    const int MAX_BATCH_SIZE = 1023;
    readonly Vector3 fishScale = new Vector3(0.2f, 0.2f, 0.2f);



    [Header ("Fish Setting")]
    [SerializeField] Material material;
    [SerializeField] Mesh mesh;
    [SerializeField] int numberOfFishes = 20;
    [SerializeField] int numberOfFishesInSchools = 50;
    [SerializeField] int SchoolRadious = 50;
    
    [Header("Flock Settings")]
    [SerializeField] Transform player;
    [SerializeField] float movementSpeed;
    [SerializeField] float maxForce = 5f;
    [SerializeField] float alignmentWeight;
    [SerializeField] float cohesionWeight;
    [SerializeField] float separationWeight;
    [SerializeField] float separationPerameter;

    [Header("Confined Area Settings")]
    [SerializeField] Vector3 gridWorldSize = new Vector3(50f, 50f, 50f); // Size of your "fish tank"
    [SerializeField] int cellSize = 5;


    [Header ("Obstacle")]
    [SerializeField] LayerMask obstacleLayer; // Assign this to a layer like "Obstacles" or "Default"
    [SerializeField] float sightDistance = 3f; // How far ahead the fish can see
    [SerializeField] float avoidanceWeight = 10f;
    RenderParams rp;
    Matrix4x4[][] instData;
    Vector3[] fishPositions;

    Vector3 alignment;
    Vector3 cohesion;
    Vector3 separation;
    Vector3 avoidanceSteer;

    // tells about fishes 
    public class SchoolData
    {
        public int startIndex;
        public int fishCount;
    }

    public SchoolData[] schools;

    // obstacle grid
    bool[,,] obstacleGrid;
    int gridX, gridY, gridZ;
    int safeCellSize;

    Vector3[] fishVelocities;
    //FishList
    List<int>[,,] fishList;

    List<Vector3Int> activeCells = new List<Vector3Int>();

    private Vector3 gridBottemLeft; // it gives bottem left value in the origin

    void Start()
    {
        List<int> school = new List<int>();


        gridBottemLeft = transform.position - (gridWorldSize / 2f);
        rp = new RenderParams(material);
        fishPositions = new Vector3[numberOfFishes];
        fishVelocities = new Vector3[numberOfFishes];
        // batching 
        int totalBatches = Mathf.CeilToInt((float)numberOfFishes / MAX_BATCH_SIZE);
        instData = new Matrix4x4[totalBatches][];

        for (int i = 0; i < totalBatches; i++)
        {
            int currentBatchSize = (i == totalBatches - 1 && numberOfFishes % MAX_BATCH_SIZE != 0)
                                 ? (numberOfFishes % MAX_BATCH_SIZE)
                                 : MAX_BATCH_SIZE;

            instData[i] = new Matrix4x4[currentBatchSize];
        }

        InitializeGrid();
        int currentNumberOfFishes = numberOfFishes;
        int schoolCount = Mathf.CeilToInt((float)numberOfFishes / numberOfFishesInSchools);

        if (currentNumberOfFishes > 0)
        {
            int globalFishIndex = 0;

            for (int i = 0; i < schoolCount; i++)
            {
                Vector3 schoolcenter = new Vector3(
                    Random.Range(transform.position.x - gridWorldSize.x / 2f + SchoolRadious, transform.position.x + gridWorldSize.x / 2f - SchoolRadious),
                    Random.Range(transform.position.y - gridWorldSize.y / 2f + SchoolRadious, transform.position.y + gridWorldSize.y / 2f - SchoolRadious),
                    Random.Range(transform.position.z - gridWorldSize.z / 2f + SchoolRadious, transform.position.z + gridWorldSize.z / 2f - SchoolRadious)
                );

                for (int j = 0; j < numberOfFishesInSchools; j++)
                {
                    if (currentNumberOfFishes <= 0)
                    {
                        break;
                    }

                    // Apply position using the global index, then increase it by 1 for the next fish
                    fishPositions[globalFishIndex] = schoolcenter + Random.insideUnitSphere * SchoolRadious;

                    globalFishIndex++;
                    currentNumberOfFishes--;
                }
            }
        }
        // Spawn fish randomly within the confined area
        for (int i = 0; i < numberOfFishes; i++)
        {
            fishVelocities[i] = Random.insideUnitSphere.normalized * movementSpeed;
        }
    }
    
    void Update()
    {
        UpdateSpatialHash();
        Flocking();
        FishRendering();
    }
    void InitializeGrid()
    {
        safeCellSize = Mathf.Max(1, cellSize);

        gridX = Mathf.CeilToInt(gridWorldSize.x / safeCellSize);
        gridY = Mathf.CeilToInt(gridWorldSize.y / safeCellSize);
        gridZ = Mathf.CeilToInt(gridWorldSize.z / safeCellSize);

        // Initialize the 3D array instead of the Dictionary
        obstacleGrid = new bool[gridX, gridY, gridZ];
        fishList = new List<int>[gridX, gridY, gridZ];

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                for (int z = 0; z < gridZ; z++)
                {
                    Vector3 cellCenter = gridBottemLeft + new Vector3(
                        (x * safeCellSize) + (safeCellSize / 2f),
                        (y * safeCellSize) + (safeCellSize / 2f),
                        (z * safeCellSize) + (safeCellSize / 2f)
                    );

                    bool isObstacle = Physics.CheckSphere(cellCenter, cellSize / 2f, obstacleLayer);

                    // Assign directly to the 3D array
                    obstacleGrid[x, y, z] = isObstacle;
                    fishList[x, y, z] = new List<int>();
                }
            }
        }
    }

    void UpdateSpatialHash()
    {
        // 1. Only clear cells that had fish in them last frame
        for (int i = 0; i < activeCells.Count; i++)
        {
            fishList[activeCells[i].x, activeCells[i].y, activeCells[i].z].Clear();
        }
        activeCells.Clear();

        // 2. Assign fish and record active cells
        for (int i = 0; i < numberOfFishes; i++)
        {
            Vector3Int index = PositionToArrayIndex(fishPositions[i]);

            if (fishList[index.x, index.y, index.z].Count == 0)
            {
                activeCells.Add(index); // Mark this cell to be cleared next frame
            }

            fishList[index.x, index.y, index.z].Add(i);
        }
    }

    Vector3Int PositionToArrayIndex(Vector3 position)
    {
        int safeCellSize = Mathf.Max(1, cellSize);

        Vector3 localPos = position - gridBottemLeft;

        // Calculate the raw array indexes
        int x = Mathf.FloorToInt(localPos.x / safeCellSize);
        int y = Mathf.FloorToInt(localPos.y / safeCellSize);
        int z = Mathf.FloorToInt(localPos.z / safeCellSize);

        x = Mathf.Clamp(x, 0, gridX - 1);
        y = Mathf.Clamp(y, 0, gridY - 1);
        z = Mathf.Clamp(z, 0, gridZ - 1);

        return new Vector3Int(x, y, z);
    }

    void Flocking()
    {
        float sepParamSqr = separationPerameter * separationPerameter;

        for (int i = 0; i < numberOfFishes; i++)
        {
            alignment = Vector3.zero;
            cohesion = Vector3.zero;
            separation = Vector3.zero;
            avoidanceSteer = Vector3.zero;

            int neighbourCount = 0;

            // --- INLINED NEIGHBOUR CHECK ---
            Vector3Int centerIndex = PositionToArrayIndex(fishPositions[i]);

            // Search 3x3x3 grid directly
            for (int x = -1; x <= 1; x++)
            {
                int checkX = centerIndex.x + x;
                if (checkX < 0 || checkX >= gridX) continue;

                for (int y = -1; y <= 1; y++)
                {
                    int checkY = centerIndex.y + y;
                    if (checkY < 0 || checkY >= gridY) continue;

                    for (int z = -1; z <= 1; z++)
                    {
                        int checkZ = centerIndex.z + z;
                        if (checkZ < 0 || checkZ >= gridZ) continue;

                        List<int> cellFishes = fishList[checkX, checkY, checkZ];
                        int cellFishCount = cellFishes.Count;

                        for (int j = 0; j < cellFishCount; j++)
                        {
                            int neighbourIndex = cellFishes[j];
                            if (neighbourIndex == i) continue;

                            Vector3 neighbourPos = fishPositions[neighbourIndex];

                            // ALIGNMENT
                            alignment += fishVelocities[neighbourIndex];

                            // COHESION
                            cohesion += neighbourPos;

                            // SEPARATION 
                            Vector3 awaydirection = fishPositions[i] - neighbourPos;
                            float sqrDist = awaydirection.sqrMagnitude; 

                            if (sqrDist > 0f && sqrDist < sepParamSqr)
                            {
                                separation += awaydirection ;
                            }

                            neighbourCount++;
                        }
                    }
                }
            }
            // --- END INLINED NEIGHBOUR CHECK ---

            Vector3 steer = Vector3.zero;

            if (neighbourCount > 0)
            {
                Vector3 align = alignment / neighbourCount;
                Vector3 cohes = (cohesion / neighbourCount) - fishPositions[i];
                Vector3 seperate = alignment / neighbourCount;

                steer = (align * alignmentWeight) + (cohes * cohesionWeight) + (separation * separationWeight);
                steer = Vector3.ClampMagnitude(steer, maxForce);
            }

            // Avoidance
            Vector3 aheadPos = fishPositions[i] + (fishVelocities[i].normalized * sightDistance);
            Vector3Int aheadIndex = PositionToArrayIndex(aheadPos);

            if (obstacleGrid[aheadIndex.x, aheadIndex.y, aheadIndex.z])
            {
                Vector3 cellCenter = gridBottemLeft + new Vector3(
                    (aheadIndex.x * safeCellSize) + (safeCellSize / 2f),
                    (aheadIndex.y * safeCellSize) + (safeCellSize / 2f),
                    (aheadIndex.z * safeCellSize) + (safeCellSize / 2f)
                );
                avoidanceSteer = (fishPositions[i] - cellCenter).normalized * avoidanceWeight;
            }

            steer += avoidanceSteer;
            steer = Vector3.ClampMagnitude(steer, maxForce);

            fishVelocities[i] += steer * Time.deltaTime;
            fishVelocities[i] = fishVelocities[i].normalized * movementSpeed;
            fishPositions[i] += fishVelocities[i] * Time.deltaTime;
        }
    }


    void FishRendering()
    {
        for (int i = 0; i < numberOfFishes; i++)
        {
            int batchIndex = i / MAX_BATCH_SIZE;
            int localIndex = i % MAX_BATCH_SIZE;

            Quaternion rotation = Quaternion.identity;
            // Optimization: sqrMagnitude is faster than checking != Vector3.zero
            if (fishVelocities[i].sqrMagnitude > 0.001f)
            {
                rotation = Quaternion.LookRotation(fishVelocities[i]);
            }

            instData[batchIndex][localIndex] = Matrix4x4.TRS(fishPositions[i], rotation, fishScale);
        }

        for (int b = 0; b < instData.Length; b++)
        {
            Graphics.RenderMeshInstanced(rp, mesh, 0, instData[b]);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, gridWorldSize);
        int safeCellSize = Mathf.Max(1, cellSize);
        
        // Calculate grid dimensions safely for the Editor
        int gX = Mathf.CeilToInt(gridWorldSize.x / safeCellSize);
        int gY = Mathf.CeilToInt(gridWorldSize.y / safeCellSize);
        int gZ = Mathf.CeilToInt(gridWorldSize.z / safeCellSize);

        Vector3 origin = transform.position - (gridWorldSize / 2f);

        // Use a faint, semi-transparent color for the cells so it isn't too cluttered

        for (int x = 0; x < gX; x++)
        {
            for (int y = 0; y < gY; y++)
            {
                for (int z = 0; z < gZ; z++)
                {
                    // Calculate the exact center point of the current cell
                    Vector3 cellCenter = origin + new Vector3(
                        (x * safeCellSize) + (safeCellSize / 2f),
                        (y * safeCellSize) + (safeCellSize / 2f),
                        (z * safeCellSize) + (safeCellSize / 2f)
                    );

                    // Draw the individual cell
                    Gizmos.color = (obstacleGrid[x, y, z]) ? Color.red : Color.white;
                    Gizmos.DrawWireCube(cellCenter, Vector3.one * safeCellSize);
                }
            }
        }
    }
}