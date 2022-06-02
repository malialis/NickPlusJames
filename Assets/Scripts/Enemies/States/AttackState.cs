using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    
    protected bool isAnimationFinished;

    protected Transform attackPosition;

    public AttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition): base(entity, stateMachine, animBoolName)
    {
        this.attackPosition = attackPosition;
    }

    public override void DoChecks()
    {
        base.DoChecks();

    }

    public override void Enter()
    {
        base.Enter();
        
        entity.animToStateMachine.attackState = this;
        entity.SetVelocity(0f);
        isAnimationFinished = false;
              
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public virtual void TriggerAttack()
    {

    }

    public virtual void FinishAttack()
    {
        isAnimationFinished = true;
    }
}
