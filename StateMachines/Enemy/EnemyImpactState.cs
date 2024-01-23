using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyImpactState : EnemyBaseState
{
  private readonly int IMPACT_ONE = Animator.StringToHash("EnemyImpact1");
  private readonly int IMPACT_TWO = Animator.StringToHash("EnemyImpact2");
  private float duration = 1.0f;
  public EnemyImpactState(EnemyStateMachine stateMachine) : base(stateMachine)
  {
    duration = stateMachine.KnockbackDuration;
  }
  public override void Enter()
  {
    stateMachine.Animator.CrossFadeInFixedTime(
      Random.Range(0, 2) == 0 ? IMPACT_ONE : IMPACT_TWO,
      CROSS_FADE_TIME
    );
  }
  public override void Exit() { }
  public override void Tick(float deltaTime)
  {
    if (!CanAttack()) { stateMachine.CurrAttackCD -= deltaTime; }
    MoveToPlayer(deltaTime / 100);

    if (!IsInChaseRange())
    {
      stateMachine.IncreaseChasingRange();
      stateMachine.IncreaseChasingRange();
      stateMachine.IncreaseChasingRange();
      stateMachine.IncreaseChasingRange();
    }

    duration -= deltaTime;

    if (duration <= 0.0f)
    {
      stateMachine.SwitchState(new EnemyIdleState(stateMachine));
    }
  }
}
