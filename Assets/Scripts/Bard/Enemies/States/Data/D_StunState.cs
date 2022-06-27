using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStunStateData", menuName = "Data/State Data/Stun State")]
public class D_StunState : ScriptableObject
{
    public float stunKnockBackTime = 3f;
    public float knockBacktime = 0.25f;
    public Vector2 stunKnockBackAngle;
    public float stunKnockBackSpeed = 20f;
}
