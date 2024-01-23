using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyBlockImpactState : EnemyBaseState
{
  private readonly int BLOCK_IMPACT = Animator.StringToHash("EnemyBlockImpact");
  private float normalized = 1.0f;
  public EnemyBlockImpactState(EnemyStateMachine stateMachine) : base(stateMachine) { }
  public override void Enter()
  {
    stateMachine.Animator.CrossFadeInFixedTime(BLOCK_IMPACT, CROSS_FADE_TIME);
    normalized = stateMachine.Animator.GetCurrentAnimatorStateInfo(0).length;
  }
  public override void Exit()
  {
    if (stateMachine.BlockingCollider) { stateMachine.BlockingCollider.enabled = false; }
  }
  public override void Tick(float deltaTime)
  {
    if (!CanAttack()) { stateMachine.SetCurrAttackCD(stateMachine.CurrAttackCD - deltaTime); }
    normalized -= deltaTime;
    if (normalized <= 0.0f)
    {
      stateMachine.SetIsBlockImpact(false);
      stateMachine.SwitchState(new EnemyPatrolState(stateMachine));
    }
  }
}
