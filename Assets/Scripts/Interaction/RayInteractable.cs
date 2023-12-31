using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RayInteractable : MonoBehaviour
{
    public string promptMessage;

    public UnityEvent OnInteract;

    public void Interact()
    {
        OnInteract.Invoke();
    }
}
