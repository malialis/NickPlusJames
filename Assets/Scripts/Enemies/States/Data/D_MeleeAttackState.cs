using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMeleeAttackState", menuName = "Data/State Data/Melee Attack State")]
public class D_AttackState : ScriptableObject
{
    public float attackRadius = 0.5f;
    public float attackDamage = 10f;
    
    public LayerMask whatIsPlayer;
}
