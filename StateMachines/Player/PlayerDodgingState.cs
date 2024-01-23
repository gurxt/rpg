using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgingState : PlayerBaseState
{
  private readonly int DODGE_BLEND_TREE_HASH = Animator.StringToHash("DodgingBlendTree");
  private readonly int DODGE_RIGHT_HASH = Animator.StringToHash("DodgeRight");
  private readonly int DODGE_FORWARD_HASH = Animator.StringToHash("DodgeForward");
  private Vector3 dodgingDirectionInput;
  private Vector3 camForward;
  private Vector3 camRight;
  private float remainingDodgeDuration;
  public PlayerDodgingState(PlayerStateMachine stateMachine, Vector3 dodgingDirectionInput) : base(stateMachine)
  {
    this.dodgingDirectionInput = dodgingDirectionInput;
    camForward = stateMachine.MainCameraTransform.forward;
    camRight = stateMachine.MainCameraTransform.right;
  }
  public override void Enter()
  {
    remainingDodgeDuration = stateMachine.DodgeDuration;

    stateMachine.Animator.SetFloat(DODGE_RIGHT_HASH, dodgingDirectionInput.x);
    stateMachine.Animator.SetFloat(DODGE_FORWARD_HASH, dodgingDirectionInput.y);
    stateMachine.Animator.CrossFadeInFixedTime(DODGE_BLEND_TREE_HASH, 0.1f);
    stateMachine.Health.SetInvulnerable(true);
  }
  public override void Exit() { }
  public override void Tick(float deltaTime)
  {
    if (!IsControlEnabled()) { stateMachine.SwitchState(new PlayerFreeLookState(stateMachine)); }

    Vector3 forward = stateMachine.DodgeLength / stateMachine.DodgeDuration * camForward * dodgingDirectionInput.y;
    Vector3 right = stateMachine.DodgeLength / stateMachine.DodgeDuration * camRight * dodgingDirectionInput.x;
    forward.y = 0f;
    right.y = 0f;

    Move((right + forward) * stateMachine.WalkMovementSpeed, deltaTime);

    remainingDodgeDuration -= deltaTime;

    if (remainingDodgeDuration <= 0f)
    {
      SwitchToBowOrFreeLookState();
      stateMachine.Health.SetInvulnerable(false);
    }
  }
}
