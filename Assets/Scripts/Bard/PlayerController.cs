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

    private float movementInputDirection;
    private int facingDirection = 1;

    [Header("Movement Related")]
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float jumpForce = 16.0f;
    [SerializeField] private float movementForceInAir = 25.0f;
    [SerializeField] private float airDragMultiplier = 1.0f;
    [SerializeField] private float wallSlidingSpeed = 5.0f;

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

    private int amountOfJumpsRemaining;

    [Header("Bools and such")]
    private bool isFacingRight = true;
    private bool isWalking = false;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canJump;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color deactiveColor = Color.red;


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
        CheckIfWallSliding();
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
            Jump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }

    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadious, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsWall);
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
        if((isGrounded && rb.velocity.y <= 0.01f) || isWallSliding)
        {
            amountOfJumpsRemaining = amountOfJumps;
        }
        if(amountOfJumpsRemaining <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
        
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }


    #endregion

    private void ApplyMovement()
    {
        if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else
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

    private void Jump()
    {
        if (canJump && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsRemaining--;
        }
        else if(isWallSliding && movementInputDirection == 0 && canJump)
        {
            //wall hop
            isWallSliding = false;
            amountOfJumpsRemaining--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);

            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        else if((isWallSliding || isTouchingWall) && movementInputDirection != 0 && canJump)
        {
            //wall jump

            isWallSliding = false;
            amountOfJumpsRemaining--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);

            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        
    }



    private void Flip()
    {
        if (!isWallSliding)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;

            transform.Rotate(0.0f, 180f, 0f);
        }
    }


    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadious);
       // Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));

        if (isTouchingWall)
        {
            Gizmos.color = activeColor;
            Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
        }
        else
        {
            Gizmos.color = deactiveColor;
            Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
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
