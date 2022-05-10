using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityUnlock : MonoBehaviour
{
    public bool unlockDoubleJump;
    public bool unlockDash;
    public bool unlockBecomeBall;
    public bool unlockBombs;

    [SerializeField] private GameObject pickUpEffect;
    [SerializeField] private string unlockMessage;
    [SerializeField] private TMP_Text unlockText;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            PlayerAbilityTracker player = other.GetComponentInParent<PlayerAbilityTracker>();

            if (unlockDoubleJump)
            {
                player.canDoubleJump = true;
            }
            if (unlockDash)
            {
                player.canDash = true;
            }

            if (unlockBecomeBall)
            {
                player.canBecomeBall = true;
            }

            if (unlockBombs)
            {
                player.canBomb = true;
            }

            Instantiate(pickUpEffect, transform.position, transform.rotation);

            unlockText.transform.parent.SetParent(null);
            unlockText.transform.parent.position = transform.position;

            unlockText.text = unlockMessage;
            unlockText.gameObject.SetActive(true);


            Destroy(unlockText.transform.parent.gameObject, 5f);
            Destroy(gameObject);
        }
    }


}
