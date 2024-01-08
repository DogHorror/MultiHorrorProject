using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using Mirror;

[Serializable]
public struct Grid
{
    public int x;
    public int y;
    public int z;

    public Grid(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

public class RandomMapGeneratorObsolete : NetworkBehaviour
{
    [SerializeField]
    private GameObject[] room1x1x1S;
    [SerializeField]
    private GameObject[] room2x1x1S;
    [SerializeField]
    private GameObject[] room2x2x1S;
    [SerializeField]
    private GameObject[] roomLx1Shapes;

    public List<int[,]> level = new List<int[,]>();
    public List<RoomObsolete> roomList = new List<RoomObsolete>();

    [SerializeField]
    private int roomCount = 0;

    public int gridX = 15;
    public int gridZ = 15;

    public float roomWidth = 12.0f;
    public float roomHeight = 5.0f;
    public float roomDepth = 12.0f;
    
    public bool canPlaceRoom(int[,] grid, List<Grid> checker, int x, int z)
    {
        for (int i = 0; i < checker.Count; i++)
        {
            int newX = x + checker[i].x;
            int newZ = z + checker[i].z;

            if (newX < 0 || newX >= grid.GetLength(0) || newZ < 0 || newZ >= grid.GetLength(1))
                return false;

            if (grid[newX, newZ] != -1)
                return false;
        }

        return true;
    }

    public RoomObsolete CreateRoom(int[,] grid, int x, int y, int z)
    {
        int width = 1;
        int height = 1;

        List<Grid> shape = new List<Grid>();

        Degree degree;

        int flag = -1;
        do
        {
            shape.Clear();
            degree = (Degree)Random.Range(0, 4);
            switch (Random.Range(0, 4))
            {
                case 0:
                    width = 1;
                    height = 1;
                    flag = 0;
                    
                    shape.Add(new Grid(0,0,0));
                    break;
                case 1:
                    if (degree == Degree.d0 || degree == Degree.d180)
                    {
                        width = 2;
                        height = 1;
                        flag = 1;
                    
                        shape.Add(new Grid(0,0,0));
                        shape.Add(new Grid(1,0,0));
                    }
                    else
                    {
                        width = 1;
                        height = 2;
                        flag = 1;
                    
                        shape.Add(new Grid(0,0,0));
                        shape.Add(new Grid(0,0,1));
                    }
                    break;
                case 2:
                    width = 2;
                    height = 2;
                    flag = 2;
                        
                    shape.Add(new Grid(0,0,0));
                    shape.Add(new Grid(1,0,0));
                    shape.Add(new Grid(0,0,1));
                    shape.Add(new Grid(1,0,1));
                    break;
                case 3:
                    if (degree == Degree.d0)
                    {
                        width = 2;
                        height = 2;
                        flag = 3;

                        
                        shape.Add(new Grid(0,0,0));
                        shape.Add(new Grid(1,0,0));
                        shape.Add(new Grid(0,0,1));
                    }
                    else if(degree == Degree.d90)
                    {
                        width = 2;
                        height = 2;
                        flag = 3;
                        
                        
                        shape.Add(new Grid(0,0,0));
                        shape.Add(new Grid(0,0,1));
                        shape.Add(new Grid(1,0,1));
                    }
                    else if(degree == Degree.d180)
                    {
                        width = 2;
                        height = 2;
                        flag = 3;

                        shape.Add(new Grid(0,0,0));
                        shape.Add(new Grid(1,0,0));
                        shape.Add(new Grid(1,0,-1));
                    }
                    else
                    {
                        width = 2;
                        height = 2;
                        flag = 3;

                        shape.Add(new Grid(0,0,0));
                        shape.Add(new Grid(1,0,0));
                        shape.Add(new Grid(1,0,1));
                    }

                    break;
                default:
                    width = 1;
                    height = 1;
                    
                    shape.Add(new Grid(0,0,0));
                    break;
            }
        } while (!canPlaceRoom(grid, shape, x, z));

        if (flag == -1) return null;

        for (int i = 0; i < shape.Count; i++)
        {
            grid[x + shape[i].x, z + shape[i].z] = roomCount;
        }

        GameObject roomPrefab;
        Vector3 roomPosition = new Vector3(x * roomWidth + roomWidth / 2, y * roomHeight + roomHeight / 2, z * roomDepth + roomDepth / 2);
        Quaternion roomRotation = Quaternion.identity;
        
        switch (flag)
        {
            case 0:
                roomPrefab = Instantiate(room1x1x1S[Random.Range(0, room1x1x1S.Length)], roomPosition, roomRotation);
                break;
            case 1:
                if (degree == Degree.d90 || degree == Degree.d270)
                {   
                    roomRotation = Quaternion.Euler(0, 270, 0);
                }
                roomPrefab = Instantiate(room2x1x1S[Random.Range(0, room2x1x1S.Length)], roomPosition, roomRotation);
                break;
            case 2:
                roomPrefab = Instantiate(room2x2x1S[Random.Range(0, room2x2x1S.Length)], roomPosition, roomRotation);
                break;
            case 3:
                if (degree == Degree.d90)
                {
                    roomPosition.z += roomDepth;
                    roomRotation = Quaternion.Euler(0, 90, 0);
                } else if (degree == Degree.d180)
                {
                    roomPosition.x += roomWidth;
                    roomRotation = Quaternion.Euler(0, 180, 0);
                } else if (degree == Degree.d270)
                {
                    roomPosition.x += roomWidth;
                    roomRotation = Quaternion.Euler(0, 270, 0);
                }

                roomPrefab = Instantiate(roomLx1Shapes[Random.Range(0, roomLx1Shapes.Length)], roomPosition, roomRotation);
                break;
            default:
                roomPrefab = Instantiate(room1x1x1S[Random.Range(0, room1x1x1S.Length)], roomPosition, roomRotation);
                break;
        }

        RoomObsolete room = roomPrefab.GetComponent<RoomObsolete>();
        room.Init(roomCount++, shape, x, y, z, roomWidth, roomHeight, roomDepth);
        room.CreateWallUsingGrid(grid);
        return room;
    }

    public void GenerateLevel(int floor)
    {
        
        int[,] grid = new int[gridX, gridZ];

        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridZ; j++)
            {
                grid[i, j] = -1;
            }
        }

        grid[gridX / 2, gridZ / 2] = -2;

        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridZ; j++)
            {
                Debug.Log("Grid[" + i + ", " + j + "] == " + grid[i,j]);
                if (grid[i, j] != -1) continue;

                int width = 1;
                int height = 1;

                RoomObsolete room = CreateRoom(grid, i, 0, j);
                
                roomList.Add(room);
            }
        }
        
        level.Add(grid);
    }

    private void Start()
    {
        if(isServer)
            GenerateLevel(0);
    }
}
