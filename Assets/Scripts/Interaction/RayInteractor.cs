using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RayInteractor : MonoBehaviour
{
    [Header("Ray Settings")] 
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private List<RayInteractable> interactables = new List<RayInteractable>();
    
    
    
    public void OnTriggerEnter(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            RayInteractable element = other.GetComponent<RayInteractable>();
            if (element != null)
            {
                if(!interactables.Contains(element)) interactables.Add(element);
            }
        }
    }
    
    public void OnTriggerExit(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            RayInteractable element = other.GetComponent<RayInteractable>();
            if (element != null)
            {
                if(interactables.Contains(element)) interactables.Remove(element);
            }
        }
    }
}
