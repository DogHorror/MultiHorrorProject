using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    [SerializeField]
    private int gridX;
    [SerializeField]
    private int gridY;
    [SerializeField]
    private int gridZ;
    
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;
    [SerializeField]
    private int depth;

    private bool[,] shape;  // ex.  [ 1, 1 ]  : L shape
                            //      [ 1, 0 ]
                            // 
                            // ex.  [ 1, 1 ]  : I shape
                            

    private bool isFliped;  // ex. [ 0, 1 ] : Flipped L Shape
                            //     [ 1, 1 ]
                            //
                            // ex. [ 1 ] : Flipped I Shape
                            //     [ 1 ]

    public Room(int gridX, int gridY, int gridZ, int width, int height, int depth, bool[,] shape, bool isFliped)
    {
        this.gridX = gridX;
        this.gridY = gridY;
        this.gridZ = gridZ;

        this.width = width;
        this.height = height;
        this.depth = depth;

        this.shape = shape;

        this.isFliped = isFliped;
    }

    public void Init(GameObject room)
    {
        
    }
}
