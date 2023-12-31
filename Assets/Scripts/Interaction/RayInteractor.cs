using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayInteractor : NetworkBehaviour
{
    [Header("Ray Settings")] 
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Camera camera;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private List<RayInteractable> interactables = new List<RayInteractable>();

    
    [Header("UI Settings")] 
    [SerializeField] private RectTransform handCursor;
    
    public void Start()
    {
        handCursor = UIManager.instance.handCursor;
    }
    
    public void Update()
    {
        RayInteractable element = GetClosestElementOnCamera();
        if (element)
        {
            if (isLocalPlayer)
            {
                if (!UIManager.instance.camera)
                {
                    UIManager.instance.camera = camera;
                }
                UIManager.instance.promptMessage.enabled = true;
                UIManager.instance.promptMessage.text = element.promptMessage;
                UIManager.instance.hasHandCursorTarget = true;
                UIManager.instance.SetHandCursorTarget(element.transform.position);
                if (InputManager.instance.playerInput.actions["Interact"].triggered)
                {
                    element.Interact();
                }
            }
        }
        else
        {
            if (isLocalPlayer)
            {
                UIManager.instance.promptMessage.enabled = false;
                UIManager.instance.hasHandCursorTarget = false;
            }
        }
        /*
        Ray ray = new Ray(cameraHolder.position, cameraHolder.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange, targetLayer))
        {
            RayInteractable hitElement = hitInfo.collider.GetComponent<RayInteractable>();
            if (hitElement != null)
            {
                UIManager.instance.promptMessage.enabled = true;
                UIManager.instance.promptMessage.text = hitElement.promptMessage;
                if (InputManager.instance.playerInput.actions["Interact"].triggered)
                {
                    hitElement.Interact();
                }
            }
        }
        */
    }
    
    private RayInteractable GetClosestElementOnCamera()
    {
        if (interactables.Count == 0)
        {
            return null;
        }

        RayInteractable closestElement = null;
        float closestDistance = Mathf.Infinity;
        Vector3 lineDirection = cameraHolder.rotation * Vector3.forward; // 직선의 방향 벡터

        foreach (RayInteractable interactable in interactables)
        {
            Vector3 point = interactable.transform.position;
            Vector3 lineToPoint = point - cameraHolder.position;
            float distance = Vector3.Cross(lineDirection, lineToPoint).magnitude; // 직선과 점 사이의 수직 거리 계산

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestElement = interactable;
            }
        }

        return closestElement;
    }
    
    
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
