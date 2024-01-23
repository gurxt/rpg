using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
  [SerializeField] private Animator Animator;
  [SerializeField] private CharacterController Controller;
  private Collider[] allColliders;
  private Rigidbody[] allRigidBodies;
  private void Start()
  {
    ToggleRagdoll(false);
  }
  public void ToggleRagdoll(bool isRagdoll)
  {
    allColliders = GetComponentsInChildren<Collider>(true);
    allRigidBodies = GetComponentsInChildren<Rigidbody>(true);

    foreach (Collider collider in allColliders)
    {
      if (collider.gameObject.CompareTag("Ragdoll"))
      {
        collider.enabled = isRagdoll;
      }
    }
    foreach (Rigidbody rigidbody in allRigidBodies)
    {
      if (rigidbody.gameObject.CompareTag("Ragdoll"))
      {
        rigidbody.isKinematic = !isRagdoll;
        rigidbody.useGravity = isRagdoll;
      }
    }

    Controller.enabled = !isRagdoll;
    Animator.enabled = !isRagdoll;
  }
}
