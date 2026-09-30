using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class FishFlockManager : MonoBehaviour
{
    const int MAX_BATCH_SIZE = 1023;

    [Header ("Fish Setting")]
    [SerializeField] Material material;
    [SerializeField] Mesh mesh;
    [SerializeField] private float scale = 1f;
    [SerializeField] int numberOfFishes = 20;
    [SerializeField] int numberOfFishesInSchools = 50;
    [SerializeField] int schoolRadios = 50;


    [Header("Flock Settings")]
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
    [SerializeField]private bool showObstacle;
    [SerializeField]private bool showGrid;

    private Vector3 fishScale;
    RenderParams rp;
    Matrix4x4[][] instData;
    Vector3[] fishPositions;

    Vector3 alignment;
    Vector3 cohesion;
    Vector3 separation;
    Vector3 avoidanceSteer;

    // obstacle grid
    bool[,,] obstacleGrid;
    int gridX, gridY, gridZ;
    int safeCellSize;
    List<Vector3Int> activeCells = new List<Vector3Int>();

    Vector3[] fishVelocities;


    //FishList
    List<int>[,,] fishList;

    private Vector3 gridBottemLeft; // it gives bottem left value in the origin

    void Start()
    {
        fishScale = Vector3.one * scale;
        List<int> school = new List<int>();
        
        // this the bottom left of the grid 
        gridBottemLeft = transform.position - (gridWorldSize / 2f);
        
        // necessary for Instancing 
        rp = new RenderParams(material);
        fishPositions = new Vector3[numberOfFishes];
        fishVelocities = new Vector3[numberOfFishes];
        
        // Total number of batches in matrix 
        int totalBatches = Mathf.CeilToInt((float)numberOfFishes / MAX_BATCH_SIZE);
        instData = new Matrix4x4[totalBatches][];
        
        // Gives the size of matrix
        for (int i = 0; i < totalBatches; i++)
        {
            int currentBatchSize = (i == totalBatches - 1 && numberOfFishes % MAX_BATCH_SIZE != 0)
                                 ? (numberOfFishes % MAX_BATCH_SIZE)
                                 : MAX_BATCH_SIZE;

            instData[i] = new Matrix4x4[currentBatchSize];
        }
        
        //the gird 
        InitializeGrid();
        
        int currentNumberOfFishes = numberOfFishes;
        int schoolCount = Mathf.CeilToInt((float)numberOfFishes / numberOfFishesInSchools);  //number of fishes in a school
        
        // checks if there is no fish 
        if (currentNumberOfFishes > 0)
        {
            int globalFishIndex = 0;
            
            for (int i = 0; i < schoolCount; i++)
            {   
                // Determine the size and radius of fish 
                Vector3 schoolcenter = new Vector3(
                    Random.Range(transform.position.x - gridWorldSize.x / 2f + schoolRadios, transform.position.x + gridWorldSize.x / 2f - schoolRadios),
                    Random.Range(transform.position.y - gridWorldSize.y / 2f + schoolRadios, transform.position.y + gridWorldSize.y / 2f - schoolRadios),
                    Random.Range(transform.position.z - gridWorldSize.z / 2f + schoolRadios, transform.position.z + gridWorldSize.z / 2f - schoolRadios)
                );

                for (int j = 0; j < numberOfFishesInSchools; j++)
                {
                    if (currentNumberOfFishes <= 0)
                    {
                        break;
                    }

                    // Apply position using the global index, then increase it by 1 for the next fish
                    fishPositions[globalFishIndex] = schoolcenter + Random.insideUnitSphere * schoolRadios;

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
        
        //number of cells in x y z Vecotor3 value 
        gridX = Mathf.CeilToInt(gridWorldSize.x / safeCellSize);
        gridY = Mathf.CeilToInt(gridWorldSize.y / safeCellSize);
        gridZ = Mathf.CeilToInt(gridWorldSize.z / safeCellSize);
        
        //3D array if the grid
        obstacleGrid = new bool[gridX, gridY, gridZ];
        
        // 3d list of fish
        fishList = new List<int>[gridX, gridY, gridZ];

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                for (int z = 0; z < gridZ; z++)
                {
                    //center of the every cell
                    Vector3 cellCenter = gridBottemLeft + new Vector3(
                        (x * safeCellSize) + (safeCellSize / 2f),
                        (y * safeCellSize) + (safeCellSize / 2f),
                        (z * safeCellSize) + (safeCellSize / 2f)
                    );
                    
                    // it is used to make avoidance zone 
                    bool isObstacle = Physics.CheckSphere(cellCenter, cellSize / 2f, obstacleLayer);

                    obstacleGrid[x, y, z] = isObstacle;
                    
                    if (x == 0 ||  x == gridX - 1 || y == 0 || y == gridY - 1 || z == 0 || z == gridZ - 1)
                    {
                        obstacleGrid[x, y, z] = true;
                    }
                    fishList[x, y, z] = new List<int>();
                }
            }
        }
    }

    void UpdateSpatialHash()
    {
        //Only clear cells that had fish in them last frame
        foreach (var cell in activeCells)
        {
            fishList[cell.x, cell.y, cell.z].Clear();
        }
        activeCells.Clear();

        //Assign fish and record active cells
        for (int i = 0; i < numberOfFishes; i++)
        {
            Vector3Int index = WorldPositionToArrayIndex(fishPositions[i]);

            if (fishList[index.x, index.y, index.z].Count == 0)
            {
                activeCells.Add(index); // Mark this cell to be cleared next frame
            }

            fishList[index.x, index.y, index.z].Add(i);
        }
    }
    
    // This function is used to change world position to array index
    Vector3Int WorldPositionToArrayIndex(Vector3 position)
    {
        safeCellSize = Mathf.Max(1, cellSize);

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
            Vector3Int centerIndex = WorldPositionToArrayIndex(fishPositions[i]);

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

            Vector3 steer = Vector3.zero;

            if (neighbourCount > 0)
            {
                Vector3 align = alignment / neighbourCount;
                Vector3 cohes = (cohesion / neighbourCount) - fishPositions[i];

                steer = (align * alignmentWeight) + (cohes * cohesionWeight) + (separation * separationWeight);
                steer = Vector3.ClampMagnitude(steer, maxForce);
            }

            //Obstacle Avoidance
            Ray ray = new Ray(fishPositions[i],fishVelocities[i].normalized);
            
            // Vector3Int aheadIndex = WorldPositionToArrayIndex(fishPositions[i] + fishVelocities[i].normalized * sightDistance);
            Vector3Int[] aheadIndexes = new Vector3Int[4];

            Vector3 forwards = fishVelocities[i].normalized;

            for (int j = 0; j < 4; j++)
            {
                float distance = sightDistance * (j + 1) / 4f;

                aheadIndexes[j] = WorldPositionToArrayIndex(
                    fishPositions[i] + forwards * distance
                );
            }

            for (int j = 0; j < 4; j++)
            {
                Vector3Int index = aheadIndexes[j];
                
                if (obstacleGrid[index.x, index.y, index.z])
                {
                    Vector3 cellCenter = gridBottemLeft + new Vector3(
                        (index.x * safeCellSize) + (safeCellSize / 2f),
                        (index.y * safeCellSize) + (safeCellSize / 2f),
                        (index.z * safeCellSize) + (safeCellSize / 2f)
                    );

                    // box bounds
                    Vector3 vMin = cellCenter - new Vector3((safeCellSize / 2f), (safeCellSize / 2f), (safeCellSize / 2f));
                    Vector3 vMax = cellCenter + new Vector3((safeCellSize / 2f), (safeCellSize / 2f), (safeCellSize / 2f));

                    if (Intersect(vMin, vMax, ray, out float smoothMinBound))
                    {
                        Vector3 hitPoint = fishPositions[i] + fishVelocities[i].normalized * smoothMinBound;
                        
                        Vector3 away = (hitPoint - cellCenter).normalized;
                        
                        Vector3 forward = fishVelocities[i].normalized;

                        Vector3 lateral = away - forward *  Vector3.Dot(away, forward);
                        
                        avoidanceSteer = lateral.normalized * avoidanceWeight;
                        break;
                    }
                }
            }

            steer += avoidanceSteer;
            steer = Vector3.ClampMagnitude(steer, maxForce);

            fishVelocities[i] += steer * Time.deltaTime;
            fishVelocities[i] = fishVelocities[i].normalized * movementSpeed;
            fishPositions[i]  += fishVelocities[i] * Time.deltaTime;
            
            // --- END INLINED NEIGHBOUR CHECK ---
        }
    }
    
    public bool Intersect(Vector3 boundsMin, Vector3 boundsMax, Ray ray ,out float tmin)
    {
        Vector3 invdir = new Vector3(1 / ray.direction.x, 1 / ray.direction.y, 1 / ray.direction.z);
        
        tmin = (boundsMin.x - ray.origin.x) * invdir.x;
        float tmax = (boundsMax.x - ray.origin.x) * invdir.x;
        
        if (tmin > tmax) (tmin, tmax) = (tmax, tmin);

        float tymin = (boundsMin.y - ray.origin.y) * invdir.y;
        float tymax = (boundsMax.y - ray.origin.y) * invdir.y;
        
        if(tymin > tymax) (tymin, tymax) = (tymax, tymin);
        
        if ((tmin > tymax) || (tymin > tmax)) 
            return false;
        
        if (tymin > tmin) tmin = tymin;
        if (tymax < tmax) tmax = tymax;
        
        float tzmin = (boundsMin.z - ray.origin.z) * invdir.z;
        float tzmax = (boundsMax.z - ray.origin.z) * invdir.z;
        
        if (tzmin > tzmax)  (tzmin, tzmax) = (tzmax, tzmin);
        
        if ((tmin > tzmax) || (tzmin > tmax))
            return false;
        
        if (tzmin > tmin) tmin = tzmin;
        if (tzmax < tmax) tmax = tzmax;
        return true;
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

        for(int b = 0; b < instData.Length; b++)
        {
            Graphics.RenderMeshInstanced(rp, mesh, 0, instData[b]);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (obstacleGrid == null) return;
            
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, gridWorldSize);
        
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
                    if(showGrid)
                    {
                        Gizmos.color = (obstacleGrid[x, y, z]) ? Color.red : Color.white;
                        // Draw the individual cell
                        if (showObstacle)
                        { 
                            if(obstacleGrid[x, y, z])
                                Gizmos.DrawWireCube(cellCenter, Vector3.one * safeCellSize);
                        }
                        
                        else
                            Gizmos.DrawWireCube(cellCenter, Vector3.one * safeCellSize);
                    }
                }
            }
        }
    }
}