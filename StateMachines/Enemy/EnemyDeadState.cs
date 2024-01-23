using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
  private readonly bool isDead = false;
  public EnemyDeadState(EnemyStateMachine stateMachine, bool isDead) : base(stateMachine)
  {
    this.isDead = isDead;
  }
  public override void Enter()
  {
    if (stateMachine.Weapon) { stateMachine.Weapon.gameObject.SetActive(!isDead); }
    stateMachine.Ragdoll.ToggleRagdoll(isDead);
    if (isDead) { stateMachine.Target.enabled = false; return; }
    stateMachine.Target.enabled = true;
    stateMachine.SwitchState(new EnemyPatrolState(stateMachine));
  }
  public override void Exit() { }
  public override void Tick(float deltaTime) { }
}
