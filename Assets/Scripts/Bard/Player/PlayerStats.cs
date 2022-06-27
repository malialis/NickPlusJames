using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;

    [SerializeField] private GameObject deathChunkParticles;
    [SerializeField] private GameObject deathBloodParticles;

    private float currentHealth;

    private GameManager gm;

    private void Start() 
    {
        currentHealth = maxHealth;  
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();      
    }

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;

        if(currentHealth <= 0.0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Instantiate(deathChunkParticles, transform.position, deathChunkParticles.transform.rotation);
        Instantiate(deathBloodParticles, transform.position, deathBloodParticles.transform.rotation);
        gm.Respawn();
        Destroy(gameObject, 0.5f);
    }
}

