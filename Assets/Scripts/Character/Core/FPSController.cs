using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    
    [Header("Movement")] 
    [SerializeField] private FPSMovement movementComponent;
    // Used for free-look
    private Vector2 _freeLookInput;
    
    private Quaternion desiredRotation;
    private Quaternion moveRotation;
    
    private float _jumpState = 0f;
    
    private Vector2 _playerInput;

    private float sensitivity = 2f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    private void UpdateLookInput()
    {
        float deltaMouseX = Input.GetAxis("Mouse X") * sensitivity;
        float deltaMouseY = -Input.GetAxis("Mouse Y") * sensitivity;
        
        _freeLookInput = Vector2.Lerp(_freeLookInput, Vector2.zero, 1 - Mathf.Exp(-15f * Time.deltaTime));
        
        _playerInput.x += deltaMouseX;
        _playerInput.y += deltaMouseY;
        
        //float proneWeight = animator.GetFloat("ProneWeight");
        float proneWeight = 0.2f;
        Vector2 pitchClamp = Vector2.Lerp(new Vector2(-90f, 90f), new Vector2(-30, 0f), proneWeight);
        
        _playerInput.y = Mathf.Clamp(_playerInput.y, pitchClamp.x, pitchClamp.y);
        moveRotation *= Quaternion.Euler(0f, deltaMouseX, 0f);
        //TurnInPlace();

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
}
