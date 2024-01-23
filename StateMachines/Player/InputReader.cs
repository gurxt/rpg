using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions
{
  private Controls controls;
  public event Action JumpEvent;
  public event Action DodgeEvent;
  public event Action SwapWeaponsEvent;
  public bool IsSaving { get; private set; }
  public bool IsLoading { get; private set; }
  public bool IsAttacking { get; private set; }
  public bool IsUsingAbility { get; private set; }
  public bool IsRunning = false;
  public bool IsBlocking { get; private set; } = false;
  public bool IsBowEquipped = false;
  public bool IsBowDraw = false;
  public int AbilityIdx { get; private set; } = -1;
  public Vector2 MovementValue { get; private set; }
  public Vector2 MouseValue;
  private void Start()
  {
    controls = new Controls();
    controls.Player.SetCallbacks(this);
    controls.Player.Enable();
  }
  private void OnDestroy()
  {
    controls.Player.Disable();
  }
  public void OnJump(InputAction.CallbackContext context)
  {
    if (!context.performed) { return; }
    JumpEvent?.Invoke();
  }
  public void OnDodge(InputAction.CallbackContext context)
  {
    if (!context.performed) { return; }
    DodgeEvent?.Invoke();
  }
  public void OnMove(InputAction.CallbackContext context)
  {
    MovementValue = context.ReadValue<Vector2>();
  }
  public void OnAttack(InputAction.CallbackContext context)
  {
    if (!IsUsingAbility)
    {
      if (context.performed)
      {
        if (IsBowEquipped) { IsBowDraw = true; }
        IsAttacking = true;
      }
      else if (context.canceled)
      {
        if (IsBowEquipped) { IsBowDraw = false; }
        IsAttacking = false;
      }
    }
  }
  public void OnAbility(InputAction.CallbackContext context)
  {
    if (context.performed)
    {
      if (context.control.name == "q")
      {
        AbilityIdx = 0;
      }
      else if (context.control.name == "e")
      {
        AbilityIdx = 1;
      }
      else if (context.control.name == "r")
      {
        AbilityIdx = 2;
      }
      IsUsingAbility = true;
    }
    else if (context.canceled)
    {
      IsUsingAbility = false;
      AbilityIdx = -1;
    }
  }
  public void OnRun(InputAction.CallbackContext context)
  {
    if (context.performed && !IsBlocking && !IsBowDraw)
    {
      IsRunning = !IsRunning;
    }
  }
  public void SetIsRunning(bool status) { this.IsRunning = status; }
  public void OnLook(InputAction.CallbackContext context) { }
  public void OnBlock(InputAction.CallbackContext context)
  {
    if (context.performed)
    {

      IsBlocking = true;
      IsRunning = false;
    }
    else if (context.canceled)
    {
      IsBlocking = false;
    }
  }
  public void OnSave(InputAction.CallbackContext context)
  {
    if (context.performed) { IsSaving = true; }
    else if (context.canceled) { IsSaving = false; }
  }
  public void OnLoad(InputAction.CallbackContext context)
  {
    if (context.performed) { IsLoading = true; }
    else if (context.canceled) { IsLoading = false; }
  }
  public void OnSwapWeapons(InputAction.CallbackContext context)
  {
    if (!context.performed) { return; }
    SwapWeaponsEvent?.Invoke();
  }
  public void OnMouse(InputAction.CallbackContext context)
  {
    //if (!context.performed) { return; }
    MouseValue = context.ReadValue<Vector2>();
  }
}
