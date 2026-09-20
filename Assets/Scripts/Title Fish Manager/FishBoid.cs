using UnityEngine;

public class FishBoid : MonoBehaviour
{
    [Header("Flocking Weights")]
    public float cohesionWeight = 1.0f;
    public float alignmentWeight = 1.0f;
    public float separationWeight = 1.5f; // Usually needs to be stronger to prevent clipping

    [Header("Detection Radiuses")]
    public float neighborDistance = 4.0f;
    public float separationDistance = 1.5f;
    
    [Header("Movement")]
    public float maxSpeed = 3.0f;
    public Vector3 velocity;

    [Header("Debug & Gizmos")]
    public bool showGizmos = true;

    // Storing forces for Gizmos
    private Vector3 cohesionForce;
    private Vector3 alignmentForce;
    private Vector3 separationForce;

    void Start()
    {
        // Give each fish a random starting velocity
        velocity = Random.insideUnitSphere * maxSpeed;
    }

    void Update()
    {
        Vector3 cohesion = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 separation = Vector3.zero;
        int numNeighbors = 0;

        // Loop through all fish to find neighbors
        foreach (FishBoid otherFish in FlockManager.allFishes)
        {
            if (otherFish == this || otherFish == null) continue;

            float distance = Vector3.Distance(transform.position, otherFish.transform.position);

            if (distance < neighborDistance)
            {
                // 1. Cohesion: Add up all neighbor positions
                cohesion += otherFish.transform.position;
                
                // 2. Alignment: Add up all neighbor velocities
                alignment += otherFish.velocity;
                
                numNeighbors++;

                // 3. Separation: Push away if too close
                if (distance < separationDistance && distance > 0.001f)
                {
                    Vector3 pushAway = transform.position - otherFish.transform.position;
                    // Weight the separation force by how close they are
                    separation += pushAway.normalized / distance; 
                }
            }
        }

        if (numNeighbors > 0)
        {
            // Average out the vectors
            cohesion = (cohesion / numNeighbors - transform.position).normalized;
            alignment = (alignment / numNeighbors).normalized;
            separation = separation.normalized;
        }

        // Store for Gizmos
        cohesionForce = cohesion;
        alignmentForce = alignment;
        separationForce = separation;

        // Keep fish inside a general area so they don't fly off to infinity
        // Vector3 boundaryForce = Vector3.zero;
        // if (transform.position.magnitude > FlockManager.flockBounds)
        // {
        //     boundaryForce = -transform.position.normalized * 2f; // Steer back to center (0,0,0)
        // }

        // Apply weights and add to current velocity
        velocity += (cohesion * cohesionWeight + 
                     alignment * alignmentWeight + 
                     separation * separationWeight ) * Time.deltaTime;

        // Clamp speed so they don't go super sonic
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // Move and rotate the fish
        transform.position += velocity * Time.deltaTime;
        if (velocity != Vector3.zero)
        {
            transform.forward = velocity.normalized;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // Draw Cohesion in BLUE
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, cohesionForce * 1.5f);

        // Draw Alignment in GREEN
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, alignmentForce * 1.5f);

        // Draw Separation in RED
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, separationForce * 1.5f);
    }
}