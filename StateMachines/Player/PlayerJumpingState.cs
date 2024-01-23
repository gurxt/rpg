using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
  private readonly int JUMP = Animator.StringToHash("Jump");
  private Vector3 momentum;
  public PlayerJumpingState(PlayerStateMachine stateMachine) : base(stateMachine) { }
  public override void Enter()
  {
    stateMachine.ForceReceiver.Jump(stateMachine.JumpForce);
    momentum = stateMachine.Controller.velocity;
    momentum.y = 0.0f;
    stateMachine.Animator.CrossFadeInFixedTime(JUMP, 0.1f);
  }
  public override void Exit() { }
  public override void Tick(float deltaTime)
  {
    if (!IsControlEnabled()) { stateMachine.SwitchState(new PlayerFreeLookState(stateMachine)); }
    Move(momentum, deltaTime);
    if (stateMachine.Controller.velocity.y <= 0.0f)
    {
      stateMachine.SwitchState(new PlayerFallingState(stateMachine));
      return;
    }
  }
}
