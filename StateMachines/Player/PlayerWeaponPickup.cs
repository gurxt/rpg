using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponPickup : MonoBehaviour
{
  [SerializeField] GameObject weapon = null;
  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.CompareTag("Player"))
    {
      other.GetComponent<PlayerStateMachine>().EquipWeapon(weapon);
    }
  }
}
