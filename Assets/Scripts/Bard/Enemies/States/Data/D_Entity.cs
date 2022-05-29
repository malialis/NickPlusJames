using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityData", menuName = "Data/Entity Data/Base Data")]
public class D_Entity : ScriptableObject
{
    public float wallCheckDistance = 0.3f;
    public float ledgeCheckDistance = 0.4f;

    public float maxAgroDistance = 4.0f;
    public float minAgroDistance = 1.0f;

    public LayerMask whatIsPlayer;
    public LayerMask whatIsGround;
}
