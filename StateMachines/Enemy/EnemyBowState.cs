using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBowState : EnemyBaseState
{
  private readonly int BOW_BLEND_TREE = Animator.StringToHash("EnemyBowBlendTree");
  private readonly int BOW_STATE = Animator.StringToHash("BowState");
  private readonly float CROSS_FADE_DURATION = 0.3f;
  private float duration = 0.0f;
  private bool hasFired = false;
  public EnemyBowState(EnemyStateMachine stateMachine) : base(stateMachine) { }
  public override void Enter()
  {
    stateMachine.Animator.CrossFadeInFixedTime(BOW_BLEND_TREE, CROSS_FADE_DURATION);
    stateMachine.Animator.SetFloat(BOW_STATE, 0.0f);
  }
  public override void Exit() { }
  public override void Tick(float deltaTime)
  {
    FaceLocation(stateMachine.Player.transform.position);
    duration += deltaTime;
    stateMachine.Animator.SetFloat(BOW_STATE, duration, 0.1f, deltaTime);
    if (duration >= 0.5f && !hasFired)
    {
      stateMachine.CurrAttackCD -= 0.01f;
      UnityEngine.GameObject arrowInstance = UnityEngine.GameObject.Instantiate(
        stateMachine.EnemyArrowProjectile,
        stateMachine.RightHand.transform.position,
        Quaternion.LookRotation((stateMachine.Player.transform.position + Vector3.up - stateMachine.RightHand.transform.position).normalized)
      );
      hasFired = true;
    }
    if (duration >= 1.0f) { stateMachine.SwitchState(new EnemyPatrolState(stateMachine)); }
  }
}
