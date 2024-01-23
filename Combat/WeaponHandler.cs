using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
  [SerializeField] private GameObject weaponLogic;
  public void SetWeaponLogic(GameObject weaponLogic)
  {
    this.weaponLogic = weaponLogic;
  }
  public void EnableWeapon()
  {
    if (!weaponLogic) { return; }
    weaponLogic.SetActive(true);
  }
  public void DisableWeapon()
  {
    if (!weaponLogic) { return; }
    weaponLogic.SetActive(false);
  }
}