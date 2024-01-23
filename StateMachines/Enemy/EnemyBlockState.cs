using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlockState : EnemyBaseState
{
  private readonly int BLOCKING = Animator.StringToHash("EnemyBlock");
  private float normalized = 1.0f;
  public EnemyBlockState(EnemyStateMachine stateMachine) : base(stateMachine) { }
  public override void Enter()
  {
    stateMachine.Animator.CrossFadeInFixedTime(BLOCKING, CROSS_FADE_TIME);
    normalized = stateMachine.Animator.GetCurrentAnimatorStateInfo(0).length;
  }
  public override void Exit()
  {
    if (stateMachine.BlockingCollider) { stateMachine.BlockingCollider.enabled = false; }
  }
  public override void Tick(float deltaTime)
  {
    if (!CanAttack()) { stateMachine.CurrAttackCD -= deltaTime; }
    FaceLocation(stateMachine.Player.transform.position);

    if (stateMachine.BlockImpact) { stateMachine.SwitchState(new EnemyBlockImpactState(stateMachine)); }

    if (stateMachine.BlockingCollider) { stateMachine.BlockingCollider.enabled = true; }
    normalized -= deltaTime;
    if (normalized <= 0.0f) { stateMachine.SwitchState(new EnemyPatrolState(stateMachine)); }
  }
}
