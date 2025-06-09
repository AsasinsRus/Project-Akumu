using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float wallRunForce;
    [SerializeField] private float wallJumpUpForce;
    [SerializeField] private float wallJumpForwardForce;
    [SerializeField] private float wallJumpSideForce;
    [SerializeField] private float maxWallRunTime;
    [SerializeField] private float wallClimbSpeed;

    private float wallRunTimer;

    [Header("Exiting")]
    [SerializeField] private float exitWallTime;
    
    private float exitWallTimer;
    private bool exitingWall;

    [Header("Gravity")]
    [SerializeField] private bool useGravity;
    [SerializeField] private float gravityCountryForce;

    [Header("Detection")]
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float minJumpHeight;

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    public bool wallLeft;
    public bool wallRight;

    public bool downwardsRunning = false;
    
    private PlayerController playerController;
    private Rigidbody rb;

    public int wallRunDirection = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();

        if (!wallRight && !wallLeft || exitingWall)
            wallRunDirection = 0;
    }

    private void FixedUpdate()
    {
        if(playerController.wallRunnning)
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position - new Vector3(0, playerController.playerHeight / 2, 0), 
            Vector3.back, out rightWallHit, wallCheckDistance, whatIsWall)
            && Physics.Raycast(transform.position + new Vector3(0, playerController.playerHeight / 2, 0), 
            Vector3.back, out rightWallHit, wallCheckDistance, whatIsWall);
        wallRight = Physics.Raycast(transform.position - new Vector3(0, playerController.playerHeight / 2, 0), 
            Vector3.forward, out leftWallHit, wallCheckDistance, whatIsWall)
            && Physics.Raycast(transform.position + new Vector3(0, playerController.playerHeight / 2, 0), 
            Vector3.forward, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        if (wallRunDirection == 0 && (wallRight || wallLeft))
            wallRunDirection = (int)Mathf.Sign(playerController.inputVector.x);

        if ((wallLeft || wallRight) && playerController.inputVector.x != 0 
            && AboveGround() && !exitingWall && Mathf.Sign(playerController.inputVector.x) == Mathf.Sign(wallRunDirection))
        {
            if(!playerController.wallRunnning)
            {
                StartWallRun();
            }    

            if(playerController.wantToWallJump)
            {
                WallJump();
                playerController.wantToWallJump = false;
            }
        }
        else if(exitingWall)
        {
            if (playerController.wallRunnning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }
        else
        {
            if (playerController.wallRunnning)
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        playerController.wallRunnning = true;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }

    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (playerController.inputVector.x != 0)
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        if (useGravity)
            rb.AddForce(transform.up * gravityCountryForce, ForceMode.Force);

        /*if (downwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        }*/
    }

    private void StopWallRun()
    {
        playerController.wallRunnning = false;
        downwardsRunning = false;
    }

    private void WallJump()
    {
        if (playerController.holding || GetComponent<LedgeGrabbing>().exitingLedge)
            return;

        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce 
            + wallNormal * wallJumpSideForce 
            + Vector3.right * playerController.inputVector.x * wallJumpForwardForce;

        rb.velocity = new Vector3(0, 0, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
