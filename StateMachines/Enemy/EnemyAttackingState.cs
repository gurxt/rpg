using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackingState : EnemyBaseState
{
  private readonly int BASE_ATTACK = Animator.StringToHash("EnemyBaseAttack");
  private readonly int POWER_ATTACK = Animator.StringToHash("EnemyPowerAttack");
  private readonly int CURR_ATTACK_ANIMATOR;
  private readonly EnemyAttack currAttack;
  private bool hasAppliedForce;
  public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine)
  {
    int atkIdx = Random.Range(0, stateMachine.Attacks.Length);
    switch (atkIdx)
    {
      case 0:
        CURR_ATTACK_ANIMATOR = BASE_ATTACK;
        currAttack = stateMachine.Attacks[atkIdx];
        break;
      case 1:
        CURR_ATTACK_ANIMATOR = POWER_ATTACK;
        currAttack = stateMachine.Attacks[atkIdx];
        break;
      default:
        CURR_ATTACK_ANIMATOR = BASE_ATTACK;
        currAttack = stateMachine.Attacks[atkIdx];
        break;
    }
  }
  public override void Enter()
  {
    stateMachine.SetCurrAttackCD(stateMachine.CurrAttackCD - 0.01f);
    stateMachine.Animator.CrossFadeInFixedTime(CURR_ATTACK_ANIMATOR, CROSS_FADE_TIME);
    stateMachine.Weapon.SetAttack(currAttack.AttackDamage, currAttack.Knockback);
  }
  public override void Exit() { }
  public override void Tick(float deltaTime)
  {
    Move(deltaTime);
    FaceLocation(stateMachine.Player.transform.position);

    float normalizedTime = GetNormalizedTime(stateMachine.Animator, "EnemyAttack");

    if (normalizedTime >= 1.0f) { stateMachine.SwitchState(new EnemyIdleState(stateMachine)); return; }

    if (normalizedTime >= currAttack.ForceTime) { TryApplyForce(); }
  }
  private void TryApplyForce()
  {
    if (hasAppliedForce) { return; }
    stateMachine.ForceReceiver.AddForce(stateMachine.transform.forward * currAttack.Force);
    hasAppliedForce = true;
  }
}
