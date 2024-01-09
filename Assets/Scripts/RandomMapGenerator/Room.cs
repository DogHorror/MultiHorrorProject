using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;
using Random = System.Random;



public enum RotAngle
{
    d0 = 0,
    d90 = 1,
    d180 = 2,
    d270 = 3
}

public class Room : MonoBehaviour
{
    [Header("Prefab Settings")]
    public Vector3Int scale;
    public List<Vector3Int> doorSpawnPoints = new List<Vector3Int>();

    [Space(20f)]
    public List<GameObject> lights = new List<GameObject>();
    public List<GameObject> emissiveObjects = new List<GameObject>();

    [Header("Runtime Settings")] 
    [SerializeField] private int[] minDoorCount;
    [SerializeField] private int[] maxDoorCount;

    private List<Vector3Int> doors = new List<Vector3Int>();
    [SerializeField] private Grid3D<Wall> wallGrid;
    [SerializeField] private Wall[] walls;
    
    
    private Random random;
    private int seed;
    private Vector3Int mapSize;
    [SerializeField] private Vector3Int position;
    private RotAngle rotAngle;

    public Wall[] GetDoorPosition()
    {
        Wall[] doorPositions = new Wall[doors.Count];
        for (int i = 0; i < doors.Count; i++)
        {
            //doorPositions[i] = wallGrid[doors[i]];
        }
        
        return walls;
    }



    public void Init(Vector3Int mapSize, Vector3Int position, RotAngle rotAngle, int seed)
    {
        this.seed = seed;
        this.mapSize = mapSize;
        this.position = position;
        this.rotAngle = rotAngle;
        random = new Random(seed);

        walls = GetComponentsInChildren<Wall>(true);
        
        //wallGrid = new Grid3D<Wall>(new Vector3Int(scale.x + 2, scale.y, scale.z + 2), Vector3Int.zero);

        for (int i = 0; i < walls.Length; i++)
        {
            //wallGrid[walls[i].gridPosition] = walls[i];
            walls[i].SetPosition(position);
        }
        
        SetRandomDoor();
    }
    
    public void SetRandomDoor()
    {
        List<Vector3Int>[] tempDoors = new List<Vector3Int>[scale.y];
        for (int i = 0; i < tempDoors.Length; i++)
        {
            tempDoors[i] = new List<Vector3Int>();
        }
        foreach (Vector3Int point in doorSpawnPoints)
        {
            tempDoors[point.y].Add(point);
        }
        for (int floor = 0; floor < tempDoors.Length; floor++)
        {
            int maxCount = (maxDoorCount[floor] == -1) ? doorSpawnPoints.Count : maxDoorCount[floor];
            int doorCount = random.Next(1, maxCount);

            for (int i = 0; i < doorCount; i++)
            {
                var index = random.Next(0, tempDoors[floor].Count);
                doors.Add(tempDoors[floor][index]);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        foreach (Vector3Int door in doorSpawnPoints)
        {
            Gizmos.color = Color.red;
            foreach (Wall wall in walls)
            {
                if(wall.gridPosition == door)
                    Gizmos.color = Color.blue;
            }
            Gizmos.DrawWireSphere(transform.position + new Vector3(door.x * 12f + 6f, door.y * 5f + 2.5f, door.z * 12f + 6f), 2f);
        }
    }
}
