using UnityEngine;

public class PlayerFreeLookState : PlayerBaseState
{
  private const float CROSS_FADE_DURATION = 0.3f;
  private readonly bool isRunning = false;
  private readonly bool isBowDrawn = false;
  public PlayerFreeLookState(PlayerStateMachine stateMachine, bool isRunning = false, bool isBowDrawn = false) : base(stateMachine)
  {
    this.isRunning = isRunning;
    this.isBowDrawn = isBowDrawn;
  }
  public override void Enter()
  {
    stateMachine.Input.DodgeEvent += OnDodge;
    stateMachine.Input.JumpEvent += OnJump;

    SetBlendTree(isRunning, isBowDrawn, CROSS_FADE_DURATION);
  }
  public override void Exit()
  {
    stateMachine.Input.DodgeEvent -= OnDodge;
    stateMachine.Input.JumpEvent -= OnJump;
  }
  public override void Tick(float deltaTime)
  {
    if (!IsControlEnabled())
    {
      UpdateAnimator(
        deltaTime,
        isRunning ? RUN_RIGHT : WALK_RIGHT,
        isRunning ? RUN_FORWARD : WALK_FORWARD,
        true);
      return;
    }
    MovePlayer(deltaTime);
    UpdateAnimator(deltaTime);

    ChecksIfBow();
    if (CheckIfBlockOrAttack()) { return; }
    if (CheckIfRunning(isRunning)) { return; }
  }
  private bool CheckIfBlockOrAttack()
  {
    if (stateMachine.Input.IsBlocking)
    {
      stateMachine.SwitchState(new PlayerBlockingState(stateMachine));
      return true;
    }
    else if (stateMachine.WeaponPrefab && !isBowDrawn)
    {
      if (stateMachine.Input.IsAttacking)
      {
        stateMachine.InAttackAnimation = true;
        stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
        return true;
      }
      else if (stateMachine.Input.IsUsingAbility)
      {
        stateMachine.SwitchState(new PlayerAttackingState(stateMachine, stateMachine.Input.AbilityIdx));
        return true;
      }
    }
    return false;
  }
  private void ChecksIfBow()
  {
    if (stateMachine.PrimaryWeaponIndex == 1)
    { stateMachine.SwitchState(new PlayerBowState(stateMachine, false, false)); return; }
  }
}
