using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Room : MonoBehaviour
{
    [Header("Prefab Settings")]
    public Vector3Int scale;
    public List<Vector3Int> doorSpawnPoints = new List<Vector3Int>();

    [Space(10f)]
    public List<GameObject> lights = new List<GameObject>();
    public List<GameObject> emissiveObjects = new List<GameObject>();

    [Header("Runtime Settings")] 
    public int seed;
    [SerializeField] private int minDoorCount = 1;
    [SerializeField] private int maxDoorCount = -1;

    private Random random;
    [SerializeField] private List<Vector3Int> doors = new List<Vector3Int>();

    public void Start()
    {
        random = new Random(seed);
        SetRandomDoor();
    }
    public void SetRandomDoor()
    {
        int maxCount = (maxDoorCount == -1) ? doorSpawnPoints.Count : maxDoorCount;
        int doorCount = random.Next(1, maxCount);

        for (int i = 0; i < doorCount; i++)
        {
            var index = random.Next(0, doorSpawnPoints.Count);
            doors.Add(doorSpawnPoints[index]);
            doorSpawnPoints.RemoveAt(index);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        foreach(Vector3Int door in doorSpawnPoints)
            Gizmos.DrawWireSphere(transform.position + new Vector3(door.x * 12f + 6f, door.y * 5f + 2.5f, door.z * 12f + 6f), 2f);
    }
}
