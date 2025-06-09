using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject playerModel;

    [Header("Movement Settings")]
    [SerializeField] public float sprintSpeed;
    [SerializeField] public float crouchSpeed;
    [SerializeField] public float slideSpeed;
    [SerializeField] public float wallRunSpeed;
    [SerializeField] public float climbingSpeed;
    [SerializeField] public float moveToLedgeSpeed;
    [SerializeField] public float dashSpeed;
    [SerializeField] public float speedIncreaseMultiplier;
    [SerializeField] public float slopeIncreaseMultiplier;

    public float moveSpeed;
    public float desiredMoveSpeed;
    private Vector3 moveDirection;

    public bool sliding = false;
    public bool wallRunnning = false;
    public bool climbing = false;
    public bool holding = false;
    public bool freeze = false;
    public bool restricted = false;
    public bool dashing = false;
    public bool canMove = true;
    public bool isDead = false;

    public float velocity;

    [Header("Crouch Settings")]
    [SerializeField] public float colliderShrinkScale;
    [SerializeField] public float playerModelOffset;

    public bool crouching = false;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float slideJumpMultiplier = 1.2f;

    public bool wantToJump = false;
    public bool wantToWallJump = false;

    [Header("Ground Check")]
    [SerializeField] public float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] float groundDrag;

    [Header("References")]
    [SerializeField] private CameraController _camera;

    public bool isGrounded;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;

    private RaycastHit slopeHit;

    public Vector2 inputVector { get; private set; }
    public int playerDirection;

    private float startShrinkScale;
    private Rigidbody rb;
    public MovementState movementState;
    public enum MovementState
    {
        DASHING,
        FREEZE,
        SPRINTING,
        CROUCHING,
        SLIDING,
        ClIMBING,
        WALLRUNNING
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        startShrinkScale = GetComponent<CapsuleCollider>().height;

        inputVector = new Vector2();
        rb = GetComponent<Rigidbody>();

        moveSpeed = sprintSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDead)
            return;

        GroundCheck();
        MovePlayer();
        SpeedControl();
        _Jump();
        StateHandler();
                
        velocity = rb.velocity.x;
    }

    private void StateHandler()
    {
        //if (restricted) return;
        if(dashing)
        {
            movementState = MovementState.DASHING;
            desiredMoveSpeed = dashSpeed;
        }
        else if(freeze)
        {
            movementState = MovementState.FREEZE;
            rb.velocity = Vector3.zero;
            desiredMoveSpeed = 0;
        }
        else if(crouching)
        {
            movementState = MovementState.CROUCHING;
            desiredMoveSpeed = crouchSpeed;
        }
        else if(sliding)
        {
            movementState = MovementState.SLIDING;
        }
        else if (wallRunnning)
        {
            movementState = MovementState.WALLRUNNING;
            desiredMoveSpeed = wallRunSpeed;
        }
        else if(climbing)
        {
            movementState = MovementState.ClIMBING;
        }
        else if(!canMove)
        {
            desiredMoveSpeed = 0;
        }
        else
        {
            movementState = MovementState.SPRINTING;
            desiredMoveSpeed = sprintSpeed;
        }

        if (sliding && moveSpeed != 0)
        {
            StopCoroutine(SmoothlyLerpMoveSpeed());
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }
    }

    private void Update()
    {
        if(!isGrounded && !holding)
            GravityMultiplier();
    }

    public void Movement(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();

        if (inputVector.x != 0)
            playerDirection = (int)inputVector.x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (wallRunnning || holding 
                || (GetComponent<Climbing>().wallWithoutInteraction && !isGrounded) 
                || GetComponent<Climbing>().WallCheckBackwards())
            {
                wantToWallJump = true;
            }
            else
            {
                wantToJump = true;
            }   
        }

        if (context.canceled)
        {
            wantToWallJump = false;
            wantToJump = false;
        }
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed && !holding && isGrounded)
        {
            if (rb.velocity.magnitude - 0.01f < sprintSpeed)
            {
                CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

                capsuleCollider.height -= colliderShrinkScale;
                /*playerModel.transform.localPosition =
                    new Vector3(0, -playerHeight / 2 + (colliderShrinkScale - playerModelOffset), 0);*/
                rb.AddForce(Vector3.down * 20f, ForceMode.Impulse);

                crouching = true;
            }
            else
            {
                if (!wallRunnning)
                {
                    GetComponent<Sliding>().wantToSlide = true;
                    desiredMoveSpeed = slideSpeed;

                    sliding = true;
                    gameObject.layer = 10;
                }

                GetComponent<WallRunning>().downwardsRunning = true;
            }          
        }

        if (context.canceled)
        {
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

            capsuleCollider.height = startShrinkScale;
            /*playerModel.transform.localPosition = startModelPos;*/

            StopCoroutine(SmoothlyLerpMoveSpeed());

            GetComponent<Sliding>().wantToSlide = false;
            GetComponent<Sliding>().StopSlide();

            desiredMoveSpeed = sprintSpeed;
            gameObject.layer = 9;

            crouching = false;
        }
    }
    private void _Jump()
    {
        if ((isGrounded || OnSlope()) && wantToJump)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            if (sliding)
            {
                rb.AddForce(Vector3.up * jumpForce * slideJumpMultiplier, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    private void GravityMultiplier()
    {
        Vector3 gravityMultiplier = new Vector3();

        if (rb.velocity.y < 0)
        {
            gravityMultiplier = Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !wantToJump)
        {
            gravityMultiplier = Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        rb.velocity += gravityMultiplier;
    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2f + 0.2f, whatIsGround);

        if (isGrounded
            && (movementState == MovementState.SPRINTING || movementState == MovementState.CROUCHING))
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void SpeedControl()
    {
        if (OnSlope())
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0, 0);

            if (flatVel.magnitude > moveSpeed)
            {
                var limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, rb.velocity.z);
            }
        }

    }

    private void MovePlayer()
    {
        if (GetComponent<Climbing>().exitingWall || GetComponent<Climbing>().WallCheckSides())
        {
            return;
        }

        if (isGrounded && !wantToJump)
            moveDirection = new Vector3(inputVector.x * moveSpeed * 10, 0, 0);
        else if (wallRunnning)
        {
            var wallRunDirection = GetComponent<WallRunning>().wallRunDirection;
            moveDirection = new Vector3(wallRunDirection * moveSpeed * 10, 0, 0);
        }
        else moveDirection = new Vector3(inputVector.x * moveSpeed * 10 * airMultiplier, 0, 0);

        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else
        {
            rb.AddForce(moveDirection, ForceMode.Force);
        }

        if (!wallRunnning && !holding)
            rb.useGravity = !OnSlope();
    }

    public IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startSpeed = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startSpeed, desiredMoveSpeed, time / difference);

            if(OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime * speedIncreaseMultiplier;
            }
            
            yield return null;
        }
    }
    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector2 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

}
