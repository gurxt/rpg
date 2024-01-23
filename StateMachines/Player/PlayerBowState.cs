using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBowState : PlayerBaseState
{
  private const float CROSS_FADE_DURATION = 0.3f;
  private readonly bool isRunning = false;
  private readonly bool isBowDrawn = false;
  public PlayerBowState(PlayerStateMachine stateMachine, bool isRunning = false, bool isBowDrawn = false) : base(stateMachine)
  {
    this.isBowDrawn = isBowDrawn;
    this.isRunning = isRunning;
    if (isBowDrawn) { stateMachine.SwapCameras(9, 10, 11); }
    else { stateMachine.SwapCameras(9, 11, 10); }
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
    AlignPlayerWithCamera();

    if (!isBowDrawn && CheckIfRunning(isRunning)) { return; }
    if (DrawOrShoot()) { return; }
  }
  private void AlignPlayerWithCamera()
  {
    Vector2 _look = Mouse.current.delta.ReadValue();

    stateMachine.FollowTransform.rotation *= Quaternion.AngleAxis(_look.x * 0.1f, Vector3.up);
    stateMachine.FollowTransform.rotation *= Quaternion.AngleAxis(-_look.y * 0.1f, Vector3.right);

    Vector3 angles = stateMachine.FollowTransform.localEulerAngles;
    angles.z = 0;

    float angle = stateMachine.FollowTransform.localEulerAngles.x;

    if (angle > 180 && angle < 300)
    {
      angles.x = 300;
    }
    else if (angle < 180 && angle > 60)
    {
      angles.x = 60;
    }

    stateMachine.FollowTransform.transform.localEulerAngles = angles;
    stateMachine.transform.rotation = Quaternion.Euler(0, stateMachine.FollowTransform.rotation.eulerAngles.y, 0);
    stateMachine.FollowTransform.localEulerAngles = new Vector3(stateMachine.FollowTransform.localEulerAngles.x, 0, 0);
  }
  private bool DrawOrShoot()
  {
    if (stateMachine.Input.IsBowDraw && !isBowDrawn)
    {
      stateMachine.Input.SetIsRunning(false);
      stateMachine.SwitchState(new PlayerDrawState(stateMachine));
      return true;
    }
    else if (!stateMachine.Input.IsBowDraw && isBowDrawn)
    {
      UnityEngine.GameObject arrowInstance = UnityEngine.GameObject.Instantiate(
        stateMachine.ArrowProjectile,
        stateMachine.RightHandTransform.position,
        stateMachine.MainCameraTransform.rotation
      );
      stateMachine.SwitchState(new PlayerBowState(stateMachine));
      //stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
      return true;
    }
    return false;
  }
}
