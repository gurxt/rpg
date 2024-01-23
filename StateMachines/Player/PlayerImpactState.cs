using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerImpactState : PlayerBaseState
{
  private readonly int IMPACT_ONE = Animator.StringToHash("Impact1");
  private readonly int IMPACT_TWO = Animator.StringToHash("Impact2");
  private readonly int IMPACT_THREE = Animator.StringToHash("Impact3");
  private float duration = 1.0f;
  private const float CROSS_FADE_TIME = 0.3f;
  public PlayerImpactState(PlayerStateMachine stateMachine) : base(stateMachine)
  {
    duration = stateMachine.KnockbackDuration;
    stateMachine.GetComponent<WeaponHandler>()?.DisableWeapon();
  }
  public override void Enter()
  {
    int rand = Random.Range(0, 3);
    switch (rand)
    {
      case 0:
        stateMachine.Animator.CrossFadeInFixedTime(IMPACT_ONE, CROSS_FADE_TIME);
        break;
      case 1:
        stateMachine.Animator.CrossFadeInFixedTime(IMPACT_TWO, CROSS_FADE_TIME);
        break;
      default:
        stateMachine.Animator.CrossFadeInFixedTime(IMPACT_THREE, CROSS_FADE_TIME);
        break;
    }

  }

  public override void Exit() { }
  public override void Tick(float deltaTime)
  {
    Move(deltaTime);

    duration -= deltaTime;

    if (duration > 0.0f) { return; }

    if (stateMachine.PrimaryWeaponIndex == 1)
    {
      stateMachine.Input.IsBowDraw = false;
      stateMachine.SwitchState(new PlayerBowState(stateMachine));
    }
    else
    {
      stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
    }
  }
}
