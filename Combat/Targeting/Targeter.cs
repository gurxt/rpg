using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Targeter : MonoBehaviour
{
  public List<Target> targets = new();
  private void OnTriggerEnter(Collider other)
  {
    if (!other.TryGetComponent<Target>(out Target target)) { return; }
    targets.Add(target);
    target.OnDestroyed += RemoveTarget;
  }
  private void OnTriggerExit(Collider other)
  {
    if (!other.TryGetComponent<Target>(out Target target)) { return; }
    targets.Remove(target);
  }
  private void RemoveTarget(Target target)
  {
    targets.Remove(target);
    target.OnDestroyed -= RemoveTarget;
  }
}
