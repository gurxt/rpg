using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class EnemyIdleState : EnemyBaseState
{
  private float remIdleDuration = 0.0f;
  public EnemyIdleState(EnemyStateMachine stateMachine, float remIdleDuration = 0.0f) : base(stateMachine)
  {
    this.remIdleDuration = remIdleDuration;
  }
  public override void Enter()
  {
    stateMachine.Animator.CrossFadeInFixedTime(LOCOMOTION, CROSS_FADE_TIME);
  }
  public override void Exit() { }
  public override void Tick(float deltaTime)
  {
    if (!CanAttack()) { stateMachine.CurrAttackCD -= deltaTime; }
    if (remIdleDuration <= 0.0f)
    {
      stateMachine.SwitchState(new EnemyPatrolState(stateMachine));
      return;
    }
    if (IsInChaseRange() && PlayerInsideMaxChasingRange())
    {
      stateMachine.SwitchState(new EnemyPatrolState(stateMachine));
      return;
    }
    remIdleDuration -= deltaTime;
    stateMachine.Animator.SetFloat(MOVEMENT, 0.0f, DAMP_TIME, deltaTime);
  }

}
