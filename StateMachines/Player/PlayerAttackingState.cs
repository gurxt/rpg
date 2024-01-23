using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class PlayerAttackingState : PlayerBaseState
{
  private readonly Attack attack;
  private readonly Ability ability;
  private readonly bool isAttacking;
  private bool hasAppliedForce;
  public PlayerAttackingState(PlayerStateMachine stateMachine, int idx) : base(stateMachine)
  {
    if (stateMachine.PrimaryWeaponIndex == 1)
    {
      attack = stateMachine.Attacks[stateMachine.PrimaryWeaponIndex].array[idx];
      isAttacking = true;
    }
    else if (stateMachine.Input.IsAttacking)
    {
      attack = stateMachine.Attacks[stateMachine.PrimaryWeaponIndex].array[idx];
      isAttacking = true;
    }
    else
    {
      ability = stateMachine.Abilities[stateMachine.PrimaryWeaponIndex].array[idx];
      isAttacking = false;
    }
  }
  public override void Enter()
  {
    stateMachine.InAttackAnimation = true;

    if (isAttacking)
    {
      stateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName, attack.TransitionDuration);
      stateMachine.Weapon.SetAttack(attack.Damage, attack.Knockback);
    }
    else
    {
      stateMachine.Animator.CrossFadeInFixedTime(ability.AnimationName, ability.TransitionDuration);
      stateMachine.Weapon.SetAttack(ability.Damage, ability.Knockback);
    }
  }
  public override void Exit()
  {
    stateMachine.InAttackAnimation = false;
  }
  public override void Tick(float deltaTime)
  {
    if (!IsControlEnabled()) { stateMachine.SwitchState(new PlayerFreeLookState(stateMachine)); }

    Move(deltaTime);
    FaceMovementDirection(deltaTime);

    if (isAttacking && !AttackTick()) { SwitchToBowOrFreeLookState(); }
    else if (!isAttacking && !AbilityTick()) { SwitchToBowOrFreeLookState(); }

  }
  private bool AbilityTick()
  {
    if (stateMachine.PrimaryWeaponIndex == 1) { return false; }
    float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Ability");
    if (normalizedTime < 1.0f)
    {
      if (normalizedTime >= ability.ForceTime)
      {
        TryApplyForce();
      }
      return true;
    }
    return false;
  }
  private bool AttackTick()
  {
    float normalizedTime = GetNormalizedTime(
      stateMachine.Animator,
      stateMachine.PrimaryWeaponIndex == 0 ? "Combo" : "BowAttack"
    );

    if (normalizedTime < 1.0f)
    {
      if (normalizedTime >= attack.ForceTime)
      {
        TryApplyForce();
      }
      if (stateMachine.Input.IsAttacking)
      {
        TryComboAttack(normalizedTime);
      }
      return true;
    }
    return false;
  }
  private void TryComboAttack(float normalizedTime)
  {
    if (attack.ComboStateIndex == -1) { return; }
    if (normalizedTime < attack.ComboAttackTime) { return; }
    stateMachine.SwitchState(
      new PlayerAttackingState(stateMachine, attack.ComboStateIndex)
    );
  }
  private void TryApplyForce()
  {
    if (hasAppliedForce) { return; }
    if (isAttacking)
    {
      stateMachine.ForceReceiver.AddForce(stateMachine.transform.forward * attack.Force);
    }
    else
    {
      stateMachine.ForceReceiver.AddForce(stateMachine.transform.forward * ability.Force);
    }
    hasAppliedForce = true;
  }
}
