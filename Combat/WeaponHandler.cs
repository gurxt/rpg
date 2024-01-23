using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
  [SerializeField] private GameObject weaponLogic;
  public void SetWeaponLogic(GameObject weaponLogic)
  {
    Debug.Log(weaponLogic);
    this.weaponLogic = weaponLogic;
  }
  public void EnableWeapon()
  {
    weaponLogic.SetActive(true);
  }
  public void DisableWeapon()
  {
    weaponLogic.SetActive(false);
  }
}
