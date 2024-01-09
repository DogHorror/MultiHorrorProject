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
    [SerializeField] private Vector2Int[] doorCountRange;

    private List<Vector3Int> doors = new List<Vector3Int>();
    [SerializeField] private Grid3D<Wall> wallGrid;
    [SerializeField] private Wall[] walls;
    
    
    private Random random;
    private int seed;
    private Vector3Int mapSize;
    [SerializeField] private Vector3Int position;
    private RotAngle rotAngle;

    public List<Wall> GetDoorPosition()
    {
        List<Wall> ret = new List<Wall>();
        foreach (Vector3Int door in doors)
        {
            foreach (Wall wall in walls)
            {
                if (door == wall.gridPosition)
                {
                    if(!ret.Contains(wall)) ret.Add(wall);
                }
            }
        }
        Debug.Log("Room GetDoorPosition : " + ret.Count);
        return ret;
    }

    public void Init(Vector3Int mapSize, Vector3Int position, RotAngle rotAngle, int seed)
    {
        this.seed = seed;
        this.mapSize = mapSize;
        this.position = position;
        this.rotAngle = rotAngle;
        random = new Random(seed);

        walls = GetComponentsInChildren<Wall>(true);
        
        for (int i = 0; i < walls.Length; i++)
        {
            //wallGrid[walls[i].gridPosition] = walls[i];
            walls[i].room = this;
            walls[i].ActiveWall();
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
            int doorCount = random.Next(doorCountRange[floor].x, doorCountRange[floor].y);

            for (int i = 0; i < doorCount; i++)
            {
                var index = random.Next(0, tempDoors[floor].Count);
                doors.Add(tempDoors[floor][index]);
                doorSpawnPoints.Remove(tempDoors[floor][index]);
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
