using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

[Serializable]
public class Ability
{
  [field: SerializeField] public string AnimationName { get; private set; }
  [field: SerializeField] public float TransitionDuration { get; private set; }
  [field: SerializeField] public int AbilityIndex { get; private set; } = -1;
  [field: SerializeField] public float AbilityAttackTime { get; private set; }
  [field: SerializeField] public float ForceTime { get; private set; }
  [field: SerializeField] public float Force { get; private set; }
  [field: SerializeField] public int Damage { get; private set; }
  [field: SerializeField] public int Knockback { get; private set; }
}