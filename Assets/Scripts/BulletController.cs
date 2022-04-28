using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private int bulletDamageAmount;
    [SerializeField] private Rigidbody2D myRB;
    [SerializeField] private GameObject impactSFX;

    public Vector2 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        myRB.velocity = moveDirection * bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy")
        {
            other.GetComponent<EnemyHealthController>().DamageEnemy(bulletDamageAmount);
        }

        if(impactSFX != null)
        {
            Instantiate(impactSFX, transform.position, Quaternion.identity);
        }
        //add audio impact latter
        Destroy(gameObject);
        Debug.Log("I hit something and blew up");
    }

    private void OnBecameInvisible()
    {
        if (impactSFX != null)
        {
            Instantiate(impactSFX, transform.position, Quaternion.identity);
        }
        Destroy(gameObject, 3f);
    }

}
