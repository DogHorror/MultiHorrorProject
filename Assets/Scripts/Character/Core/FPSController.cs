using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : NetworkBehaviour
{
    [Header("Controller")]
    [SerializeField] private float xSensitivity;
    [SerializeField] private float ySensitivity;
    
    [Header("Camera")] 
    [SerializeField] private Camera camera;

    private float xRotation = 0f;
    private PlayerInput playerInput;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = InputManager.instance.playerInput;;

        if (!isLocalPlayer)
            camera.enabled = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!isLocalPlayer) return;
        UpdateLook();
    }

    private void UpdateLook()
    {
        Vector2 input = playerInput.actions["Look"].ReadValue<Vector2>();
        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
    }
}
