using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Sliding : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerController playerController;

    [Header("Sliding")]
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float slideForce;
    [SerializeField] private float slideShrinkScale;

    public float slideTimer;
    private float startShrinkScale;

    public bool sliding;

    public bool wantToSlide = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();

        startShrinkScale = GetComponent<CapsuleCollider>().height;
    }

    private void Update()
    {
        if (wantToSlide && !sliding)
        {
            StartSlide();
        }

        if (!wantToSlide && sliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if(sliding)
        {
            SlidingMovement();
        }
    }

    private void StartSlide()
    {
        sliding = true;
        slideTimer = maxSlideTime;

        GetComponent<CapsuleCollider>().height -= slideShrinkScale;
       /* playerController.playerModel.transform.localPosition =
        new Vector3(0, -playerController.playerHeight / 2 + (slideShrinkScale - playerController.playerModelOffset), 0);
*/

        /*Vector2 direction = new Vector2(playerController.inputVector.x, 0);
        rb.AddForce(direction * slideForce, ForceMode.Force);
        playerController.moveSpeed = playerController.crouchSpeed;*/

        if (playerController.isGrounded)
            rb.AddForce(Vector3.down * 20f, ForceMode.Impulse);
    }

    private void SlidingMovement()
    {
        Vector2 direction = new Vector2(playerController.inputVector.x, 0);

        if ((!playerController.OnSlope() || rb.velocity.y > -0.1f) && playerController.isGrounded)
        {
            rb.AddForce(direction.normalized * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
        }
        else
        {
            rb.AddForce(playerController.GetSlopeMoveDirection(direction) * slideForce, ForceMode.Force);
            
            if(!playerController.isGrounded)
                playerController.wantToJump = false;
        }

        if (slideTimer <= 0)
            StopSlide();
    }

    public void StopSlide()
    {
        sliding = false;

        if (wantToSlide)
        {
            GetComponent<CapsuleCollider>().height = startShrinkScale - playerController.colliderShrinkScale;
           /* playerController.playerModel.transform.localPosition =
                    new Vector3(0, -playerController.playerHeight / 2 + (playerController.colliderShrinkScale - playerController.playerModelOffset), 0);
*/
            if (playerController.isGrounded)
            {
                rb.AddForce(Vector3.down * 20f, ForceMode.Impulse);   
            }

            playerController.crouching = true;
        }
        else
        {
            GetComponent<CapsuleCollider>().height = startShrinkScale;
           /* playerController.playerModel.transform.localPosition = playerController.startModelPos;
*/        }

        playerController.sliding = false;
        wantToSlide = false;
        StopCoroutine(playerController.SmoothlyLerpMoveSpeed());
    }

    public void Slide(InputAction.CallbackContext context)
    {
        /*if (context.performed)
        {
            wantToSlide = true;
        }
        if (context.canceled)
        {
            wantToSlide = false;
        }*/
    }
}
