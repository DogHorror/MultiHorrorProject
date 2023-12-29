using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSControllerObsolete : MonoBehaviour
{
    
    [Header("Movement")] 
    [SerializeField] private FPSMovement movementComponent;
    // Used for free-look
    [SerializeField] private float turnInPlaceAngle = 85f;
    [SerializeField] private AnimationCurve turnCurve;
    [SerializeField] private float turnSpeed = 2f;

    [SerializeField]
    private float sensitivity = 2f;

    [Header("Camera")] 
    [SerializeField] private Transform firstPersonCamera;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform mainCamera;

    private bool isTurning;
    private float turnProgress;
    private PlayerInput playerInput;
    private Vector2 _playerInput;
    private Vector2 _freeLookInput;
    
    private Quaternion desiredRotation;
    private Quaternion moveRotation;
    
    private float _jumpState = 0f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLookInput();
    }

    private void LateUpdate()
    {
        UpdateCameraRotation();
    }
    
    private void TurnInPlace()
    {
        float turnInput = _playerInput.x;
        _playerInput.x = Mathf.Clamp(_playerInput.x, -90f, 90f);
        turnInput -= _playerInput.x;

        float sign = Mathf.Sign(_playerInput.x);
        if (Mathf.Abs(_playerInput.x) > turnInPlaceAngle)
        {
            if (!isTurning)
            {
                turnProgress = 0f;
                    
                //animator.ResetTrigger(TurnRight);
                //animator.ResetTrigger(TurnLeft);
                    
                //animator.SetTrigger(sign > 0f ? TurnRight : TurnLeft);
            }
                
            isTurning = true;
        }

        transform.rotation *= Quaternion.Euler(0f, turnInput, 0f);
            
        float lastProgress = turnCurve.Evaluate(turnProgress);
        turnProgress += Time.deltaTime * turnSpeed;
        turnProgress = Mathf.Min(turnProgress, 1f);
            
        float deltaProgress = turnCurve.Evaluate(turnProgress) - lastProgress;

        _playerInput.x -= sign * turnInPlaceAngle * deltaProgress;
            
        transform.rotation *= Quaternion.Slerp(Quaternion.identity,
            Quaternion.Euler(0f, sign * turnInPlaceAngle, 0f), deltaProgress);
            
        if (Mathf.Approximately(turnProgress, 1f) && isTurning)
        {
            isTurning = false;
        }
    }

    private void UpdateLookInput()
    {
        
        float deltaMouseX = playerInput.actions["Look"].ReadValue<Vector2>().x * sensitivity * Time.deltaTime;
        float deltaMouseY = -playerInput.actions["Look"].ReadValue<Vector2>().y * sensitivity * Time.deltaTime;
        
        _freeLookInput = Vector2.Lerp(_freeLookInput, Vector2.zero, 1 - Mathf.Exp(-15f * Time.deltaTime));
        
        _playerInput.x += deltaMouseX;
        _playerInput.y += deltaMouseY;
        
        //float proneWeight = animator.GetFloat("ProneWeight");
        float proneWeight = 0.2f;
        Vector2 pitchClamp = Vector2.Lerp(new Vector2(-90f, 90f), new Vector2(-30, 0f), proneWeight);
        
        _playerInput.y = Mathf.Clamp(_playerInput.y, pitchClamp.x, pitchClamp.y);
        moveRotation *= Quaternion.Euler(0f, deltaMouseX, 0f);
        TurnInPlace();

        _jumpState = Mathf.Lerp(_jumpState, movementComponent.IsInAir() ? 1f : 0f,
            1 - Mathf.Exp(-10f * Time.deltaTime));

        //float moveWeight = Mathf.Clamp01(movementComponent.AnimatorVelocity.magnitude);
        //transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, moveWeight);
        transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, _jumpState);
        //_playerInput.x *= 1f - moveWeight;
        _playerInput.x *= 1f - _jumpState;

        //charAnimData.SetAimInput(_playerInput);
        //charAnimData.AddDeltaInput(new Vector2(deltaMouseX, charAnimData.deltaAimInput.y));
    }
    
    
    public void UpdateCameraRotation()
    {
        Vector2 input = _playerInput;
            
        (Quaternion, Vector3) cameraTransform =
            (transform.rotation * Quaternion.Euler(input.y, input.x, 0f),
                firstPersonCamera.position);

        cameraHolder.rotation = cameraTransform.Item1;
        cameraHolder.position = cameraTransform.Item2;

        mainCamera.rotation = cameraHolder.rotation * Quaternion.Euler(_freeLookInput.y, _freeLookInput.x, 0f);
    }
}
