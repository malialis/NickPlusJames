using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Properties")]
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;

    [Header("Private Direction Related")]
    private float movementInputDirection;
    private int facingDirection = 1;
    private int lastWallJumpDirection;

    [Header("Movement Related")]
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float jumpForce = 16.0f;
    [SerializeField] private float movementForceInAir = 25.0f;
    [SerializeField] private float airDragMultiplier = 1.0f;
    [SerializeField] private float wallSlidingSpeed = 5.0f;
    private float turnTimer;
    [SerializeField] private float turnTimerSet = 0.1f;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float distanceBetweenDashImages;
    [SerializeField] private float dashCoolDown;
    private float dashTimeRemaining;
    private float lastImageXPosition;
    private float lastDash = -100f;

    [Header("Ground Checks and Jump Related")]
    [SerializeField] private float groundCheckRadious = 1.0f;
    [SerializeField] private float wallCheckDistance = 1.0f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private int amountOfJumps = 1;
    [SerializeField] private float variableJumpHeightMultiplier = 0.5f;
    [SerializeField] private Vector2 wallHopDirection;
    [SerializeField] private Vector2 wallJumpDirection;
    [SerializeField] private float wallHopForce = 5.0f;
    [SerializeField] private float wallJumpForce = 10.0f;
    private float jumpTimer;
    private float wallJumpTimer;
    private int amountOfJumpsRemaining;
    [SerializeField] private float wallJumpTimerSet = 0.5f;
    [SerializeField] private float jumpTimerSet = 0.15f;


    [Header("Ledge Climb related")]
    private Vector2 ledgePositionBottom;
    private Vector2 ledgePosition01;
    private Vector2 ledgePosition02;
    [SerializeField] private float ledgeClimbXOffset01 = 0.0f;
    [SerializeField] private float ledgeClimbXOffset02 = 0.0f;
    [SerializeField] private float ledgeClimbYOffset01 = 0.0f;
    [SerializeField] private float ledgeClimbYOffset02 = 0.0f;


    [Header("Bools and such")]
    private bool isFacingRight = true;
    private bool isWalking = false;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canJump;
    private bool canNormalJump;
    private bool canWallJump;
    private bool isAttemptingToJump;
    private bool checkJumpMultiplier;
    private bool canMove;
    private bool canFlip;
    private bool hasWallJumped;
    private bool isTouchingLedge;
    private bool canClimbLedge = false;
    private bool ledgeDetected;
    private bool isDashing;
    private bool knockBack;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color deactiveColor = Color.red;

    private float knockBackStartTime;
    [SerializeField] private float knockBackDuration;
    [SerializeField] private Vector2 knockBackSpeed;
    


    // Start is called before the first frame update
    void Start()
    {
        isWalking = false;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpsRemaining = amountOfJumps;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();        
        CheckIfCanJump();
        CheckJump();
        CheckIfCanDash();
        CheckIfWallSliding();
        CheckLedgeClimb();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }


    #region Checks

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if(isGrounded || (amountOfJumpsRemaining > 0 && !isTouchingWall))
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }

        if(Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if(!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }

        if (turnTimer >= 0)
        {
            turnTimer -= Time.deltaTime;

            if(turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }

        if (Input.GetButtonDown("Dash"))
        {
            if(Time.time >= (lastDash + dashCoolDown))
            {
                AttemptToDash();
            }

        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadious, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsWall);

        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, whatIsWall);

        if(isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePositionBottom = wallCheck.position;
        }
    }


    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (Mathf.Abs(rb.velocity.x) >= 0.01f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.01f)
        {
            //if((isGrounded && rb.velocity.y <= 0.01f) || isWallSliding)
            amountOfJumpsRemaining = amountOfJumps;
        }

        if (isTouchingWall)
        {
            canWallJump = true;
        }

        if(amountOfJumpsRemaining <= 0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
        }
        
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && movementInputDirection == facingDirection && rb.velocity.y < 0 && !canClimbLedge)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CheckLedgeClimb()
    {
        if(ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;

            if (isFacingRight)
            {
                ledgePosition01 = new Vector2(Mathf.Floor(ledgePositionBottom.x + wallCheckDistance) - ledgeClimbXOffset01, Mathf.Floor(ledgePositionBottom.y) + ledgeClimbYOffset01);
                ledgePosition02 = new Vector2(Mathf.Floor(ledgePositionBottom.x + wallCheckDistance) + ledgeClimbXOffset02, Mathf.Floor(ledgePositionBottom.y) + ledgeClimbYOffset02);
            }
            else
            {
                ledgePosition01 = new Vector2(Mathf.Ceil(ledgePositionBottom.x - wallCheckDistance) + ledgeClimbXOffset01, Mathf.Floor(ledgePositionBottom.y) + ledgeClimbYOffset01);
                ledgePosition02 = new Vector2(Mathf.Ceil(ledgePositionBottom.x - wallCheckDistance) - ledgeClimbXOffset02, Mathf.Floor(ledgePositionBottom.y) + ledgeClimbYOffset02);
            }

            canMove = false;
            canFlip = false;

            anim.SetBool("canClimbLedge", canClimbLedge);
        }

        if (canClimbLedge)
        {
            transform.position = ledgePosition01;
        }
    }

    private void CheckIfCanDash()
    {
        if (isDashing)
        {
            if(dashTimeRemaining > 0)
            {
                canMove = false;
                canFlip = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, rb.velocity.y);
                dashTimeRemaining -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXPosition) > distanceBetweenDashImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXPosition = transform.position.x;
                }
            }
            if(dashTimeRemaining <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }
        }
    }

    #endregion

    private void ApplyMovement()
    {
        if (!isGrounded && !isWallSliding && movementInputDirection == 0 && !knockBack)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else if(canMove && !knockBack)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
        /*else if(!isGrounded && !isWallSliding && movementInputDirection != 0)
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
            rb.AddForce(forceToAdd);

            if(MathF.Abs(rb.velocity.x) > movementSpeed)
            {
                rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
            }
        }
        */
        
        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlidingSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
            }
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void CheckJump()
    {

        /*
         * else if(isWallSliding && movementInputDirection == 0 && canJump)
        {
            //wall hop
            isWallSliding = false;
            amountOfJumpsRemaining--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);

            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        */

        if(jumpTimer > 0)
        {
            //wall jump
            if(!isGrounded && isTouchingWall && movementInputDirection != 0 && movementInputDirection != -facingDirection)
            {
                WallJump();
            }
            else if (isGrounded)
            {
                NormalJump();
            }
        }
        if (isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if(wallJumpTimer > 0)
        {
            if (hasWallJumped && movementInputDirection == -lastWallJumpDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                hasWallJumped = false;
            }
            else if(wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }

    }

    private void NormalJump()
    {
        if (canNormalJump && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            amountOfJumpsRemaining--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
        }
    }

    private void WallJump()
    {
        if (canWallJump)
        {
            //if (isWallSliding || isTouchingWall) && movementInputDirection != 0 && canWallJump)
            //wall jump

            rb.velocity = new Vector2(rb.velocity.x, 0.0f);

            isWallSliding = false;
            amountOfJumpsRemaining = amountOfJumps;
            amountOfJumpsRemaining--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);

            rb.AddForce(forceToAdd, ForceMode2D.Impulse);

            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;

            turnTimer = 0;
            canMove = true;
            canFlip = true;

            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
        }
    }

    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeRemaining = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXPosition = transform.position.x;
    }

    public void KnockBack(int direction)
    {
        knockBack = true;
        knockBackStartTime = Time.time;
        rb.velocity = new Vector2(knockBackSpeed.x * direction, knockBackSpeed.y);
    }

    private void CheckKnockBack()
    {
        if(Time.time >= knockBackStartTime + knockBackDuration)
        {
            knockBack = false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
    }

    public bool GetDashStatus()
    {
        return isDashing;
    }

    public void DisableFlip()
    {
        canFlip = false;
    }

    public void EnableFlip()
    {
        canFlip = true;
    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    private void Flip()
    {
        if (!isWallSliding && canFlip && !knockBack)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;

            transform.Rotate(0.0f, 180f, 0f);
        }
    }

    public void FinishLedgeClimb()
    {
        canClimbLedge = false;
        transform.position = ledgePosition02;
        canMove = true;
        canFlip = true;
        ledgeDetected = false;
        anim.SetBool("canClimbLedge", canClimbLedge);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadious);
       // Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));

        if (isTouchingWall)
        {
            Gizmos.color = activeColor;
            Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDirection, wallCheck.position.y, wallCheck.position.z));
        }
        else
        {
            Gizmos.color = deactiveColor;
            Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDirection, wallCheck.position.y, wallCheck.position.z));
        }

        if (isTouchingLedge)
        {
            Gizmos.color = activeColor;
            Gizmos.DrawLine(ledgeCheck.position, new Vector3(ledgeCheck.position.x + wallCheckDistance * facingDirection, ledgeCheck.position.y, ledgeCheck.position.z));
        }
        else
        {
            Gizmos.color = deactiveColor;
            Gizmos.DrawLine(ledgeCheck.position, new Vector3(ledgeCheck.position.x + wallCheckDistance * facingDirection, ledgeCheck.position.y, ledgeCheck.position.z));
        }

        if (isGrounded)
        {
            Gizmos.color = activeColor;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadious);
        }
        else
        {
            Gizmos.color = deactiveColor;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadious);
        }

        Gizmos.DrawLine(ledgePosition01 * facingDirection, ledgePosition02);
    }


}

