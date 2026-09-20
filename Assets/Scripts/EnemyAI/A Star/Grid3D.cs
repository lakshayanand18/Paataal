using System.Collections.Generic;
using UnityEngine;

public class Grid3D : MonoBehaviour
{
    public Transform player;
    public LayerMask unwalkableMask3D;
    public Vector3 gridWorldSize;
    public float nodeRadius;
    public TerrainType3D[] walkableRegions3D;
    public bool displayWhiteGrid = false;
    public bool onlyDisplayNonWalkableArea = false;
    public bool showPlayer = false;
    public float sphereSize = .5f;
    LayerMask walkableMask3D;
    Dictionary<int, int> walkableRegionsDictionary3D = new Dictionary<int, int>();
    Node3D[,,] node3D;

    float nodeDiameter;
    int gridSizeX, gridSizeY, gridSizeZ;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

        foreach (TerrainType3D region in walkableRegions3D)
        {
            walkableMask3D.value |= region.terrainMask3D.value;
            walkableRegionsDictionary3D.Add((int)Mathf.Log(region.terrainMask3D.value, 2), region.terrainPenalty3D);
        }
        
        CreateGrid();
    }
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY * gridSizeZ;
        }
    }
    void CreateGrid()
    {
        node3D = new Node3D[gridSizeX, gridSizeY, gridSizeZ];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2 - Vector3.forward * gridWorldSize.z / 2;


        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                    // Check if the node is walkable in 3D space
                    bool walkable3D = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask3D);

                    int movementPenalty = 0; // You can set movement penalty based on your requirements

                    //raycast to determine if the node is walkable
                    if (walkable3D)
                    {
                        Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 100, walkableMask3D))
                        {
                            walkableRegionsDictionary3D.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                        }
                    }
                    node3D[x, y, z] = new Node3D(walkable3D, worldPoint, x, y, z, movementPenalty);
                }
            }
        }
    }
    public List<Node3D> GetNeighbours(Node3D node)
    {
        List<Node3D> neighbours = new List<Node3D>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    //it will continue if the node is the center node
                    if (x == 0 && y == 0 && z == 0)    
                        continue;

                    //check neighbour node in 3D space 
                    int checkX = node.gridX + x;            
                    int checkY = node.gridY + y;
                    int checkZ = node.gridZ + z;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        neighbours.Add(node3D[checkX, checkY, checkZ]);
                    }
                }
            }
        }
        return neighbours;
    }
    
    public Node3D NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        float percentZ = (worldPosition.z + gridWorldSize.z / 2) / gridWorldSize.z;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSizeY - 1);
        z = Mathf.Clamp(z, 0, gridSizeZ - 1);

        return node3D[x, y, z];
    }
    void OnDrawGizmos()
    {
        // Draw grid boundary
        Gizmos.DrawWireCube(transform.position, gridWorldSize);

        // Exit early if grid not yet generated
        if (node3D == null) return;

        // Get player's current node (if player assigned)
        if (showPlayer && player != null)
        {
            Node3D playerNode = NodeFromWorldPoint(player.position);
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(playerNode.worldPosition, Vector3.one * nodeDiameter);
            List<Node3D> playerProximity = PlayerProzimity(NodeFromWorldPoint(player.position), 10);

            foreach (Node3D n in playerProximity)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(n.worldPosition, nodeRadius);
            }
        }

        foreach (Node3D n in node3D)
        {
            // Skip drawing walkable (white) nodes if disabled
            if (displayWhiteGrid)
            {
                Gizmos.color = (n.walkable3D) ? Color.white : Color.red;
                Gizmos.DrawSphere(n.worldPosition, sphereSize);
            }
            else if (onlyDisplayNonWalkableArea && !n.walkable3D)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(n.worldPosition, sphereSize);
            }
        }
    }
    [System.Serializable]public class TerrainType3D
    {
        public LayerMask terrainMask3D;
        public int terrainPenalty3D;
    }

    public List<Node3D> PlayerProzimity(Node3D node, int distance)
    {
        List<Node3D> neighbours = new List<Node3D>();

        int sqrRadius = distance * distance;

        for (int x = -distance; x <= distance; x++)
        {
            for (int y = -distance; y <= distance; y++)
            {
                for (int z = -distance; z <= distance; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    int SqrDistance = x * x + y * y + z * z;

                    if (SqrDistance <= sqrRadius)
                    {
                        int checkX = node.gridX + x;
                        int checkY = node.gridY + y;
                        int checkZ = node.gridZ + z;

                        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ)
                        {
                            neighbours.Add(node3D[checkX, checkY, checkZ]);
                        }
                    }

                }
            }
        }
        return neighbours;
    }

    private void Start()
    {
        int size = MaxSize;

        Debug.Log(size);
    }
}