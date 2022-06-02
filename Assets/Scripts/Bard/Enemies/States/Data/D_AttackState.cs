using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackState", menuName = "Data/State Data/Attack State")]
public class D_AttackState : ScriptableObject
{
    public float chargeSpeed = 6.0f;
    public float chargeTime = 2.0f;
}
