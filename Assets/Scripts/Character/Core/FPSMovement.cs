using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;


public enum MovementState
{
    Idle,
    Walking,
    Sprinting,
    InAir
}

public enum PoseState
{
    Standing,
    Crouching,
    Prone
}

public class FPSMovement : NetworkBehaviour
{
    #region Member Variable
    //==================================================================
    

    [Header("Physics Setting")] [SerializeField]
    private float gravity = 10f;
    [SerializeField]
    private float maxFallVelocity = 25f;
    [SerializeField]
    private float desiredGait = 3f;
    [SerializeField]
    private float walkingGait = 3f;
    [SerializeField]
    private float sprintingGait = 6.5f;
    [SerializeField]
    private float crouchingGait = 1.5f;
    [SerializeField]
    private float velocitySmoothing = 5f;
    [SerializeField]
    private float airFriction = 0.3f;
    [SerializeField]
    private float airVelocity = 0.4f;
    [SerializeField]
    private float jumpHeight = 4f;
    
    public Vector3 MoveVector { get; private set; }
    
    [Header("Character Settings")]
    public float crouchRatio;
    
    public Transform rootBone;

    
    private PlayerInput playerInput;
    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 inputDirection;
    private float originalHeight;
    private Vector3 originalCenter;

    public MovementState movementState;
    public PoseState poseState;
    //==================================================================
    #endregion
    
    #region Unity Life Cycle
    //==================================================================
    
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        
        originalHeight = controller.height;
        originalCenter = controller.center;

        movementState = MovementState.Idle;
        poseState = PoseState.Standing;

    }
    
    private void Update()
    {
        var prevState = movementState;
        UpdateMovementState();
        UpdatePoseState();
        
        if (prevState != movementState)
        {
            OnMovementStateChanged(prevState);
        }
        
        if (movementState == MovementState.InAir)
        {
            UpdateInAir();
        }
        else
        {
            UpdateGrounded();
        }
        
        UpdateMovement();
    }

    //==================================================================
    #endregion

    #region Functionality
    //==================================================================

    private void UnCrouch()
    {
        controller.height = originalHeight;
        controller.center = originalCenter;
            
        poseState = PoseState.Standing;
            
        //_animator.SetBool(Crouching, false);
        //onUncrouch.Invoke();
    }
    
    private void Crouch()
    {
        float crouchedHeight = originalHeight * crouchRatio;
        float heightDifference = originalHeight - crouchedHeight;

        controller.height = crouchedHeight;

        // Adjust the center position so the bottom of the capsule remains at the same position
        Vector3 crouchedCenter = originalCenter;
        crouchedCenter.y -= heightDifference / 2;
        controller.center = crouchedCenter;

        poseState = PoseState.Crouching;
            
        //_animator.SetBool(Crouching, true);
        //onCrouch.Invoke();
    }

    //==================================================================
    #endregion
    
    #region Check Condition
    //==================================================================
    
    private bool IsMoving()
    {
        return !Mathf.Approximately(inputDirection.normalized.magnitude, 0f);
    }
    
    private bool CanUnCrouch()
    {
        float height = originalHeight - controller.radius * 2f;
        Vector3 position = rootBone.TransformPoint(originalCenter + Vector3.up * height / 2f);
        return !Physics.CheckSphere(position, controller.radius);
    }
    
    public bool IsInAir()
    {
        return !controller.isGrounded;
    }

    #endregion
    
    #region Check State
    //==================================================================
    
    private bool TryJump()
    {
        if (!playerInput.actions["Jump"].triggered || poseState == PoseState.Crouching) return false;

        movementState = MovementState.InAir;
        return true;
    }
    
    
    private bool TrySprint()
    {
        if (poseState is PoseState.Crouching or PoseState.Prone)
        {
            return false;
        }

        if (inputDirection.y <= 0f || inputDirection.x != 0f || !playerInput.actions["Sprint"].IsPressed()) return false;

        //if (!CanSprint()) return false;
            
        movementState = MovementState.Sprinting;
        return true;
    }
    

    //==================================================================
    #endregion
    
    #region Update Function
    //==================================================================

    private void UpdateGrounded()
    {
        var normInput = inputDirection.normalized;
        var desiredVelocity = rootBone.right * normInput.x + rootBone.forward * normInput.y;

        desiredVelocity *= desiredGait;

        desiredVelocity = Vector3.Lerp(velocity, desiredVelocity, 1 - Mathf.Exp(-velocitySmoothing * Time.deltaTime));
            
        velocity = desiredVelocity;

        desiredVelocity.y = -gravity;
        MoveVector = desiredVelocity;
    }
        
    private void UpdateInAir()
    {
        var normInput = inputDirection.normalized;
        velocity.y -= gravity * Time.deltaTime;
        velocity.y = Mathf.Max(-maxFallVelocity, velocity.y);
            
        var desiredVelocity = rootBone.right * normInput.x + rootBone.forward * normInput.y;
        desiredVelocity *= desiredGait;
        
        desiredVelocity = Vector3.Lerp(velocity, desiredVelocity * airFriction, 1 - Mathf.Exp(-airVelocity * Time.deltaTime));

        desiredVelocity.y = velocity.y;
        velocity = desiredVelocity;
            
        MoveVector = desiredVelocity;
    }
    
    
    private void UpdateMovement()
    {
        controller.Move(MoveVector * Time.deltaTime);
    }


    private void OnMovementStateChanged(MovementState prevState)
    {
        if (prevState == MovementState.InAir)
        {
            //onLanded.Invoke();
        }

        if (prevState == MovementState.Sprinting)
        {
            //_sprintAnimatorInterp = 7f;
            //onSprintEnded.Invoke();
        }


        if (movementState == MovementState.InAir)
        {
            velocity.y = jumpHeight;
            return;
        }

        if (movementState == MovementState.Sprinting)
        {
            desiredGait = sprintingGait;
            return;
        }

        if (poseState == PoseState.Crouching)
        {
            desiredGait = crouchingGait;
            return;
        }

        
        // Walking state
        desiredGait = walkingGait;
    }
    
    private void UpdatePoseState()
    {
        if (movementState is MovementState.Sprinting or MovementState.InAir)
        {
            return;
        }
        
        if (!playerInput.actions["Crouch"].triggered)
        {
            return;
        }

        if (poseState == PoseState.Standing)
        {
            Crouch();

            desiredGait = crouchingGait;
            return;
        }

        if (!CanUnCrouch()) return;

        UnCrouch();
        desiredGait = walkingGait;
    }
    private void UpdateMovementState()
    {
        if (movementState == MovementState.InAir && IsInAir()) return;
        
        float moveX = playerInput.actions["Move"].ReadValue<Vector2>().x;
        float moveY = playerInput.actions["Move"].ReadValue<Vector2>().y;

        inputDirection.x = moveX;
        inputDirection.y = moveY;
        
        if (TryJump())
        {
            return;
        }
            
        if (TrySprint())
        {
            return;
        }

        if (!IsMoving())
        {
            movementState = MovementState.Idle;
            return;
        }
            
        movementState = MovementState.Walking;
    }
    
    //==================================================================
    #endregion
}