/*


n the jump() delete this:
        if (!isGrounded && !isWallSliding && movementInputDirection != 0)
        {
    Vector2 force = new Vector2(Time.fixedDeltaTime * airpullback * movementInputDirection, 0);
    rb.AddForce(force);
    if (Mathf.Abs(rb.velocity.x) > horizontalMovement)
    {
        rb.velocity = new Vector2(horizontalMovement * movementInputDirection, rb.velocity.y);
    }
}
and put this under jump():
        if (!isGrounded && !isWallSliding && movementInputDirection == 1 && facingRight)
        {
    if (rb.velocity.x >= horizontalMovement)
    {
    }
    else
    {
        Vector2 force = new Vector2(Time.fixedDeltaTime * airpullback, 0);
        rb.AddForce(force);
    }
}
        if (!isGrounded && !isWallSliding && movementInputDirection == -1 && !facingRight)
        {
    if (Mathf.Abs(rb.velocity.x) >= horizontalMovement)
    {
    }
    else
    {
        Vector2 force = new Vector2(Time.fixedDeltaTime * airpullback * -1, 0);
        rb.AddForce(force);
    }
}
        if (!isGrounded && !isWallSliding && ((movementInputDirection == 1 && !facingRight) || (movementInputDirection == -1 && facingRight)))
        {
    Vector2 force = new Vector2(Time.fixedDeltaTime * airpullback * movementInputDirection, 0);
    rb.AddForce(force);
}

*/
