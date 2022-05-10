using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldPlayerController : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private Rigidbody2D myRB;
    [SerializeField] private Animator anim;
    [SerializeField] private Animator ballAnim;
    [SerializeField] private SpriteRenderer mySR;
    [SerializeField] private GameObject standing;
    [SerializeField] private GameObject morphBall;
    [SerializeField] private float InputDirection;
    private PlayerAbilityTracker abilityTracker;


    [Header("Animation facing and running")]
    private bool isRunning = false;
    private bool isFacingRight = true;

    [Header("Move and Jump Variables")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    static public int availableJumps = 1;
    private int availableJumpsRemaining;
    private bool canJump;

    [Header("Ground Check Variables")]
    [SerializeField] private float groundCheckRadious = 1f;
    [SerializeField] private Transform groundPoint;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    [Header("Fire Variables")]
    [SerializeField] private BulletController shotToFire;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform bombPoint;
    [SerializeField] private GameObject bomb;

    [Header("Abilities")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float waitAfterDashing;
    private float dashRechargeCounter;
    private float dashCounter;
    private bool canDoubleJump;
    [SerializeField] private float waitToBallMode;
    private float ballModeCounter;

    [Header("AfterBurn Variables")]
    [SerializeField] private float afterBurnLifeTime;
    [SerializeField] private float timeBetweenAfterBurnImages;
    [SerializeField] private SpriteRenderer afterBurn;
    [SerializeField] private Color afterBurnColor;
    private float afterBurnCounter;

    [Header("Wall Jump")]
    static public bool canWallJump = false;
    [SerializeField] private float wallSlidingSpeed = -0.45f;
    [SerializeField] private float verticalWallForce;
    [SerializeField] private float wallJumpTime;
    [SerializeField] private float wallCheckRadious;
    [SerializeField] private bool isTouchingWall;
    //[SerializeField] private bool wallHold = false;
    [SerializeField] private bool wallJumping;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private LayerMask whatIsWallLayer;



    // Start is called before the first frame update
    void Start()
    {
        abilityTracker = GetComponent<PlayerAbilityTracker>();

        isFacingRight = true;
        availableJumpsRemaining = availableJumps;
        //wallHold = false;
    }

    // Update is called once per frame
    void Update()
    {
        Fire01();
        Fire02();

        CheckInput();
        CheckAbilities();
        CheckMovementDirection();
        UpdateAnimation();
        CheckIfCanJump();
        //CheckIfFalling();

        MorphBall();
        Dash();

    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckEnvironment();
    }


    private void Movements()
    {
        myRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, myRB.velocity.y);

        isGrounded = Physics2D.OverlapCircle(groundPoint.position, groundCheckRadious, whatIsGround);

        if (Input.GetButtonDown("Jump") && (isGrounded || canDoubleJump))
        {
            if (isGrounded)
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
        if (standing.activeSelf)
        {
            anim.SetBool("onGround", isGrounded);
            anim.SetFloat("speed", Mathf.Abs(myRB.velocity.x));
        }

        if (morphBall.activeSelf)
        {
            ballAnim.SetFloat("moveSpeed", Mathf.Abs(myRB.velocity.x));
        }
    }

    /*
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
    */
    private void CheckInput()
    {
        InputDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void ApplyMovement()
    {
        myRB.velocity = new Vector2(moveSpeed * InputDirection, myRB.velocity.y);
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && InputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && InputDirection > 0)
        {
            Flip();
        }
        
        if (myRB.velocity.x <= -0.1f | myRB.velocity.x >= 0.1f)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    private void UpdateAnimation()
    {
        if (standing.activeSelf)
        {
            anim.SetBool("isRunning", isRunning);
            anim.SetBool("onGround", isGrounded);
        }        

        if (morphBall.activeSelf)
        {
            ballAnim.SetBool("isRunning", isRunning);
        }

       // anim.SetBool("wallHold", wallHold);
        //anim.SetFloat("yVelocity", myRB.velocity.y);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
       
        if (isFacingRight)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (!isFacingRight)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

    }

    private void CheckEnvironment()
    {
        isGrounded = Physics2D.OverlapCircle(groundPoint.position, groundCheckRadious, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundPoint.position, groundCheckRadious);
        Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadious);
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && myRB.velocity.y <= 3)
        {
            availableJumpsRemaining = availableJumps;
        }
        if (availableJumpsRemaining <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            myRB.velocity = new Vector2(myRB.velocity.x, jumpForce);
            availableJumpsRemaining--;
        }
    }

    private void Fire01()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (standing.activeSelf)
            {
                Instantiate(shotToFire, firePoint.position, firePoint.rotation).moveDirection = new Vector2(transform.localScale.x, 0f);
                anim.SetTrigger("shotFired");
            }
            else if (morphBall.activeSelf && abilityTracker.canBomb)
            {
                Instantiate(bomb, bombPoint.position, bombPoint.rotation);
            }
           
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
            if (Input.GetButtonDown("Fire2") && standing.activeSelf && abilityTracker.canDash)
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

    private void Dash()
    {
        if (dashCounter > 0)
        {
            dashCounter -= Time.deltaTime;
            afterBurnCounter -= Time.deltaTime;
            myRB.velocity = new Vector2(dashSpeed * transform.localScale.x, myRB.velocity.y);
            if (afterBurnCounter <= 0)
            {
                ShowAfterBurn();
            }
            dashRechargeCounter = waitAfterDashing;
        }
        else
        {
            CheckInput();
        }

    }

    private void MorphBall()
    {
        if (!morphBall.activeSelf)
        {
            if (Input.GetAxisRaw("Vertical") < -0.9f && abilityTracker.canBecomeBall)
            {
                ballModeCounter -= Time.deltaTime;
                if (ballModeCounter <= 0)
                {
                    morphBall.SetActive(true);
                    standing.SetActive(false);
                }
            }
            else
            {
                ballModeCounter = waitToBallMode;
            }
        }
        else
        {
            if (Input.GetAxisRaw("Vertical") > 0.9f)
            {
                ballModeCounter -= Time.deltaTime;
                if (ballModeCounter <= 0)
                {
                    morphBall.SetActive(false);
                    standing.SetActive(true);
                }
            }
            else
            {
                ballModeCounter = waitToBallMode;
            }
        }
    }

    private void CheckAbilities()
    {
        if (abilityTracker.canDoubleJump)
        {
            availableJumpsRemaining += 1;
        }
        else
        {
            availableJumpsRemaining = 1;
        }


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




private void WallJump()
    {
        //variable jump height
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (canWallJump == true)
        {
            isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadious, whatIsWallLayer);
            if (isTouchingWall == true && !isGrounded && InputDirection != 0)
            {
                wallHold = true;
                Debug.Log("Holding the Wall");
            }
            else
            {
                wallHold = false;
            }

            if (wallHold)
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            }

            if (Input.GetButtonDown("Jump") && wallHold == true)
            {
                wallJumping = true;
                Invoke("SetWallJumpingToFalse", wallJumpTime);
            }

            if (wallJumping == true)
            {
                rb.velocity = new Vector2(rb.velocity.x, verticalWallForce);
            }

            if (Input.GetKey(KeyCode.DownArrow) && wallHold | Input.GetKey(KeyCode.S) && wallHold)
            {
                wallSlidingSpeed = 3f;
                CreateDust();
                Debug.Log("Dust is created");
            }
            else
            {
                wallSlidingSpeed = -0.45f;
            }


        }
    }

    private void SetWallJumpingToFalse()
    {
        wallJumping = false;
        availableJumpsRemaining++;
    }

    private void CreateDust()
    {
        dust.Play();
    }


private void CheckIfFalling()
    {

    }

    private void PlayerShootInput()
    {
        float shootTimeLength = 0f;
        float keyShootReleaseTimeLength = 0f;

        if(shootKey && keyShootRelease)
        {
            isShooting = true;
            keyShootRelease = false;
            shootTime = Time.time;
            //shoot the buller
        }

        if(!shootKey && !keyShootRelease)
        {
            keyShootReleaseTimeLength = Time.time - shootTime;
            keyShootRelease = true;
        }

        if (isShooting)
        {
            shootTimeLength = Time.time - shootTime;
            if(shootTimeLength >= 0.25f || keyShootReleaseTimeLength >= 0.15f)
            {
                isShooting = false;
            }
        }
    }












*/

