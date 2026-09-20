using System.Collections.Generic;
using UnityEngine;

public class SpacialHashMap : MonoBehaviour
{
    public float cellSize = 10f;
    public  List<EnemyContext> AllEnemies = new List<EnemyContext>();
    [SerializeField] private bool showHashMap = false;
    private Dictionary<Vector3Int, List<EnemyContext>> HashMap = new Dictionary<Vector3Int, List<EnemyContext>>();

    public static SpacialHashMap instance;
    public void Awake()
    {
        instance = this;
    }
    
    // Hash Values of the fishes 
    Vector3Int Hash(Vector3 position)
    {
        return new Vector3Int( 
            Mathf.FloorToInt(position.x / cellSize), 
            Mathf.FloorToInt(position.y / cellSize), 
            Mathf.FloorToInt(position.z / cellSize));
    }
    
    // Continuously adds the Enemies in the list 
    void Insert(EnemyContext enemy)
    {
        Vector3Int key = Hash(enemy.EnemyTransform.position);
        if (!HashMap.TryGetValue(key, out List<EnemyContext> cellList))
        {
            cellList = new List<EnemyContext>();
            HashMap[key] = cellList;
        }
        HashMap[key].Add(enemy);
    }

    void FixedUpdate()
    {
        foreach (var enemy in HashMap)
        {
            enemy.Value.Clear();
        }


        foreach (EnemyContext member in AllEnemies)
        {
            if (member.EnemyTransform == null) continue; 
            Insert(member);
        }
    }
    
    //Adds the enemy List
    public void GetNeighbours(Vector3 position, List<EnemyContext> results)
    {
        results.Clear();

        Vector3Int center = Hash(position);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    Vector3Int key = center + new Vector3Int(x, y, z);

                    if (HashMap.TryGetValue(key, out List<EnemyContext> cellList))
                    {
                        results.AddRange(cellList);
                    }
                }
            }
        }
    }
    //Register the Enemy around
    public void Register(EnemyContext enemy)
    {
        if (!AllEnemies.Contains(enemy))
            AllEnemies.Add(enemy);
    }
    
    //Unrgister the Enemies around
    public void UnRegister(EnemyContext enemy)
    {
        if(AllEnemies.Contains(enemy))
            AllEnemies.Remove(enemy);
    }


    void OnDrawGizmos()
    {
        if(showHashMap)
        {
            if (AllEnemies == null)
                return;

            foreach (EnemyContext enemy in SpacialHashMap.instance.AllEnemies)
            {
                Vector3Int key = Hash(enemy.EnemyTransform.position);
                Vector3 cellCenter = new Vector3(
                    key.x * cellSize + cellSize / 2,
                    key.y * cellSize + cellSize / 2,
                    key.z * cellSize + cellSize / 2);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(cellCenter, Vector3.one * cellSize);
            }
        }
    }
}