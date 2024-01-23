using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMageState : EnemyBaseState
{
  private readonly int MAGE_ATTACK = Animator.StringToHash("MagicAttack");
  public EnemyMageState(EnemyStateMachine stateMachine) : base(stateMachine) { }
  private bool hasFired = false;
  public override void Enter()
  {
    stateMachine.Animator.CrossFadeInFixedTime(MAGE_ATTACK, CROSS_FADE_TIME);
  }
  public override void Exit() { }
  public override void Tick(float deltaTime)
  {
    FaceLocation(stateMachine.Player.transform.position);
    float normalizedTime = GetNormalizedTime(stateMachine.Animator, "MageAttack");
    if (normalizedTime >= 0.75f && !hasFired)
    {
      stateMachine.CurrAttackCD -= 0.01f;
      UnityEngine.GameObject.Instantiate(
        stateMachine.MagicAttackProjectile,
        stateMachine.RightHand.transform.position,
        Quaternion.LookRotation((stateMachine.Player.transform.position + Vector3.up - stateMachine.RightHand.transform.position).normalized)
      );
      hasFired = true;
    }
    else if (normalizedTime >= 1.0f) { stateMachine.SwitchState(new EnemyPatrolState(stateMachine)); }
  }
}
