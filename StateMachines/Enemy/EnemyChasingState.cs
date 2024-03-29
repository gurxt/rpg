using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyChasingState : EnemyBaseState
{
  public EnemyChasingState(EnemyStateMachine stateMachine) : base(stateMachine) { }
  public override void Enter()
  {
    stateMachine.SetMovementSpeed(3.5f);
    stateMachine.Animator.CrossFadeInFixedTime(LOCOMOTION, CROSS_FADE_TIME);
  }
  public override void Exit()
  {
    if (stateMachine.Agent.isOnNavMesh)
    {
      stateMachine.Agent.ResetPath();
      stateMachine.Agent.velocity = Vector3.zero;
    }
  }
  public override void Tick(float deltaTime)
  {
    if (!CanAttack()) { stateMachine.SetCurrAttackCD(stateMachine.CurrAttackCD - deltaTime); }

    MoveToPlayer(deltaTime);
    FaceLocation(stateMachine.Player.transform.position);

    stateMachine.Animator.SetFloat(MOVEMENT, 0.50f, DAMP_TIME, deltaTime);

    if (stateMachine.Player.GetComponent<Health>().GetHealth() == 0) { stateMachine.SwitchState(new EnemyPatrolState(stateMachine)); }

    if (IsInChaseRange() && !IsInsideOfMaxChasingRange())
    {
      stateMachine.SetIsReturningToStart(true);
      stateMachine.SwitchState(new EnemyIdleState(stateMachine, 3.0f));
    }
    else if (!IsInChaseRange() && IsInsideOfMaxChasingRange())
    {
      stateMachine.SwitchState(new EnemyIdleState(stateMachine, 2.0f));
    }
    else if (IsInAttackRange(stateMachine.AttackRange))
    {
      if (!CanAttack() && !stateMachine.IsArcher && !stateMachine.IsMage)
      {
        switch (Random.Range(0, 3))
        {
          case 0:
            stateMachine.SwitchState(new EnemyDodgeState(stateMachine));
            break;
          case 1:
            stateMachine.SwitchState(new EnemyBlockState(stateMachine));
            break;
          default:
            stateMachine.SwitchState(new EnemyStrafeState(stateMachine));
            break;
        }
      }
      else if (!CanAttack() && IsInAttackRange(0.25f)) { stateMachine.SwitchState(new EnemyAttackingState(stateMachine)); }
      else if (stateMachine.IsArcher) { stateMachine.SwitchState(new EnemyBowState(stateMachine)); }
      else if (stateMachine.IsMage) { stateMachine.SwitchState(new EnemyMageState(stateMachine)); }
      else { stateMachine.SwitchState(new EnemyAttackingState(stateMachine)); }
    }
  }
  private bool IsInAttackRange(float range)
  {
    if (stateMachine.Player.GetComponent<Health>().IsDead) { return false; }
    float playerDistanceSqr = (stateMachine.Player.transform.position - stateMachine.transform.position).sqrMagnitude;
    return playerDistanceSqr <= stateMachine.AttackRange * range;
  }
}