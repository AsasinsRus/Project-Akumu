using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;

    private PlayerController playerController;
    [HideInInspector] public Animator animator;

    private int velocityXHash;
    public float velocityX;
    private int velocityYHash;
    public float velocityY;

    public float wallDirection;

    private int playerInputHash;
    private int playerDirectionHash;
    private int wallDirectionHash;

    private int isGroundedHash;
    private int isDeadHash;
    private int wantToJumpHash;
    private int wantToWallJumpHash;
    private int slidingHash;
    private int climbingHash;
    private int holdingHash;

    [SerializeField] private float acceleration;
    private float smoothValue;
    private int smoothValueHash;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        animator = GetComponent<Animator>();

        velocityXHash = Animator.StringToHash("velocityX");
        velocityYHash = Animator.StringToHash("velocityY");
        playerInputHash = Animator.StringToHash("playerInput");
        playerDirectionHash = Animator.StringToHash("playerDirection");
        smoothValueHash = Animator.StringToHash("smoothValue");
        wallDirectionHash = Animator.StringToHash("wallDirection");

        isGroundedHash = Animator.StringToHash("isGrounded");
        isDeadHash = Animator.StringToHash("isDead");
        wantToJumpHash = Animator.StringToHash("wantToJump");
        wantToWallJumpHash = Animator.StringToHash("wantToWallJump");
        slidingHash = Animator.StringToHash("sliding");
        climbingHash = Animator.StringToHash("climbing");
        holdingHash = Animator.StringToHash("holding");
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
            return;

        SetVariables();

        SetWallDirectionAnimator();
        SetClimbingAnimator();
        SetHoldingAnimtor();
        SetCrouchingWeightAnimator();

        animator.SetFloat(velocityXHash, velocityX);
        animator.SetFloat(velocityYHash, velocityY);
        animator.SetFloat(playerInputHash, playerController.inputVector.x);
        animator.SetFloat(playerDirectionHash, playerController.playerDirection);
        animator.SetFloat(smoothValueHash, SmoothValue(playerController.playerDirection));

        animator.SetBool(isGroundedHash, playerController.isGrounded);
        animator.SetBool(isDeadHash, playerController.isDead);
        animator.SetBool(wantToJumpHash, playerController.wantToJump);
        animator.SetBool(wantToWallJumpHash, playerController.wantToWallJump);
        animator.SetBool(slidingHash, playerController.sliding);
    }

    private float SmoothValue(float desiredValue)
    {
        if (desiredValue != smoothValue)
        {
            if (desiredValue > smoothValue)
            {
                smoothValue += Time.deltaTime * acceleration;

                if (smoothValue > desiredValue)
                    smoothValue = desiredValue;
            }
            else if (desiredValue < smoothValue)
            {
                smoothValue -= Time.deltaTime * acceleration;

                if (smoothValue < desiredValue)
                    smoothValue = desiredValue;
            }
        }

        return smoothValue;
    }

    private void SetVariables()
    {
        velocityX = player.GetComponent<Rigidbody>().velocity.x / playerController.sprintSpeed;
        velocityY = player.GetComponent<Rigidbody>().velocity.y;
        wallDirection = GetComponentInParent<Climbing>().wallDirection;
    }

    private void SetCrouchingWeightAnimator()
    {
        if (playerController.crouching && playerController.isGrounded)
        {
            animator.SetLayerWeight(1, 1);
        }
        else
        {
            animator.SetLayerWeight(1, 0);
        }
    }

    private void SetHoldingAnimtor()
    {
        if (!playerController.climbing)
            animator.SetBool(holdingHash, playerController.holding);
    }

    private void SetClimbingAnimator()
    {
        if (!playerController.crouching)
            animator.SetBool(climbingHash, playerController.climbing);
    }

    private void SetWallDirectionAnimator()
    {
        if (playerController.climbing && 
            (wallDirection == 0 && playerController.inputVector.y != 0 ||
            wallDirection != 0 && playerController.inputVector.x != 0)
            || playerController.holding)
            animator.SetFloat(wallDirectionHash, GetComponentInParent<Climbing>().wallDirection);
    }
}
