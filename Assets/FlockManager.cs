using UnityEngine;
using static UnityEditor.PlayerSettings;

public class FlockManager : MonoBehaviour
{
    public static FlockManager FM;
    public GameObject fishPrefab;
    public int numFish = 20;
    public GameObject[] allFish;
    public Vector3 swimLimits = new Vector3(5, 5, 5);
    public Vector3 goalPos = Vector3.zero;

    [Header("Fish Settings")]
    [Range(0.0f, 5.0f)]
    public float minSpeed;
    [Range(0.0f, 15.0f)]
    public float maxSpeed;
    [Range(1.0f, 10.0f)]
    public float neighbourDist;
    [Range(1.0f, 5.0f)]
    public float rotationSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        allFish = new GameObject[numFish];
        for(int i = 0; i < numFish; i++)
        {
            Vector3 pos = this.transform.position + new Vector3(Random.Range(-swimLimits.x, swimLimits.x),
                                                                Random.Range(-swimLimits.y, swimLimits.y), 
                                                                Random.Range(-swimLimits.z, swimLimits.z));
            allFish[i] = Instantiate(fishPrefab, pos, Quaternion.identity);
        }
        FM = this;
        goalPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0, 10000) < 10)
        {
            Vector3 potentialGoal = this.transform.position + new Vector3(
                Random.Range(-swimLimits.x, swimLimits.x),
                Random.Range(-swimLimits.y, swimLimits.y),
                Random.Range(-swimLimits.z, swimLimits.z)
            );

            // Check if the potential goal is not inside an obstacle
            if (!Physics.CheckSphere(potentialGoal, 3.0f, LayerMask.GetMask("Obstacle")))
            {
                goalPos = potentialGoal;
            }
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, swimLimits * 2);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(goalPos, 0.5f);
    }
}
