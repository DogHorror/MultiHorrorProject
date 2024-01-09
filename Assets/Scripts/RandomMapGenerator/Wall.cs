using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("Wall Setting")]
    public Room room;
    public int floor;
    public Vector3Int gridPosition;
    
    [Space(10f)]
    [Header("Objects Settings")]
    [SerializeField] private GameObject wallObject;
    [SerializeField] private GameObject doorObject;
    private Vector3Int position;
    

    #if UNITY_EDITOR
    private void OnValidate()
    {
        room = GetComponentInParent<Room>();
    }
    #endif
    public void Start()
    {
        room = GetComponentInParent<Room>();
    }

    public void ActiveDoor()
    {
        wallObject.SetActive(false);
        doorObject.SetActive(true);
    }
    
    public void ActiveWall()
    {
        wallObject.SetActive(true);
        doorObject.SetActive(false);
    }

    public void SetPosition(Vector3Int position)
    {
        this.position = position;
    }

    public Vector3Int GetDoorPosition()
    {
        return gridPosition + position;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(room.transform.position + new Vector3(gridPosition.x * 12f + 6f, gridPosition.y * 5f + 2.5f, gridPosition.z * 12f + 6f), 1f);
    }
}
