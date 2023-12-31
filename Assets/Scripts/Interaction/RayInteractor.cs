using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RayInteractor : NetworkBehaviour
{
    [Header("Ray Settings")] 
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Camera camera;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private List<RayInteractable> interactables = new List<RayInteractable>();
    [SerializeField] private int checkPerFrame = 3;
    [SerializeField] private float interactRange = 10f;

    
    [Header("UI Settings")] 
    [SerializeField] private RectTransform handCursor;
    
    private int tick = 0;

    public void Start()
    {
        handCursor = UIManager.instance.handCursor;
    }
    
    public void Update()
    {
        tick++;
        if (handCursor && tick > checkPerFrame)
        {
            tick = 0;
            RayInteractable element = GetClosestElementOnCamera();
            if (element)
            {
                if (isLocalPlayer && !UIManager.instance.camera) UIManager.instance.camera = camera;
                UIManager.instance.hasHandCursorTarget = true;
                UIManager.instance.SetHandCursorTarget(element.transform.position);
            }
            else
            {
                UIManager.instance.hasHandCursorTarget = false;
            }

            UIManager.instance.promptMessage.enabled = false;
            Ray ray = new Ray(cameraHolder.position, cameraHolder.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange, targetLayer))
            {
                RayInteractable hitElement = hitInfo.collider.GetComponent<RayInteractable>();
                if (hitElement)
                {
                    UIManager.instance.promptMessage.enabled = true;
                    UIManager.instance.promptMessage.text = hitElement.promptMessage;
                }
            }
        }
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
        Debug.Log("Trigger Enter");
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
        Debug.Log("Trigger Exit");
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
