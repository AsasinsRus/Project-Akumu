using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(menuName = "Attacks/Combo atack")]

public class AttackSO : ScriptableObject
{
    public AnimatorOverrideController animatorOV;
    public float damage;
    public float hitDelay;
    public float soundDelay;
    public VisualEffectAsset visualEffect;
}
