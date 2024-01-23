using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
  private readonly bool isDead = false;
  public PlayerDeadState(PlayerStateMachine stateMachine, bool isDead) : base(stateMachine)
  {
    this.isDead = isDead;
  }
  public override void Enter()
  {
    stateMachine.SwapCameras(11, 10, 9);
    if (stateMachine.PrimaryWeaponIndex == -1) { return; }
    //stateMachine.WeaponLogic[stateMachine.PrimaryWeaponIndex].gameObject.SetActive(!isDead);
    stateMachine.Ragdoll.ToggleRagdoll(isDead);
  }
  public override void Exit() { }
  public override void Tick(float deltaTime) { }
}
