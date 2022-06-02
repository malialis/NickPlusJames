using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDummyController : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool applyKnockBack;
    [SerializeField] private float knockBackSpeedX, knockBackSpeedY, knockBackDuration;
    [SerializeField] private float knockBackDeathSpeedX, knockBackDeathSpeedY, deathSpeedTorque;

    private PlayerController pc;
    private int playerFacingDirection;
    private bool playerOnLeft;
    private bool knockBack;
    private float knockBackStart;

    private GameObject aliveGO;
    private GameObject brokenTopGO;
    private GameObject brokenBottomGO;
    [SerializeField] private GameObject hitParticles;

    private Rigidbody2D rbAlive;
    private Rigidbody2D rbBrokenTop;
    private Rigidbody2D rbBrokenBottom;

    private Animator aliveAnim;

    private void Awake()
    {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();

        aliveGO = transform.Find("Alive").gameObject;
        brokenTopGO = transform.Find("BrokenTop").gameObject;
        brokenBottomGO = transform.Find("BrokenBottom").gameObject;

    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        
        aliveAnim = aliveGO.GetComponent<Animator>();

        rbAlive = aliveGO.GetComponent<Rigidbody2D>();
        Debug.Log("I have found alive GO " + rbAlive.name);
        rbBrokenTop = rbBrokenTop.GetComponent<Rigidbody2D>();
        Debug.Log("I have found brokenTop GO " + rbBrokenTop.name);
        rbBrokenBottom = rbBrokenBottom.GetComponent<Rigidbody2D>();

        aliveGO.SetActive(true);
        brokenTopGO.SetActive(false);
        brokenBottomGO.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckKnockBack();
    }

    private void Damage(float amount)
    {
        currentHealth -= amount;
        playerFacingDirection = pc.GetFacingDirection();

        Instantiate(hitParticles, aliveGO.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));

        if(playerFacingDirection == 1)
        {
            playerOnLeft = true;
        }
        else
        {
            playerOnLeft = false;
        }

        aliveAnim.SetBool("playerOnLeft", playerOnLeft);
        aliveAnim.SetTrigger("damage");

        if(applyKnockBack && currentHealth > 0.0f)
        {
            //knockback function
            KnockBack();
        }

        if(currentHealth < 0.0f)
        {
            //die
            Die();
        }
    }

    private void KnockBack()
    {
        knockBack = true;
        knockBackStart = Time.time;
        rbAlive.velocity = new Vector2(knockBackSpeedX * playerFacingDirection, knockBackSpeedY);
    }

    private void CheckKnockBack()
    {
        if(Time.time >= knockBackStart + knockBackDuration && knockBack)
        {
            knockBack = false;
            rbAlive.velocity = new Vector2(0.0f, rbAlive.velocity.y);
        }
    }

    private void Die()
    {
        aliveGO.SetActive(false);
        brokenTopGO.SetActive(true);
        brokenBottomGO.SetActive(true);

        brokenTopGO.transform.position = aliveGO.transform.position;
        brokenBottomGO.transform.position = aliveGO.transform.position;

        rbBrokenBottom.velocity = new Vector2(knockBackSpeedX * playerFacingDirection, knockBackSpeedY);
        rbBrokenTop.velocity = new Vector2(knockBackDeathSpeedX * playerFacingDirection, knockBackDeathSpeedY);
        rbBrokenTop.AddTorque(deathSpeedTorque * -playerFacingDirection, ForceMode2D.Impulse);
    }


}
