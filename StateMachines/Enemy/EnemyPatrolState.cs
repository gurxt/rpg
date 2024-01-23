using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{
  public EnemyPatrolState(EnemyStateMachine stateMachine) : base(stateMachine) { }
  private const float waypointTolerance = 2.0f;
  private int currentWaypointIdx = 0;
  public override void Enter()
  {
    stateMachine.SetMovementSpeed(1.5f);
    stateMachine.Animator.CrossFadeInFixedTime(LOCOMOTION, CROSS_FADE_TIME);
  }
  public override void Exit() { }
  public override void Tick(float deltaTime)
  {
    if (!CanAttack()) { stateMachine.CurrAttackCD -= deltaTime; }
    if (stateMachine.IsReturningToStart && PlayerInsideMaxChasingRange() && IsInsideOfMaxChasingRange())
    {
      stateMachine.IsReturningToStart = false;
      stateMachine.SwitchState(new EnemyChasingState(stateMachine));
      return;
    }
    else if (IsInChaseRange() && IsInsideOfMaxChasingRange() && !stateMachine.IsReturningToStart)
    {
      stateMachine.SwitchState(new EnemyChasingState(stateMachine));
      return;
    }
    stateMachine.Animator.SetFloat(MOVEMENT, 0.25f, DAMP_TIME, deltaTime);
    MoveToWaypoint(deltaTime);
    AtWaypoint();
  }
  private bool AtWaypoint()
  {
    float distanceToWaypoint = Vector3.Distance(stateMachine.transform.position, GetWaypoint(currentWaypointIdx));
    bool hasArrived = distanceToWaypoint < waypointTolerance;
    if (hasArrived)
    {
      currentWaypointIdx = stateMachine.PatrolPath.GetNextIndex(currentWaypointIdx);
    }
    return hasArrived;
  }
  private Vector3 GetWaypoint(int idx)
  {
    return stateMachine.PatrolPath.GetWaypoint(idx);
  }
  private void MoveToWaypoint(float deltaTime)
  {
    FaceLocation(GetWaypoint(currentWaypointIdx));
    if (stateMachine.Agent.isOnNavMesh)
    {
      stateMachine.Agent.destination = GetWaypoint(currentWaypointIdx);
      Move(stateMachine.Agent.desiredVelocity.normalized * stateMachine.MovementSpeed, deltaTime);
    }
    stateMachine.Agent.velocity = stateMachine.Controller.velocity;
  }
}
