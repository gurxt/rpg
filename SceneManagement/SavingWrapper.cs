using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingWrapper : MonoBehaviour
{
  const string defaultSaveFile = "save";
  [field: SerializeField] public InputReader Input { get; private set; }
  private float refactory = 5.0f;
  private bool inRefactoryPeriod = false;
  private IEnumerator Start()
  {
    Fader fader = FindObjectOfType<Fader>();
    fader.FadeOutImmediate();
    yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
    yield return fader.FadeIn(1.0f);
  }
  private void Update()
  {
    if (Input.IsSaving && refactory == 5.0f)
    {
      Save();
      inRefactoryPeriod = true;
    }
    if (Input.IsLoading && refactory == 5.0f)
    {
      Load();
      inRefactoryPeriod = true;
    }
    if (inRefactoryPeriod)
    {
      refactory -= Time.deltaTime;
      if (refactory <= 0.0f)
      {
        refactory = 5.0f;
        inRefactoryPeriod = false;
      }
    }
  }
  public void Load()
  {
    Debug.Log("LOADING...");
    GetComponent<SavingSystem>().Load(defaultSaveFile);
  }
  public void Save()
  {
    Debug.Log("SAVING...");
    GetComponent<SavingSystem>().Save(defaultSaveFile);
  }
}
