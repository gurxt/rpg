using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class AttackArray
{
  public Attack[] array;
}

[System.Serializable]
public class AbilityArray
{
  public Ability[] array;
}

public class PlayerStateMachine : StateMachine, ISaveable
{
  [field: SerializeField] public InputReader Input { get; private set; }
  [field: SerializeField] public CharacterController Controller { get; private set; }
  [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
  [field: SerializeField] public Animator Animator { get; private set; }
  [field: SerializeField] public ShieldHandler ShieldHandler { get; private set; }
  [field: SerializeField] public Health Health { get; private set; }
  [field: SerializeField] public Ragdoll Ragdoll { get; private set; }
  [field: SerializeField] public GameObject Shield { get; private set; }
  [field: SerializeField] public WeaponDamage Weapon { get; private set; }
  [field: SerializeField] public GameObject WeaponPrefab { get; private set; }
  [field: SerializeField] public Transform RightHandTransform { get; private set; }
  [field: SerializeField] public Transform LeftHandTransform { get; private set; }
  [field: SerializeField] public WeaponHandler Handler { get; private set; }
  [field: SerializeField] public GameObject ArrowProjectile { get; private set; }
  [field: SerializeField] public CinemachineFreeLook FreeLookCamera { get; private set; }
  [field: SerializeField] public CinemachineVirtualCamera BowFreeLookCamera { get; private set; }
  [field: SerializeField] public CinemachineVirtualCamera AimCamera { get; private set; }
  [field: SerializeField] public Transform FollowTransform { get; private set; }
  [field: SerializeField] public float WalkMovementSpeed { get; private set; }
  [field: SerializeField] public float RunMovementSpeed { get; private set; }
  [field: SerializeField] public float RotationLerp { get; private set; }
  [field: SerializeField] public float DodgeDuration { get; private set; }
  [field: SerializeField] public float DodgeLength { get; private set; }
  [field: SerializeField] public float JumpForce { get; private set; }
  [field: SerializeField] public float KnockbackDuration { get; private set; }
  [field: SerializeField] public int PrimaryWeaponIndex { get; private set; } = 0;
  [field: SerializeField] public int SecondaryWeaponIndex { get; private set; } = 1;
  [field: SerializeField] public AttackArray[] Attacks { get; private set; }
  [field: SerializeField] public AbilityArray[] Abilities { get; private set; }
  public bool InAttackAnimation;
  public Transform MainCameraTransform { get; private set; }
  private void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;

    MainCameraTransform = Camera.main.transform;

    SwitchState(new PlayerFreeLookState(this));
  }
  public void EquipWeapon(GameObject weapon)
  {
    Destroy(WeaponPrefab);
    Transform weaponClone;

    if (weapon.name.Contains("Bow"))
    {
      Instantiate(weapon, LeftHandTransform);
      PrimaryWeaponIndex = 1;
      Input.IsBowEquipped = true;
      weaponClone = LeftHandTransform.Find(weapon.name + "(Clone)");
      SwapCameras(10, 11, 9);
      SwitchState(new PlayerBowState(this));
    }
    else
    {
      Instantiate(weapon, RightHandTransform);
      PrimaryWeaponIndex = 0;
      Input.IsBowEquipped = false;
      weaponClone = RightHandTransform.Find(weapon.name + "(Clone)");
      SwapCameras(11, 10, 9);
      SwitchState(new PlayerFreeLookState(this));
    }

    WeaponPrefab = weaponClone.gameObject;

    Transform weaponLogicTransform = weaponClone.Find("WeaponLogic");
    Handler.SetWeaponLogic(weaponLogicTransform.gameObject);

    Weapon = weaponLogicTransform.gameObject.GetComponent<WeaponDamage>();

    // -------
    Transform pickups = GameObject.Find("Pickups").transform;
    Destroy(pickups.Find("Pickup " + weapon.name).gameObject);
  }
  private void OnEnable()
  {
    Input.SwapWeaponsEvent += HandleSwapWeapons;
    Health.OnTakeDamage += HandleTakeDamage;
    Health.OnDie += HandleDeath;
  }
  private void OnDisable()
  {
    Input.SwapWeaponsEvent -= HandleSwapWeapons;
    Health.OnTakeDamage -= HandleTakeDamage;
    Health.OnDie -= HandleDeath;
  }
  private void HandleSwapWeapons()
  {
    if (InAttackAnimation) { return; }
    if (SecondaryWeaponIndex == -1) { return; }
    (SecondaryWeaponIndex, PrimaryWeaponIndex) = (PrimaryWeaponIndex, SecondaryWeaponIndex);
    SetActiveWeapon(PrimaryWeaponIndex, SecondaryWeaponIndex);
  }
  private void HandleTakeDamage() { SwitchState(new PlayerImpactState(this)); }
  private void HandleDeath() { SwitchState(new PlayerDeadState(this, true)); }
  public void SetActiveWeapon(int NewWeaponIndex, int OldWeaponIndex)
  {
    // Weapons[NewWeaponIndex].SetActive(true);
    // if (NewWeaponIndex == 0 && OldWeaponIndex != 0) { Shield.SetActive(true); }
    // if (NewWeaponIndex != 0 && OldWeaponIndex == 0) { Shield.SetActive(false); }
    // if (OldWeaponIndex == -1) { return; }
    // if (OldWeaponIndex != NewWeaponIndex) { Weapons[OldWeaponIndex].SetActive(false); }
  }
  [Serializable]
  private struct PlayerStateData
  {
    public SerializableVector3 position;
    public SerializableVector3 rotation;
    public int health;
  }
  public object CaptureState()
  {
    PlayerStateData data = new()
    {
      position = new SerializableVector3(transform.position),
      rotation = new SerializableVector3(transform.rotation.eulerAngles),
      health = Health.GetHealth()
    };
    return data;
  }
  public void RestoreState(object state)
  {
    PlayerStateData data = (PlayerStateData)state;

    Controller.enabled = false;
    transform.SetPositionAndRotation(data.position.ToVector(), Quaternion.Euler(data.rotation.ToVector()));
    Controller.enabled = true;

    StartCoroutine(RestoreHealthWithDelay(data.health));
    Health.SetHealth(data.health); // TODO: test -> if (health != 0) { Health.SetHealth(...) } 
  }
  private IEnumerator RestoreHealthWithDelay(int health)
  {
    yield return null;

    Health.SetHealth(health);

    if (health == 0)
    {
      SwitchState(new PlayerDeadState(this, true));
    }
    else
    {
      SwitchState(new PlayerDeadState(this, false));
      SwitchState(new PlayerFreeLookState(this));
    }
  }
  public void SwapCameras(int freeValue, int bowValue, int aimValue)
  {
    FreeLookCamera.Priority = freeValue;
    BowFreeLookCamera.Priority = bowValue;
    AimCamera.Priority = aimValue;
  }
}

