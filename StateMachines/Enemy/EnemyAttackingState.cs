using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackingState : EnemyBaseState
{
  private readonly int BASE_ATTACK = Animator.StringToHash("EnemyBaseAttack");
  private readonly int POWER_ATTACK = Animator.StringToHash("EnemyPowerAttack");
  private readonly int KICK_ATTACK = Animator.StringToHash("EnemyKickAttack");
  private readonly int CURR_ATTACK_ANIMATOR;
  private readonly EnemyAttack currAttack;
  private bool hasEnabledKickCollider = false;
  private bool hasAppliedForce;
  public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine)
  {
    int atkIdx = Random.Range(0, stateMachine.Attacks.Length);
    if (stateMachine.IsArcher || stateMachine.IsMage)
    {
      switch (atkIdx)
      {
        case 0:
          CURR_ATTACK_ANIMATOR = KICK_ATTACK;
          currAttack = stateMachine.Attacks[atkIdx];
          break;
      }
    }
    else
    {
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
  }
  public override void Enter()
  {
    stateMachine.SetCurrAttackCD(stateMachine.CurrAttackCD - 0.01f);
    stateMachine.Animator.CrossFadeInFixedTime(CURR_ATTACK_ANIMATOR, CROSS_FADE_TIME);
    if (!stateMachine.IsArcher && !stateMachine.IsMage) { stateMachine.Weapon.SetAttack(currAttack.AttackDamage, currAttack.Knockback); }
  }
  public override void Exit()
  {
    if (hasEnabledKickCollider) { stateMachine.KickAttackObject.GetComponent<CapsuleCollider>().enabled = false; }
  }
  public override void Tick(float deltaTime)
  {
    Move(deltaTime);
    FaceLocation(stateMachine.Player.transform.position);

    float normalizedTime = GetNormalizedTime(stateMachine.Animator, "EnemyAttack");
    Debug.Log(normalizedTime);

    if (normalizedTime >= 1.0f) { stateMachine.SwitchState(new EnemyIdleState(stateMachine)); return; }
    if (CURR_ATTACK_ANIMATOR == KICK_ATTACK && normalizedTime > 0.3f && hasEnabledKickCollider)
    {
      stateMachine.KickAttackObject.GetComponent<CapsuleCollider>().enabled = true;
      hasEnabledKickCollider = true;
    }
    if (normalizedTime >= currAttack.ForceTime) { TryApplyForce(); }
  }
  private void TryApplyForce()
  {
    if (hasAppliedForce) { return; }
    stateMachine.ForceReceiver.AddForce(stateMachine.transform.forward * currAttack.Force);
    hasAppliedForce = true;
  }
}
