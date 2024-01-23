using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class PlayerBaseState : State
{
  [SerializeField] protected PlayerStateMachine stateMachine;
  protected readonly int FREE_LOOK_BLEND_TREE = Animator.StringToHash("FreeLookBlendTree");
  protected readonly int WALK_RIGHT = Animator.StringToHash("WalkRight");
  protected readonly int WALK_FORWARD = Animator.StringToHash("WalkForward");
  protected readonly int RUN_BLEND_TREE = Animator.StringToHash("RunningBlendTree");
  protected readonly int RUN_RIGHT = Animator.StringToHash("RunRight");
  protected readonly int RUN_FORWARD = Animator.StringToHash("RunForward");
  protected readonly int BOW_BLEND_TREE = Animator.StringToHash("BowAimBlendTree");
  public PlayerBaseState(PlayerStateMachine stateMachine)
  {
    this.stateMachine = stateMachine;
  }
  protected void Move(Vector3 motion, float deltaTime)
  {
    stateMachine.Controller.Move((motion + stateMachine.ForceReceiver.Movement) * deltaTime);
  }
  protected void Move(float deltaTime)
  {
    Move(Vector3.zero, deltaTime);
  }
  protected bool IsControlEnabled()
  {
    return stateMachine.Controller.enabled;
  }
  protected void UpdateAnimator(float deltaTime, int right, int forward, bool controlsDisabled = false)
  {
    if (controlsDisabled)
    {
      stateMachine.Animator.SetFloat(forward, 0.0f);
      stateMachine.Animator.SetFloat(right, 0.0f);
      return;
    }
    if (stateMachine.Input.MovementValue.y == 0f)
    {
      stateMachine.Animator.SetFloat(forward, 0f, 0.1f, deltaTime);
    }
    else
    {
      float value = stateMachine.Input.MovementValue.y > 0 ? 1f : -1f;
      stateMachine.Animator.SetFloat(forward, value, 0.1f, deltaTime);
    }

    if (stateMachine.Input.MovementValue.x == 0f)
    {
      stateMachine.Animator.SetFloat(right, 0f, 0.1f, deltaTime);
    }
    else
    {
      float value = stateMachine.Input.MovementValue.x > 0 ? 1f : -1f;
      stateMachine.Animator.SetFloat(right, value, 0.1f, deltaTime);
    }
  }
  protected void FaceMovementDirection(float deltaTime)
  {
    Transform cameraTransform = Camera.main.transform;

    Vector3 forwardDirection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;

    stateMachine.transform.rotation = Quaternion.Lerp(
      stateMachine.transform.rotation,
      Quaternion.LookRotation(forwardDirection),
      deltaTime * stateMachine.RotationLerp);
  }
  protected Vector3 CalculateMovement()
  {
    Vector3 right = stateMachine.MainCameraTransform.right;
    Vector3 forward = stateMachine.MainCameraTransform.forward;

    forward.y = 0.0f;
    right.y = 0.0f;

    forward.Normalize();
    right.Normalize();

    return forward * stateMachine.Input.MovementValue.y + right * stateMachine.Input.MovementValue.x;
  }
  protected void SetBlendTree(bool isRunning, bool isBowDrawn, float crossFade)
  {
    if (isRunning)
    { stateMachine.Animator.CrossFadeInFixedTime(RUN_BLEND_TREE, crossFade); }
    else if (isBowDrawn)
    { stateMachine.Animator.CrossFadeInFixedTime(BOW_BLEND_TREE, crossFade); }
    else
    { stateMachine.Animator.CrossFadeInFixedTime(FREE_LOOK_BLEND_TREE, crossFade); }
  }
  protected bool CheckIfRunning(bool isRunning)
  {
    if (stateMachine.Input.IsRunning && !isRunning)
    {
      SwitchToBowOrFreeLookState(true, false);
      return true;
    }
    else if (!stateMachine.Input.IsRunning && isRunning)
    {
      SwitchToBowOrFreeLookState();
    }
    return false;
  }
  protected void MovePlayer(float deltaTime)
  {
    Vector3 movement = CalculateMovement();
    float movementSpeed = stateMachine.Input.IsRunning ? stateMachine.RunMovementSpeed : stateMachine.WalkMovementSpeed;
    Move(movement * movementSpeed, deltaTime);

    if (Vector3.zero != movement) { FaceMovementDirection(deltaTime); }
  }
  protected void UpdateAnimator(float deltaTime)
  {
    if (stateMachine.Input.IsRunning && !stateMachine.Input.IsBowDraw)
    {
      UpdateAnimator(deltaTime, RUN_RIGHT, RUN_FORWARD);
    }
    else
    {
      UpdateAnimator(deltaTime, WALK_RIGHT, WALK_FORWARD);
    }
  }
  protected void SwitchToBowOrFreeLookState(bool isRunning = false, bool isBowDraw = false)
  {
    if (stateMachine.PrimaryWeaponIndex == 1)
    {
      stateMachine.SwitchState(new PlayerBowState(stateMachine, isRunning, isBowDraw));
    }
    else
    {
      stateMachine.SwitchState(new PlayerFreeLookState(stateMachine, isRunning, isBowDraw));
    }
  }
  protected void OnDodge()
  {
    if (!stateMachine.Input.IsBowDraw)
    {
      stateMachine.SwitchState(new PlayerDodgingState(stateMachine, stateMachine.Input.MovementValue));
    }
  }
  protected void OnJump()
  {
    if (!stateMachine.Input.IsBowDraw)
    {
      stateMachine.SwitchState(new PlayerJumpingState(stateMachine));
    }
  }
}