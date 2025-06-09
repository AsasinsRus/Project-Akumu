using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo : MonoBehaviour
{
    [SerializeField] private List<AttackSO> combo;
    [SerializeField] public float AtackCD;
    [SerializeField] private float ComboCD;
    [SerializeField] public Animator animator;

    public int animationLayer;

    public float lastClickedTime;
    private float lastComboEnd;
    private int comboCounter;

    // Start is called before the first frame update
    void Start()
    {
        animationLayer = 2;
    }

    // Update is called once per frame
    void Update()
    {
        ExitAttack();
    }

    public AttackSO Attack()
    {
        if(Time.time - lastComboEnd > ComboCD && comboCounter <= combo.Count)
        {
            CancelInvoke("ExitCombo");

            if(Time.time - lastClickedTime >= AtackCD)
            {
                animator.runtimeAnimatorController = combo[comboCounter].animatorOV;
                animator.SetLayerWeight(animationLayer, 1);
                animator.Play("Attack", animationLayer, 0);

                comboCounter++;
                lastClickedTime = Time.time;

                if(comboCounter + 1 > combo.Count)
                {
                    comboCounter = 0;
                }

                return combo[comboCounter];
            }
        }

        return null;
    }

    private void ExitAttack()
    {
        if(animator.GetCurrentAnimatorStateInfo(2).normalizedTime > 0.9
            && animator.GetCurrentAnimatorStateInfo(2).IsTag("Attack"))
        {
            Invoke("ExitCombo", 1);
        }
    }

    private void ExitCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
        animator.SetLayerWeight(animationLayer, 0);
    }

    public bool IsAttackInCD()
    {
        return Time.time - lastClickedTime < AtackCD;
    }
}
