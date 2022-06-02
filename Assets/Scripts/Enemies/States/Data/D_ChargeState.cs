using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewChargeState", menuName = "Data/State Data/Charge State")]
public class D_ChargeState : ScriptableObject
{
    public float chargeSpeed = 6.0f;
    public float chargeTime = 2.0f;
}
