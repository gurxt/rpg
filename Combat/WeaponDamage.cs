using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
  [SerializeField] private Collider myCollider;
  private readonly List<Collider> alreadyCollidedWith = new List<Collider>();
  private int damage;
  private float knockback;
  public bool collidedWithShield = false;
  private void OnEnable()
  {
    alreadyCollidedWith.Clear();
    collidedWithShield = false;
  }
  private void OnTriggerEnter(Collider other)
  {
    if (other == myCollider) { return; }
    if (alreadyCollidedWith.Contains(other)) { return; }

    if (collidedWithShield) { return; }

    alreadyCollidedWith.Add(other);

    if (other.name == "Enemy" && other.GetComponent<BoxCollider>().enabled == true)
    {
      other.GetComponent<EnemyStateMachine>().SetIsBlockImpact(true);
      collidedWithShield = true;
      return;
    }

    if (other.name == "ShieldLogic")
    {
      collidedWithShield = true;
      return;
    }

    if (other.TryGetComponent<ForceReceiver>(out ForceReceiver forceReceiver))
    {
      Vector3 direction = (other.transform.position - myCollider.transform.position).normalized;
      forceReceiver.AddForce(direction * knockback);
    }
    if (other.TryGetComponent<Health>(out Health health))
    {
      health.DealDamage(damage);
    }
  }
  public void SetAttack(int damage, float knockback)
  {
    this.damage = damage;
    this.knockback = knockback;
  }
}
