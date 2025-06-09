using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabbing : MonoBehaviour
{
    [Header("Ledge Detection")]
    [SerializeField] private float ledgeDetectionLength;
    [SerializeField] private LayerMask whatIsLedge;

    [Header("Ledge Grabbing")]
    [SerializeField] private float maxLedgeGrabDistance;
    [SerializeField] private float minTimeOnLedge;

    private float timeOnLedge;

    [Header("Ledge Jumping")]
    [SerializeField] private float ledgeJumpForwardForce;
    [SerializeField] private float ledgeJumpUpwardForce;

    [Header("Exiting")] 
    [SerializeField] public bool exitingLedge;
    [SerializeField] private float exitLedgeTime;
    
    public float exitLedgeTimer;

    private PlayerController playerController;
    private Rigidbody rb;

    private Transform lastLedge;
    private Transform currentLedge;

    private RaycastHit ledgeHit;

    delegate Vector3 GetPosRelatedTo(Transform transform);
    GetPosRelatedTo getPos;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        LedgeDetection();
        
    }

    private void FixedUpdate()
    {
        StateMachine();
    }

    private void StateMachine()
    {
        if(playerController.holding)
        {
            FreezeRigidBodyOnLedge();

            timeOnLedge += Time.deltaTime;

            if(timeOnLedge > minTimeOnLedge && !exitingLedge && playerController.inputVector != Vector2.zero) 
                ExitLedgeHold();

            if(playerController.wantToWallJump)
            {
                LedgeJump();
                playerController.wantToWallJump = false;
            }
        }
       
        if (exitingLedge)
        {
            if (exitLedgeTimer > 0) exitLedgeTimer -= Time.deltaTime;
            else exitingLedge = false;
        }
    }
    private void LedgeDetection()
    {
        RaycastHit _ledgeHit;

        bool ledgeDetectionZ = Physics.Raycast(transform.position + Vector3.up * (playerController.playerHeight / 2),
            Vector3.right, out ledgeHit, ledgeDetectionLength, whatIsLedge) 
            || Physics.Raycast(transform.position + Vector3.up * (playerController.playerHeight / 2),
            Vector3.left, out ledgeHit, ledgeDetectionLength, whatIsLedge);
        bool ledgeDetectionX = Physics.Raycast(transform.position + Vector3.up * (playerController.playerHeight / 2),
            Vector3.forward, out _ledgeHit, ledgeDetectionLength, whatIsLedge);

        if (ledgeDetectionZ)
        {
            getPos = GetPosRelatedToObjZ;
        }
        else if (ledgeDetectionX)
        {
            getPos = GetPosRelatedToObjX;
            ledgeHit = _ledgeHit;
        }
        else return;

        float distanceToLedge = Vector3.Distance(transform.position, getPos(ledgeHit.transform));

        if (ledgeHit.transform == lastLedge) return;

        if (distanceToLedge < maxLedgeGrabDistance && !playerController.holding 
            && !exitingLedge && playerController.inputVector.y != -1) EnterLedgeHold();
    }

    private void EnterLedgeHold()
    {
        playerController.holding = true;
        playerController.restricted = true;

        currentLedge = ledgeHit.transform;
        lastLedge = ledgeHit.transform;

        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }

    private Vector3 GetPosRelatedToObjZ(Transform transform)
    {
        return new Vector3(transform.position.x, transform.position.y, this.transform.position.z);
    }
    private Vector3 GetPosRelatedToObjX(Transform transform)
    {
        return new Vector3(this.transform.position.x, transform.position.y, this.transform.position.z);
    }

    private void FreezeRigidBodyOnLedge()
    {
        rb.useGravity = false;

        Vector3 directionToLedge = getPos(currentLedge.transform) - transform.position;
        float distanceToLedge = Vector3.Distance(transform.position, getPos(currentLedge.transform));

        if(distanceToLedge > 1f)
        {
            if (rb.velocity.magnitude < playerController.moveToLedgeSpeed)
                rb.AddForce(directionToLedge.normalized * playerController.moveToLedgeSpeed * 1000f * Time.deltaTime);
        }
        else
        {
            if (!playerController.freeze)
                playerController.freeze = true;
        }

        if (distanceToLedge > maxLedgeGrabDistance) ExitLedgeHold();
    }

    private void ExitLedgeHold()
    {
        exitLedgeTimer = exitLedgeTime;
        exitingLedge = true;
        

        playerController.holding = false;
        playerController.freeze = false;
        playerController.restricted = false;

        timeOnLedge = 0;

        rb.useGravity = true;
        Invoke(nameof(ResetLastLedge), 1f);
    }

    private void ResetLastLedge()
    {
        lastLedge = null;
    }

    private void LedgeJump()
    {
        ExitLedgeHold();

        Invoke(nameof(DelayJumpForce), 0.05f);
    }

    private void DelayJumpForce()
    {
        Vector3 forceToAdd = (Vector3)playerController.inputVector * ledgeJumpForwardForce + Vector3.up * ledgeJumpUpwardForce;
        rb.velocity = Vector3.zero;
        rb.AddForce(forceToAdd, ForceMode.Impulse);
    }
}