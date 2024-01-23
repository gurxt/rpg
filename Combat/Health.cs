using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
  [SerializeField] private int maxHealth = 100;
  public event Action OnTakeDamage;
  public event Action OnDie;
  [SerializeField] private int health;
  private bool isInvulnerable;
  public bool IsDead => health == 0;
  private void Awake() { health = maxHealth; }
  public void SetInvulnerable(bool isInvulnerable) { this.isInvulnerable = isInvulnerable; }
  public void DealDamage(int damage)
  {
    if (IsDead) { return; }
    if (isInvulnerable) { return; }

    health = Mathf.Max(health - damage, 0);
    OnTakeDamage?.Invoke();
    Debug.Log(health);

    if (IsDead) { OnDie?.Invoke(); }
  }
  public int GetHealth() { return health; }
  public void SetHealth(int health) { this.health = health; if (IsDead) { OnDie?.Invoke(); } }
}
