using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockingState : PlayerBaseState
{
  private readonly int BLOCKING_TREE = Animator.StringToHash("BlockingBlendTree");
  private const float CROSS_FADE_TIME = 0.3f;
  public PlayerBlockingState(PlayerStateMachine stateMachine) : base(stateMachine) { }
  public override void Enter()
  {
    stateMachine.Animator.CrossFadeInFixedTime(BLOCKING_TREE, CROSS_FADE_TIME);
    stateMachine.ShieldHandler.EnableShield();
  }
  public override void Exit()
  {
    stateMachine.ShieldHandler.DisableShield();
  }
  public override void Tick(float deltaTime)
  {
    if (!IsControlEnabled()) { stateMachine.SwitchState(new PlayerFreeLookState(stateMachine)); }

    if (!stateMachine.Input.IsBlocking)
    {
      stateMachine.SwitchState(new PlayerFreeLookState(stateMachine, true, false));
      return;
    }

    Vector3 movement = CalculateMovement();
    Move(movement * stateMachine.WalkMovementSpeed, deltaTime);

    UpdateAnimator(deltaTime, WALK_RIGHT, WALK_FORWARD);

    if (Vector3.zero != movement) { FaceMovementDirection(deltaTime); }
  }
}