using UnityEngine;

public class Flocking : MonoBehaviour
{
    float speed;
    bool turning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        Bounds b = new Bounds(FlockManager.FM.transform.position, FlockManager.FM.swimLimits * 2);

        // Determine if the fish needs to turn back to stay within bounds
        turning = !b.Contains(transform.position);

        if (turning)
        {
            // Turn towards the center of the bounds
            Vector3 direction = FlockManager.FM.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager.FM.rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Adjust speed randomly for natural movement
            if (Random.Range(0, 100) < 10)
                speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);

            // Apply flocking behavior
            if (Random.Range(0, 100) < 30)
                ApplyRules();

            // Avoid obstacles
            ObstacleAvoidance();
        }

        // Move forward
        transform.Translate(0, 0, speed * Time.deltaTime);
    }

    void ApplyRules()
    {
        GameObject[] gos = FlockManager.FM.allFish;

        Vector3 vCenter = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        // Calculate center and avoidance for neighbors
        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if (nDistance <= FlockManager.FM.neighbourDist)
                {
                    vCenter += go.transform.position;
                    groupSize++;

                    if (nDistance < 1.0f)
                    {
                        vAvoid += (this.transform.position - go.transform.position);
                    }

                    Flocking anotherFlock = go.GetComponent<Flocking>();
                    gSpeed += anotherFlock.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            // Calculate the average center and adjust speed
            vCenter = vCenter / groupSize + (FlockManager.FM.goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            if (speed > FlockManager.FM.maxSpeed)
                speed = FlockManager.FM.maxSpeed;

            // Adjust direction to flock and avoid
            Vector3 direction = (vCenter + vAvoid) - transform.position;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                                                      FlockManager.FM.rotationSpeed * Time.deltaTime);
        }
    }

    void ObstacleAvoidance()
    {
        float avoidDistance = 1f; // Radius of the sphere for obstacle detection
        float avoidStrength = 10.0f; // How strongly the fish turns to avoid obstacles

        // Detect all colliders within the specified radius
        Collider[] obstacles = Physics.OverlapSphere(transform.position, avoidDistance, LayerMask.GetMask("Obstacle"));

        // If there are obstacles nearby
        if (obstacles.Length > 0)
        {
            Vector3 avoidDirection = Vector3.zero;

            // Calculate a combined avoidance direction
            foreach (Collider obstacle in obstacles)
            {
                Vector3 directionToObstacle = transform.position - obstacle.transform.position;
                avoidDirection += directionToObstacle.normalized; // Add weighted direction away from the obstacle
            }

            // Adjust fish rotation to avoid the obstacles
            avoidDirection.Normalize();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(avoidDirection), avoidStrength * Time.deltaTime);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f); // Match avoidDistance
    }


}
