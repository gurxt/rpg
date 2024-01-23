using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrawState : PlayerBaseState
{
  private readonly int DRAW = Animator.StringToHash("BowDrawArrow");
  private const float CROSS_FADE_TIME = 0.3f;
  private float duration = 0.5f;
  public PlayerDrawState(PlayerStateMachine stateMachine) : base(stateMachine) { }
  public override void Enter()
  {
    stateMachine.Animator.CrossFadeInFixedTime(DRAW, CROSS_FADE_TIME);
  }
  public override void Exit() { }
  public override void Tick(float deltaTime)
  {
    if (!IsControlEnabled()) { stateMachine.SwitchState(new PlayerBowState(stateMachine, false, false)); }

    duration -= deltaTime;

    if (duration <= 0.0f)
    {
      stateMachine.SwitchState(new PlayerBowState(stateMachine, false, true));
    }
  }
}
