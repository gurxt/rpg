using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyStrafeState : EnemyBaseState
{
  private readonly int STRAFE_BLEND_TREE = Animator.StringToHash("EnemyStrafeBlendTree");
  private readonly int STRAFE_X = Animator.StringToHash("StrafeX");
  private readonly int STRAFE_Y = Animator.StringToHash("StrafeY");
  private float duration = 1.0f;
  private int x = 1;
  private int y = 1;
  public EnemyStrafeState(EnemyStateMachine stateMachine) : base(stateMachine) { }
  public override void Enter()
  {
    duration = Random.Range(0.5f, 2.0f);
    stateMachine.Animator.CrossFadeInFixedTime(STRAFE_BLEND_TREE, CROSS_FADE_TIME);
    x = GetRandom();
    y = GetRandom();
    if (x == 0 & y == 0) x = 1;
    stateMachine.Animator.SetFloat(STRAFE_X, x);
    stateMachine.Animator.SetFloat(STRAFE_Y, y);
  }
  private int GetRandom()
  {
    int rand = Random.Range(0, 2);
    if (Random.Range(0, 2) == 0) { return rand; }
    return -rand;
  }
  public override void Exit()
  {
    stateMachine.Animator.SetFloat(STRAFE_X, 0);
    stateMachine.Animator.SetFloat(STRAFE_Y, 0);
  }
  private Vector3 CalculateMovement()
  {
    Vector3 right = x * Vector3.right;
    Vector3 forward = y * Vector3.forward;

    forward.y = 0.0f;
    right.y = 0.0f;

    forward.Normalize();
    right.Normalize();

    return forward + right;
  }
  public override void Tick(float deltaTime)
  {
    duration -= deltaTime;

    if (!CanAttack()) { stateMachine.SetCurrAttackCD(stateMachine.CurrAttackCD - deltaTime); }
    else { stateMachine.SwitchState(new EnemyPatrolState(stateMachine)); }

    FaceLocation(stateMachine.Player.transform.position);

    //stateMachine.Agent.destination = CalculateMovement();
    if (stateMachine.Agent.isOnNavMesh) { Move(CalculateMovement(), deltaTime); }
    stateMachine.Agent.velocity = stateMachine.Controller.velocity;

    if (duration <= 0.0f) { stateMachine.SwitchState(new EnemyPatrolState(stateMachine)); }
  }
}
