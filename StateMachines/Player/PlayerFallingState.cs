using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingState : PlayerBaseState
{
  private readonly int FALL = Animator.StringToHash("Fall");
  private Vector3 momentum;
  public PlayerFallingState(PlayerStateMachine stateMachine) : base(stateMachine) { }
  public override void Enter()
  {
    momentum = stateMachine.Controller.velocity;
    momentum.y = 0.0f;
    stateMachine.Animator.CrossFadeInFixedTime(FALL, 0.1f);
  }
  public override void Exit() { }
  public override void Tick(float deltaTime)
  {
    if (!IsControlEnabled()) { stateMachine.SwitchState(new PlayerFreeLookState(stateMachine)); }
    Move(momentum, deltaTime);
    if (stateMachine.Controller.isGrounded)
    {
      SwitchToBowOrFreeLookState();
    }
  }
}
