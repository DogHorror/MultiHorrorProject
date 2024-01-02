using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class FPSController : NetworkBehaviour
{
    [Header("Controller")]
    [SerializeField] private float xSensitivity;
    [SerializeField] private float ySensitivity;
    
    
    
    [Header("Camera")] 
    [SerializeField] private Camera camera;

    [SerializeField] private Transform aimTarget;
    [SerializeField] private float aimDistance = 20f;
    [SerializeField] private float aimSmoothing = 0.2f;
    
    [Header("Network")]
    [SerializeField] private Renderer[] renderers;
    
    [Header("Interaction")]
    public RayInteractable interactionTarget;
    
    private float xRotation = 0f;
    private float yRotation = 0f;
    private PlayerInput playerInput;
    private Vector3 lookTargetPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        playerInput = InputManager.instance.playerInput;;

        if (!isLocalPlayer)
            camera.enabled = false;
        else
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                //renderers[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }
    }
    
    
    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        UpdateLook();
        UpdateTargetLook();
    }

    private void UpdateTargetLook()
    {
        
        if (interactionTarget != null)
        {
            lookTargetPosition = interactionTarget.transform.position;
        }

        aimTarget.position = Vector3.Lerp(aimTarget.position, lookTargetPosition, aimSmoothing);
    }

    private void UpdateLook()
    {
        Vector2 input = playerInput.actions["Look"].ReadValue<Vector2>();
        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        float yDiff = (mouseX * Time.deltaTime) * xSensitivity;
        
        yRotation += yDiff;

        camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * yDiff);

        lookTargetPosition = camera.transform.position + Quaternion.Euler(xRotation, yRotation, 0) * Vector3.forward * aimDistance;
    }
}
