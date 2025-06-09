using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class Climbing : MonoBehaviour
{
    [Header("Climbing")]
    [SerializeField] private float maxClimbTime;
    [SerializeField] private LayerMask whatIsWall;

    private float climbTimer;

    [Header("Climb Jumping")]
    [SerializeField] private float climbJumpUpForce;
    [SerializeField] private float climbJumpBackForce;
    [SerializeField] private int climbJumps;

    private int climbJumpsLeft;

    [Header("Detection")]
    [SerializeField] private float detectionLenght;
    [SerializeField] private float detectionLenghtBackward;
    [SerializeField] private float sphereCastRadius;

    private RaycastHit frontWallHit;
    public bool wallFront;

    [Header("Exititing")]
    [SerializeField] private float exitingWallTime;

    public bool wallWithoutInteraction;
    public int wallDirection;

    public bool exitingWall;
    private float exitingWallTimer;

    private Transform lastWall;
    private Vector3 lastWallNormal;

    private PlayerController playerController;
    private Rigidbody rb;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        CanClimb();
        StateMachine();

        
        if(!exitingWall)
            ChooseWallDirection();

        wallWithoutInteraction = WallCheckWithoutInteraction(Vector3.right) || WallCheckWithoutInteraction(Vector3.left);
    }
    private void FixedUpdate()
    {
        if (playerController.climbing && !playerController.crouching
            && !playerController.sliding && !exitingWall)
        {
            ClimbingMovement();
        }
    }

    private void ChooseWallDirection()
    {
        if (WallCheckWithoutInteraction(Vector3.right))
            wallDirection = 1;
        else if (WallCheckWithoutInteraction(Vector3.left))
            wallDirection = -1;
        else if(WallCheckWithoutInteractionBackward())
            wallDirection = 0;
    }
    private void StateMachine()
    {
        if(playerController.holding && playerController.climbing)
        {
            StopClimbing();
        }
        else if(wallFront && playerController.inputVector != Vector2.zero && !exitingWall)
        {
            if(!playerController.climbing && climbTimer > 0)
            {
                StartClimbing();
            }

            if(climbTimer > 0)
            {
                climbTimer -= Time.deltaTime;
            }
            if(climbTimer <= 0)
            {
                StopClimbing();
            }
        }
        else if(exitingWall)
        {
            if (playerController.climbing)
            {
                StopClimbing();
            }

            if (exitingWallTimer > 0)
            {
                exitingWallTimer -= Time.deltaTime;
            }
            if (exitingWallTimer <= 0)
            {
                exitingWall = false;
            }
        }
        else
        {
            if(playerController.climbing)
            {
                StopClimbing();
            }
        }

        if(!playerController.holding 
            && wallWithoutInteraction
            && playerController.wantToWallJump && climbJumpsLeft > 0)
        {
            ClimbJump();
            playerController.wantToWallJump = false;
        }

        if (!playerController.holding && WallCheckWithoutInteractionBackward() && playerController.wantToWallJump && climbJumpsLeft > 0)
        {
            ClimbJumpFromBackward();
            playerController.wantToWallJump = false;
        }
    }

    private void CanClimb()
    {
        wallFront = WallCheckSides() || WallCheckBackwards();
        bool newWall = frontWallHit.transform != lastWall;

        if ((wallFront && newWall) || playerController.isGrounded || playerController.wallRunnning || playerController.holding)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }

    public bool WallCheckWithoutInteractionBackward()
    {
        return Physics.SphereCast(transform.position, sphereCastRadius,
            Vector3.forward, out frontWallHit, detectionLenghtBackward, whatIsWall);
    }
    public bool WallCheckWithoutInteraction(Vector3 direction)
    {
        return Physics.SphereCast(transform.position, sphereCastRadius,
            direction, out frontWallHit, detectionLenght, whatIsWall);
    }
    public bool WallCheckSides()
    {
        return Physics.SphereCast(transform.position, sphereCastRadius,
            new Vector3(playerController.inputVector.x, 0, 0), out frontWallHit, detectionLenght, whatIsWall);
    }
    
    public bool WallCheckBackwards()
    {
        return Physics.SphereCast(transform.position, sphereCastRadius,
            new Vector3(0, 0, playerController.inputVector.y), out frontWallHit, detectionLenghtBackward, whatIsWall);
    }

    private void StartClimbing()
    {
        playerController.climbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, playerController.climbingSpeed, rb.velocity.z);
    }

    private void StopClimbing()
    {
        playerController.climbing = false;
        //rb.velocity = Vector3.zero;
    }

    private void ClimbJump()
    {
        if (playerController.isGrounded || playerController.holding || GetComponent<LedgeGrabbing>().exitingLedge)
            return;

        Vector3 forceToApply = Vector3.up * climbJumpUpForce + Vector3.right * -playerController.playerDirection * climbJumpBackForce;

        playerController.playerDirection = -playerController.playerDirection;

        exitingWall = true;
        exitingWallTimer = exitingWallTime;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;

        climbTimer = maxClimbTime;
        climbJumpsLeft = climbJumps;
    }

    private void ClimbJumpFromBackward()
    {
        if (playerController.isGrounded || playerController.holding || GetComponent<LedgeGrabbing>().exitingLedge)
            return;

        Vector3 forceToApply = Vector3.up * climbJumpUpForce + Vector3.right * playerController.inputVector.x * climbJumpBackForce;

        exitingWall = true;
        exitingWallTimer = exitingWallTime;

        rb.velocity = new Vector3(0, 0, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;
    }
}
