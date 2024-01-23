using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBaseState : State
{
  protected readonly int LOCOMOTION = Animator.StringToHash("Locomotion");
  protected int MOVEMENT = Animator.StringToHash("Movement");
  protected const float CROSS_FADE_TIME = 0.4f;
  protected const float DAMP_TIME = 0.3f;
  protected EnemyStateMachine stateMachine;
  public EnemyBaseState(EnemyStateMachine stateMachine) { this.stateMachine = stateMachine; }
  protected void Move(Vector3 motion, float deltaTime)
  {
    stateMachine.Controller.Move((motion + stateMachine.ForceReceiver.Movement) * deltaTime);
  }
  protected void Move(float deltaTime)
  {
    Move(Vector3.zero, deltaTime);
  }
  protected bool IsInChaseRange()
  {
    if (stateMachine.Player.GetComponent<Health>().IsDead) { return false; }
    float playerDistanceSqr = (stateMachine.Player.transform.position - stateMachine.transform.position).sqrMagnitude;
    return playerDistanceSqr <= stateMachine.PlayerChasingRange * stateMachine.PlayerChasingRange;
  }
  protected bool IsInsideOfMaxChasingRange()
  {
    if (stateMachine.Player.GetComponent<Health>().IsDead) { return false; }
    float range = (stateMachine.StartingPosition - stateMachine.transform.position).sqrMagnitude;
    return range <= stateMachine.MaxChasingRange * stateMachine.MaxChasingRange;
  }
  protected bool PlayerInsideMaxChasingRange()
  {
    if (stateMachine.Player.GetComponent<Health>().IsDead) { return false; }
    float playerDistanceSqr = (stateMachine.StartingPosition - stateMachine.Player.transform.position).sqrMagnitude;
    return playerDistanceSqr <= stateMachine.MaxChasingRange * stateMachine.MaxChasingRange;
  }
  protected void FaceLocation(Vector3 location)
  {
    if (stateMachine.Player == null) { return; }

    Vector3 lookPos = location - stateMachine.transform.position;
    lookPos.y = 0.0f;

    Quaternion lookRotation = Quaternion.LookRotation(lookPos);
    float rotationSpeed = 10f;

    stateMachine.transform.rotation = Quaternion.Slerp(
        stateMachine.transform.rotation,
        lookRotation,
        Time.deltaTime * rotationSpeed
    );
  }
  protected void MoveToStart(float deltaTime)
  {
    FaceLocation(stateMachine.StartingPosition);
    if (stateMachine.Agent.isOnNavMesh)
    {
      stateMachine.Agent.destination = stateMachine.StartingPosition;
      Move(stateMachine.Agent.desiredVelocity.normalized * stateMachine.MovementSpeed, deltaTime);
    }
    stateMachine.Agent.velocity = stateMachine.Controller.velocity;
  }
  protected bool IsBackAtStartPos()
  {
    bool status = Vector3.Distance(stateMachine.transform.position, stateMachine.StartingPosition) < 0.1f;
    if (status)
    {
      stateMachine.SetIsReturningToStart(false);
    }
    return status;
  }
  protected void MoveToPlayer(float deltaTime)
  {
    if (stateMachine.Agent.isOnNavMesh)
    {
      stateMachine.Agent.destination = stateMachine.Player.transform.position;
      Move(stateMachine.Agent.desiredVelocity.normalized * stateMachine.MovementSpeed, deltaTime);
    }
    stateMachine.Agent.velocity = stateMachine.Controller.velocity;
  }
  protected bool CanAttack()
  {
    if (stateMachine.CurrAttackCD <= 0.0f)
    {
      stateMachine.SetBaseCD(2.0f); // ! CD values must be greater than blocking and dodging animations.
      stateMachine.SetCurrAttackCD(stateMachine.BaseCD);
      return true;
    }
    else if (stateMachine.CurrAttackCD == stateMachine.BaseCD) { return true; }
    return false;
  }
}
