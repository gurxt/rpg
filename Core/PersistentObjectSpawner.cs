using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjectSpawner : MonoBehaviour
{
  [field: SerializeField] public GameObject PersistentObjectPrefab { get; private set; }
  private static bool hasSpawned = false;
  private void Awake()
  {
    if (hasSpawned) { return; }
    SpawnPersistentObject();
    hasSpawned = true;
  }
  private void SpawnPersistentObject()
  {
    GameObject persistentObject = Instantiate(PersistentObjectPrefab);
    DontDestroyOnLoad(persistentObject);
  }
}
