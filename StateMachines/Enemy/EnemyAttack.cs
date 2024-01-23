using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyAttack
{
  [field: SerializeField] public string AnimationName { get; private set; }
  [field: SerializeField] public int AttackDamage { get; private set; }
  [field: SerializeField] public float Knockback { get; private set; }
  [field: SerializeField] public float ForceTime { get; private set; }
  [field: SerializeField] public float Force { get; private set; }
}
