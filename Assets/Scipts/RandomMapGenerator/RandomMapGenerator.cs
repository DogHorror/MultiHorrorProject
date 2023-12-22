using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RandomMapGenerator : MonoBehaviour
{
    //룸의 예시
    [SerializeField]
    private GameObject[] room1x1x1S;
    [SerializeField]
    private GameObject[] room2x1x1S;
    [SerializeField]
    private GameObject[] room2x2x1S;
    [SerializeField]
    private GameObject[] roomLx1Shapes;

    public List<bool[,]> level = new List<bool[,]>();
    public List<Room> roomList = new List<Room>();


    public bool canPlaceRoom(bool[,] grid, bool[,] checker, int x, int y)
    {
        for (int i = 0; i < checker.GetLength(0); i++)
        {
            for (int j = 0; j < checker.GetLength(1); j++)
            {
                if (x + i > 8 || y + j > 8) return false;
                if (grid[x + i, y + j] == false) return false;
            }
        }

        return true;
    }

    public Room CreateRoom(bool[,] grid, int x, int y, int z)
    {
        int width = 1;
        int height = 1;

        bool[,] checker;
        switch (Random.Range(0, 4))
        {
            case 0:
                width = 1;
                height = 1;
                        
                checker = new bool[1,1];
                checker[0, 0] = true;
                break;
            case 1:
                width = 2;
                height = 1;
                        
                checker = new bool[2,1];
                checker[0, 0] = true;
                checker[1, 0] = true;
                break;
            case 2:
                width = 2;
                height = 2;
                        
                checker = new bool[2,2];
                checker[0, 0] = true;
                checker[1, 0] = true;
                checker[0, 1] = true;
                checker[1, 1] = true;
                break;
            case 3:
                width = 2;
                height = 2;
                        
                checker = new bool[2,2];
                checker[0, 0] = true;
                checker[1, 0] = true;
                checker[0, 1] = true;
                checker[1, 1] = false;
                break;
            default:
                width = 1;
                height = 1;
                        
                checker = new bool[1,1];
                checker[0, 0] = true;
                break;
        }

        Room room = new Room(x, y, z, width, height, 1, checker, false);
        return room;
    }

    public void GenerateLevel(int floor)
    {
        bool[,] grid = new bool[9, 9];
        grid[4, 4] = false;

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (grid[i, j] == false) continue;

                int width = 1;
                int height = 1;

                bool[,] checker;
                
                
                
            }
        }
        
    }
}
