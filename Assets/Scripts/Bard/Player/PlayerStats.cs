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

    private void Start() 
    {
        currentHealth = maxHealth;        
    }

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;

        if(currentHealth >= 0.0f)
        {
            Die();
        }
    }

    private voi Die()
    {
        Instantiate(deathChunkParticles, transform.position, deathChunkParticles.transform.rotation);
        Instantiate(deathBloodParticles, transform.position, deathBloodParticles.transform.rotation);

        Destroy(gameObject, 0.5f);
    }
}
