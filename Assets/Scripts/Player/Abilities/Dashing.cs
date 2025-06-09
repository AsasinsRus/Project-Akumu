using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{
    [Header("Dashing")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashUpwardForce;
    [SerializeField] private float dashDuration;

    private Vector3 forceToApply;

    [Header("Cooldown")]
    [SerializeField] private float dashCd;

    private float dashCdTimer;

    private Rigidbody rb;
    private PlayerController playerController;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (dashCdTimer > 0)
            dashCdTimer -= Time.deltaTime;
    }

    public void Dash()
    {
        if (dashCdTimer > 0) return;
        else dashCdTimer = dashCd;

        playerController.dashing = true;

        forceToApply = Vector3.right * playerController.playerDirection * dashForce + Vector3.up * dashUpwardForce;
        rb.gameObject.layer = 10;

        Invoke(nameof(DelayedDashForce), 0.025f);
        Invoke(nameof(DelayedInteractivityActivation), 0.030f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private void DelayedDashForce()
    {
        rb.AddForce(forceToApply, ForceMode.Impulse);

        MeshTrail meshTrail = GetComponentInChildren<MeshTrail>();

        if(meshTrail != null)
        {
            meshTrail.ActivateTrail();
        }
    }

    private void DelayedInteractivityActivation()
    {
        rb.gameObject.layer = 9;
    }

    private void ResetDash()
    {
        playerController.dashing = false;
    }
}
