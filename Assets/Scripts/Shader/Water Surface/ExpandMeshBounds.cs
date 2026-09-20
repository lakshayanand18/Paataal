using UnityEngine;

public class ExpandMeshBounds : MonoBehaviour
{
    public float padding = 2f; // match/exceed your max displacement
    
        void Start()
        {
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Bounds b = mesh.bounds;
            b.Expand(padding);
            mesh.bounds = b;
            Debug.Log(mesh.bounds);
        }
}