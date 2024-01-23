using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : StateMachine, ISaveable
{
  [field: SerializeField] public Animator Animator { get; private set; }
  [field: SerializeField] public CharacterController Controller { get; private set; }
  [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
  [field: SerializeField] public NavMeshAgent Agent { get; private set; }
  [field: SerializeField] public WeaponDamage Weapon { get; private set; } = null;
  [field: SerializeField] public Health Health { get; private set; }
  [field: SerializeField] public Target Target { get; private set; }
  [field: SerializeField] public Ragdoll Ragdoll { get; private set; }
  [field: SerializeField] public PatrolPath PatrolPath { get; private set; }
  [field: SerializeField] public Transform RightHand { get; private set; }
  [field: SerializeField] public GameObject EnemyArrowProjectile { get; private set; } = null;
  [field: SerializeField] public GameObject MagicAttackProjectile { get; private set; } = null;
  [field: SerializeField] public float PlayerChasingRange { get; private set; }
  [field: SerializeField] public float MovementSpeed { get; private set; }
  [field: SerializeField] public float AttackRange { get; private set; }
  [field: SerializeField] public int AttackDamage { get; private set; }
  [field: SerializeField] public float KnockbackDuration { get; private set; }
  [field: SerializeField] public float MaxChasingRange { get; private set; } = 30.0f;
  [field: SerializeField] public bool IsArcher { get; private set; } = false;
  [field: SerializeField] public bool IsMage { get; private set; } = false;
  [field: SerializeField] public float AttackCD { get; private set; } = 2.0f;
  [field: SerializeField] public EnemyAttack[] Attacks { get; private set; }
  public GameObject Player { get; private set; }
  public Vector3 StartingPosition { get; private set; }
  public bool HasIncreasedChasingRange { get; private set; } = false;
  public bool HasIncreaseMaxChasingRange { get; private set; } = false;
  public bool IsReturningToStart = false;
  public float CurrAttackCD;
  private void OnEnable()
  {
    Health.OnTakeDamage += HandleTakeDamage;
    Health.OnDie += HandleDeath;
  }
  private void OnDisable()
  {
    Health.OnTakeDamage -= HandleTakeDamage;
    Health.OnDie -= HandleDeath;
  }
  private void Start()
  {
    CurrAttackCD = AttackCD;

    Player = GameObject.FindGameObjectWithTag("Player");

    Agent.updatePosition = false;
    Agent.updateRotation = false;

    StartingPosition = transform.position;

    SwitchState(new EnemyPatrolState(this));
  }
  public void SetMovementSpeed(float speed) { MovementSpeed = speed; }
  private void HandleTakeDamage()
  {
    SwitchState(new EnemyImpactState(this));
  }
  private void HandleDeath()
  {
    SwitchState(new EnemyDeadState(this, true));
  }
  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, PlayerChasingRange);
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(transform.position, PlayerChasingRange + 5.0f);
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(StartingPosition, MaxChasingRange);
  }
  public void IncreaseChasingRange() { PlayerChasingRange += 5; HasIncreasedChasingRange = true; }
  public void DecreaseChasingRange() { PlayerChasingRange -= 5; HasIncreasedChasingRange = false; }
  public void IncreaseMaxChasingRange() { if (HasIncreaseMaxChasingRange) { return; } MaxChasingRange += 1.0f; HasIncreaseMaxChasingRange = true; }
  public void DecreaseMaxChasingRange() { if (!HasIncreaseMaxChasingRange) { return; } MaxChasingRange -= 1.0f; HasIncreaseMaxChasingRange = false; }

  [Serializable]
  struct EnemyStateData
  {
    public SerializableVector3 position;
    public int health;
  }
  public object CaptureState()
  {
    EnemyStateData data = new()
    {
      position = new SerializableVector3(transform.position),
      health = Health.GetHealth()
    };
    return data;
  }
  public void RestoreState(object state)
  {
    EnemyStateData data = (EnemyStateData)state;

    GetComponent<NavMeshAgent>().enabled = false;
    Controller.enabled = false;

    transform.position = data.position.ToVector();

    GetComponent<NavMeshAgent>().enabled = true;
    Controller.enabled = true;

    StartCoroutine(RestoreHealthWithDelay(data.health));
    Health.SetHealth(data.health);
  }
  private IEnumerator RestoreHealthWithDelay(int health)
  {
    yield return null;

    if (health == 0)
    {
      SwitchState(new EnemyDeadState(this, true));
    }
    else
    {
      SwitchState(new EnemyDeadState(this, false));
      SwitchState(new EnemyPatrolState(this));
    }
  }
}
