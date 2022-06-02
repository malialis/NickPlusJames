using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    private enum State
    {
        Moving,
        KnockBack,
        Dead
    }

    private State currentState;

    private bool groundDetected;
    private bool wallDetected;

    [SerializeField] private float groundCheckDistance, wallCheckDistance, movementSpeed, maxHealth, currentHealth, knockBackDurration, lastTouchDamageTime, touchDamageCoolDown, touchDamage, touchDamageWidth, touchDamageHeight;

    private float knockBackStartTime;
    private float[] attackDetails = new float[2];

    [SerializeField] private LayerMask whatIsGround, whatIsWall, whatIsPlayer;

    private int facingDirection, damageDirection;

    private GameObject aliveGo;
    [SerializeField] private GameObject hitParticles, deathChunkParticles, deathBloodParticles;
    private Rigidbody2D aliveRb;
    private Animator anim;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform touchDmgCheck;

    private Vector2 movement, touchDmgBotLeft, touchDmgTopRight;

    [SerializeField] private Vector2 knockBackSpeed;


    private void Start()
    {
        aliveGo = transform.Find("Alive").gameObject;
        aliveRb = aliveGo.GetComponent<Rigidbody2D>();
        anim = aliveGo.GetComponent<Animator>();

        facingDirection = 1;

        currentHealth = maxHealth;
    }
    private void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateWalkingState();
                break;
            case State.KnockBack:
                UpdateKnockBackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;

        }
    }

    //walking State
    private void EnterWalkingState()
    {

    }

    private void UpdateWalkingState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsWall);

        CheckTouchDamage();

        if (!groundDetected || wallDetected)
        {
            //flip
            Flip();
        }
        else
        {
            //move
            movement.Set(movementSpeed * facingDirection, aliveRb.velocity.y);
            aliveRb.velocity = movement;
        }
    }

    private void ExitWalkingState()
    {

    }

    //KnockBack State
    private void EnterKnockBackState()
    {
        knockBackStartTime = Time.time;
        movement.Set(knockBackSpeed.x * damageDirection, knockBackSpeed.y);
        aliveRb.velocity = movement;
        anim.SetBool("knockBack", true);
    }

    private void UpdateKnockBackState()
    {
        if (Time.time >= knockBackStartTime + knockBackDurration)
        {
            SwitchState(State.Moving);
        }
    }

    private void ExitKnockBackState()
    {
        anim.SetBool("knockBack", false);
    }

    //Dead State
    private void EnterDeadState()
    {
        //spawn particles
        Instantiate(deathChunkParticles, aliveGo.transform.position, deathChunkParticles.transform.rotation);
        Instantiate(deathBloodParticles, aliveGo.transform.position, deathBloodParticles.transform.rotation);
        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }

    //other functions

    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Moving:
                ExitWalkingState();
                break;
            case State.KnockBack:
                ExitKnockBackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (state)
        {
            case State.Moving:
                EnterWalkingState();
                break;
            case State.KnockBack:
                EnterKnockBackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }

    private void Flip()
    {
        facingDirection *= -1;

        aliveGo.transform.Rotate(0f, 180.0f, 0f);

    }

    private void Damage(float[] attackDetails)
    {
        currentHealth -= attackDetails[0];

        Instantiate(hitParticles, aliveGo.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));

        if(attackDetails[1] > aliveGo.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }
        //hit particles

        if(currentHealth > 0.0f)
        {
            SwitchState(State.KnockBack);
        }
        else if(currentHealth < 0.0f)
        {
            SwitchState(State.Dead);
        }
    }

    private void CheckTouchDamage()
    {
        if(Time.time >= lastTouchDamageTime + touchDamageCoolDown)
        {
            touchDmgBotLeft.Set(touchDmgCheck.position.x - (touchDamageWidth / 2), touchDmgCheck.position.y - (touchDamageHeight / 2));
            touchDmgTopRight.Set(touchDmgCheck.position.x + (touchDamageWidth / 2), touchDmgCheck.position.y + (touchDamageHeight / 2));

            Collider2D hit = Physics2D.OverlapArea(touchDmgBotLeft, touchDmgTopRight, whatIsPlayer);
            if(hit != null)
            {
                lastTouchDamageTime = Time.time;
                attackDetails[0] = touchDamage;
                attackDetails[1] = aliveGo.transform.position.x;
                hit.SendMessage("Damage", attackDetails);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }

}
