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

    private void OnValidate()
    {
        room = GetComponentInParent<Room>();
    }
    
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(room.transform.position + new Vector3(gridPosition.x * 12f + 6f, gridPosition.y * 5f + 2.5f, gridPosition.z * 12f + 6f), 2f);
    }
}
