using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D myRB;
    
    [Header("Move and Jump Variables")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 20f;

    [Header("Ground Check Variables")]
    [SerializeField] private float groundCheckRadious = 1f;
    [SerializeField] private Transform groundPoint;
    [SerializeField] private LayerMask whatIsGround;
    private bool isOnGround;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        myRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, myRB.velocity.y);

        isOnGround = Physics2D.OverlapCircle(groundPoint.position, groundCheckRadious, whatIsGround);

        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            myRB.velocity = new Vector2(myRB.velocity.x, jumpForce);
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

*/

