using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Degree{
    d0 = 0, d90 = 1, d180 = 2, d270 =3
};


public class RoomObsolete : MonoBehaviour
{
    public List<GameObject> wallPrefabList = new List<GameObject>();
    public List<GameObject> windowPrefabList = new List<GameObject>();
    public List<GameObject> doorPrefabList = new List<GameObject>();
    
    [SerializeField]
    private int id;
    
    [SerializeField]
    private int gridX;
    [SerializeField]
    private int gridY;
    [SerializeField]
    private int gridZ;
    
    [SerializeField]
    private float width;
    [SerializeField]
    private float height;
    [SerializeField]
    private float depth;


    [SerializeField]
    private List<GameObject> wallList = new List<GameObject>();

    [SerializeField]
    private List<Grid> shape;

    public List<int> adjacentRoomNumbers = new List<int>();
    
    public void Init(int id, List<Grid> shape, int gridX, int gridY, int gridZ, float width, float height, float depth)
    {
        this.id = id;

        this.shape = shape;
        
        this.gridX = gridX;
        this.gridY = gridY;
        this.gridZ = gridZ;

        this.width = width;
        this.height = height;
        this.depth = depth;
    }

    public void CreateWallUsingGrid(int[,] grid)
    {
        for (int i = 0; i < shape.Count; i++)
        {
            int newX = gridX + shape[i].x;
            int newZ = gridZ + shape[i].z;

            Vector3 position = new Vector3(newX * width + width / 2, transform.position.y, newZ * depth + depth / 2);
            
            
            #region Fraction's Left Side

            if (newZ - 1 < 0)
            {
                GameObject wall = Instantiate(GetRandomElement(windowPrefabList), position,
                    Quaternion.Euler(0, 270, 0));
                wallList.Add(wall);
            } else if (grid[newX, newZ - 1] == -2)
            {
                GameObject wall = Instantiate(GetRandomElement(windowPrefabList), position,
                    Quaternion.Euler(0, 270, 0));
                wallList.Add(wall);
            }
            else if (grid[newX, newZ - 1] != id)
            {
                if (!adjacentRoomNumbers.Contains(grid[newX, newZ - 1]))
                {
                    adjacentRoomNumbers.Add(grid[newX, newZ - 1]);
                    GameObject wall = Instantiate(GetRandomElement(doorPrefabList), position,
                        Quaternion.Euler(0, 270, 0));
                    wallList.Add(wall);
                }
                else
                {
                    GameObject wall = Instantiate(GetRandomElement(wallPrefabList), position,
                        Quaternion.Euler(0, 270, 0));
                    wallList.Add(wall);
                }
            }

            #endregion
            
            #region Fraction's Right Side

            if (newZ + 1 >= grid.GetLength(0))
            {
                GameObject wall = Instantiate(GetRandomElement(windowPrefabList), position,
                    Quaternion.Euler(0, 90, 0));
                wallList.Add(wall);
            } else if (grid[newX, newZ + 1] == -2)
            {
                GameObject wall = Instantiate(GetRandomElement(windowPrefabList), position,
                    Quaternion.Euler(0, 90, 0));
                wallList.Add(wall);
            }

            #endregion
            
            #region Fraction's Up Side

            if (newX - 1 < 0)
            {
                GameObject wall = Instantiate(GetRandomElement(windowPrefabList), position,
                    Quaternion.Euler(0, 0, 0));
                wallList.Add(wall);
            } else if (grid[newX - 1, newZ] == -2)
            {
                GameObject wall = Instantiate(GetRandomElement(windowPrefabList), position,
                    Quaternion.Euler(0, 0, 0));
                wallList.Add(wall);
            } else if (grid[newX - 1, newZ] != id)
            {
                if (!adjacentRoomNumbers.Contains(grid[newX - 1, newZ]))
                {
                    adjacentRoomNumbers.Add(grid[newX - 1, newZ]);
                    GameObject wall = Instantiate(GetRandomElement(doorPrefabList), position,
                        Quaternion.Euler(0, 0, 0));
                    wallList.Add(wall);
                }
                else
                {
                    GameObject wall = Instantiate(GetRandomElement(wallPrefabList), position,
                        Quaternion.Euler(0, 0, 0));
                    wallList.Add(wall);
                }
            }

            #endregion
            
            #region Fraction's Down Side

            if (newX + 1 >= grid.GetLength(0))
            {
                GameObject wall = Instantiate(GetRandomElement(windowPrefabList), position,
                    Quaternion.Euler(0, 180, 0));
                wallList.Add(wall);
            } else if (grid[newX + 1, newZ] == -2)
            {
                GameObject wall = Instantiate(GetRandomElement(windowPrefabList), position,
                    Quaternion.Euler(0, 180, 0));
                wallList.Add(wall);
            }

            #endregion
        }
    }


    private T GetRandomElement<T>(List<T> list)
    {
        int index = Random.Range(0, list.Count);

        return list[index];
    }
    
}
