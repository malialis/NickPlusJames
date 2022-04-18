using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D myRB;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer mySR;

    [Header("Move and Jump Variables")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 20f;

    [Header("Ground Check Variables")]
    [SerializeField] private float groundCheckRadious = 1f;
    [SerializeField] private Transform groundPoint;
    [SerializeField] private LayerMask whatIsGround;
    private bool isOnGround;

    [Header("Fire Variables")]
    [SerializeField] private BulletController shotToFire;
    [SerializeField] private Transform firePoint;

    [Header("Abilities")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float waitAfterDashing;
    private float dashRechargeCounter;
    private float dashCounter;
    private bool canDoubleJump;

    [Header("AfterBurn Variables")]
    [SerializeField] private float afterBurnLifeTime;
    [SerializeField] private float timeBetweenAfterBurnImages;
    [SerializeField] private SpriteRenderer afterBurn;
    [SerializeField] private Color afterBurnColor;
    private float afterBurnCounter;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveAnimations();
       // Flip();
        Fire01();
        Fire02();

        if(dashCounter > 0)
        {
            dashCounter -= Time.deltaTime;
            afterBurnCounter -= Time.deltaTime;
            myRB.velocity = new Vector2(dashSpeed * transform.localScale.x, myRB.velocity.y);
            if(afterBurnCounter <= 0)
            {
                ShowAfterBurn();
            }
            dashRechargeCounter = waitAfterDashing;
        }
        else
        {
            Movements();
        }
    }

    private void FixedUpdate()
    {
        Flip();
    }


    private void Movements()
    {
        myRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, myRB.velocity.y);

        isOnGround = Physics2D.OverlapCircle(groundPoint.position, groundCheckRadious, whatIsGround);

        if (Input.GetButtonDown("Jump") && (isOnGround || canDoubleJump))
        {
            if (isOnGround)
            {
                canDoubleJump = true;
            }
            else
            {
                canDoubleJump = false;
                anim.SetTrigger("doubleJump");
            }

            myRB.velocity = new Vector2(myRB.velocity.x, jumpForce);
        }
    }

    private void MoveAnimations()
    {
        anim.SetBool("onGround", isOnGround);
        anim.SetFloat("speed", Mathf.Abs(myRB.velocity.x));
    }

    private void Flip()
    {
        if (myRB.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (myRB.velocity.x > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void Fire01()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Instantiate(shotToFire, firePoint.position, firePoint.rotation).moveDirection = new Vector2(transform.localScale.x, 0f);
            anim.SetTrigger("shotFired");
        }
    }

    private void Fire02()
    {
        if (dashRechargeCounter > 0)
        {
            dashRechargeCounter -= Time.deltaTime;
        }
        else
        {
            if (Input.GetButtonDown("Fire2"))
            {
                dashCounter = dashTime;
                ShowAfterBurn();
            }
        }
    }

    private void ShowAfterBurn()
    {
        SpriteRenderer image = Instantiate(afterBurn, transform.position, transform.rotation);
        image.sprite = mySR.sprite;
        image.transform.localScale = transform.localScale;
        image.color = afterBurnColor;

        Destroy(image.gameObject, afterBurnLifeTime);

        afterBurnCounter = timeBetweenAfterBurnImages;
    }


}



/*
 * 
 * private enum AbilityType
{
    DoubleJump,
    Dash,
    Ball,
    Bomb
}
 
[SerializeField] private AbilityType abilityType;
 
private void OnTriggerEnter2D(Collider2D other)
{
    if (!other.CompareTag("Player"))
        return;
        
    var player = other.GetComponentInParent<PlayerAbilityTracker>();
 
    switch (abilityType)
    {
        case AbilityType.DoubleJump:
            player.CanDoubleJump = true;
            break;
        case AbilityType.Dash:
            player.CanDash = true;
            break;
        case AbilityType.Ball:
            player.CanBecomeBall = true;
            break;
        case AbilityType.Bomb:
            player.CanDropBomb = true;
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }
    
    Destroy(gameObject);
}

*/

