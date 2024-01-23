using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHandler : MonoBehaviour
{
  [SerializeField] private GameObject shieldLogic;
  public void EnableShield()
  {
    shieldLogic.SetActive(true);
  }
  public void DisableShield()
  {
    shieldLogic.SetActive(false);
  }
}
