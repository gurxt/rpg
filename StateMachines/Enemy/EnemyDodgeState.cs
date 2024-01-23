using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyDodgeState : EnemyBaseState
{
  private readonly int DODGE_BLEND_TREE = Animator.StringToHash("EnemyDodgeBlendTree");
  private readonly int DODGE_DIRECTION = Animator.StringToHash("DodgeDirection");
  private Vector3 direction = Vector3.zero;
  private float duration = 1.0f;

  public EnemyDodgeState(EnemyStateMachine stateMachine) : base(stateMachine) { }
  public override void Enter()
  {
    stateMachine.Animator.CrossFadeInFixedTime(DODGE_BLEND_TREE, CROSS_FADE_TIME);
    stateMachine.Health.SetInvulnerable(true);
    switch (Random.Range(0, 3))
    {
      case 0:
        stateMachine.Animator.SetFloat(DODGE_DIRECTION, 0.0f);
        direction = -Vector3.forward;
        break;
      case 1:
        stateMachine.Animator.SetFloat(DODGE_DIRECTION, 0.33f);
        direction = Vector3.forward;
        break;
      case 2:
        stateMachine.Animator.SetFloat(DODGE_DIRECTION, 0.66f);
        direction = Vector3.right;
        break;
      default:
        stateMachine.Animator.SetFloat(DODGE_DIRECTION, 1.0f);
        direction = -Vector3.left;
        break;
    }
  }
  public override void Exit() { }
  public override void Tick(float deltaTime)
  {
    if (!CanAttack()) { stateMachine.SetCurrAttackCD(stateMachine.CurrAttackCD - deltaTime); }

    FaceLocation(stateMachine.Player.transform.position);
    if (stateMachine.Agent.isOnNavMesh) { Move(direction, deltaTime); }
    stateMachine.Agent.velocity = stateMachine.Controller.velocity;

    duration -= deltaTime;
    if (duration <= 0.0f)
    {
      stateMachine.Health.SetInvulnerable(false);
      stateMachine.SwitchState(new EnemyPatrolState(stateMachine));
    }
  }
}
