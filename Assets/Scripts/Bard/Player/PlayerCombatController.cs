using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private bool combatEnabled;
    [SerializeField] private bool gotInput;
    [SerializeField] private float inputTimer;
    [SerializeField] private Transform attack01HitBoxPosition;
    [SerializeField] private LayerMask whatIsDamagable;
    [SerializeField] private float attack01Radious;
    [SerializeField] private float attack01Damage;
    [SerializeField] private float stunDamageAmount = 1f;

    private bool isAttacking;
    private bool isFirstAttack;

    private float lastInputTime = Mathf.NegativeInfinity;
    private AttackDetails attackDetails;
    private Animator anim;

    private PlayerController PC;
    private PlayerStats PS;


    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combatEnabled);
        PC = GetComponent<PlayerController>();
        PS = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        CheckCombatInput();
        CheckAttacks();
    }


    private void CheckCombatInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (combatEnabled)
            {
                //attempt combat
                gotInput = true;
                lastInputTime = Time.time;
            }
        }
    }

    private void CheckAttacks()
    {
        if (gotInput)
        {
            //peform attack 1
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;
                anim.SetBool("attack01", true);
                anim.SetBool("firstAttack", isFirstAttack);
                anim.SetBool("isAttacking", isAttacking);
            }
        }

        if(Time.time >= lastInputTime + inputTimer)
        {
            //wait for new input
            gotInput = false;
        }
    }

    private void CheckAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack01HitBoxPosition.position, attack01Radious, whatIsDamagable);

        attackDetails.damageAmount = attack01Damage;
        attackDetails.position = transform.position;
        attackDetails.stunDamageAmount = stunDamageAmount;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("Damage", attackDetails);
            //instaniate hit particles
        }
    }

    private void Damage(AttackDetails attackDetails)
    {
        if(!PC.GetDashStatus())
        {
            int direction;

            PS.DecreaseHealth(attackDetails.damageAmount);

            if(attackDetails.position.x < transform.position.x)
        {
                direction = 1;
        }
        else
        {
                direction = -1;
        }

        PC.KnockBack(direction);
        }
        
    }

    private void FinishAttack01()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack01", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack01HitBoxPosition.position, attack01Radious);
    }

}
