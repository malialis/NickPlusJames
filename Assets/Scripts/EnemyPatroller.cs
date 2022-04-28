using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatroller : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Rigidbody2D theRB;
    [SerializeField] private Animator anim;

    private int currentPoint;
    private bool isFacingRight;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float waitAtPoints = 1f;
    private float waitCounter;


    // Start is called before the first frame update
    void Start()
    {
        waitCounter = waitAtPoints;

        foreach(Transform pPoint in patrolPoints)
        {
            pPoint.SetParent(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Patrolling();
    }

    private void Flip()
    {
        if (isFacingRight)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (!isFacingRight)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void Patrolling()
    {
        if (Mathf.Abs(transform.position.x - patrolPoints[currentPoint].position.x) > 0.1f)
        {
            if (transform.position.x < patrolPoints[currentPoint].position.x)
            {
                theRB.velocity = new Vector2(moveSpeed, theRB.velocity.y); //to the right
                isFacingRight = true;
                Flip();
            }
            else
            {
                theRB.velocity = new Vector2(-moveSpeed, theRB.velocity.y); //to the left
                isFacingRight = false;
                Flip();
            }

            if(transform.position.y < patrolPoints[currentPoint].position.y -0.5f && theRB.velocity.y < 0.1f)
            {
                theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
            }
        }
        else
        {
            theRB.velocity = new Vector2(0f, theRB.velocity.y);
            Debug.Log("I am waiting at Patrol Point " + currentPoint.ToString());
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0)
            {
                waitCounter = waitAtPoints;
                currentPoint++;

                if(currentPoint >= patrolPoints.Length)
                {
                    currentPoint = 0;
                }
            }
        }
        anim.SetFloat("moveSpeed", Mathf.Abs(theRB.velocity.x));
    }

}
