using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Degree{
    d0 = 0, d90 = 1, d180 = 2, d270 =3
};


[Serializable]
public class Room
{
    [SerializeField]
    private int id;
    
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

    [SerializeField]
    private GameObject room;

    public Room(GameObject room, int id, int gridX, int gridY, int gridZ, int width, int height, int depth)
    {
        this.id = id;
        
        this.gridX = gridX;
        this.gridY = gridY;
        this.gridZ = gridZ;

        this.width = width;
        this.height = height;
        this.depth = depth;
        
        this.room = room;
    }

    public void Init(GameObject room)
    {
        
    }
}
