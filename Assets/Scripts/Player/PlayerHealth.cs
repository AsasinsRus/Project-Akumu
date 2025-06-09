using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : Health
{
    [Header("UI")]
    [SerializeField] public UIHealthBarPlayer _UIHealthBarPlayer;

    private VolumeProfile postProcessing;

    protected override void OnStart()
    {
        base.OnStart();

        postProcessing = FindFirstObjectByType<Volume>().profile;
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        _UIHealthBarPlayer.UpdateHealth(currentHealth / 100);
        SetVignette();

    }

    private void SetVignette()
    {
        Vignette vignette;
        if (postProcessing.TryGet(out vignette))
        {
            if (currentHealth < 0)
                return;

            float percent = 1.0f - (currentHealth / maxHealth);
           
            vignette.intensity.value = percent * 0.5f;
        }
    }

    protected override void OnUpdateHealth()
    {
        _UIHealthBarPlayer.UpdateHealth(currentHealth / 100);
        SetVignette();
    }

    protected override void Die()
    {
        base.Die();

        var playerController = GetComponent<PlayerController>();
        playerController.isDead = true;

        var playerAnimator = GetComponentInChildren<PlayerAnimationController>();
        playerAnimator.animator.Play("Die");

        Invoke("DieMenu", 1f);
    }

    private void DieMenu()
    {
        FindFirstObjectByType<InGameMenu>().DieMenu();
    }
}
