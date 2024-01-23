using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
  enum DestinationIdentifier
  {
    A, B, C, D
  }
  [field: SerializeField] public int NextScene { get; private set; } = -1;
  [field: SerializeField] public Transform SpawnPoint { get; private set; }
  [SerializeField] DestinationIdentifier destination;
  private GameObject player;
  private void OnTriggerEnter(Collider other)
  {
    if (other.name == "Player")
    {
      StartCoroutine(Transition());
    }
  }
  private IEnumerator Transition()
  {
    if (NextScene < 0) { yield break; }

    DontDestroyOnLoad(gameObject);

    Fader fader = FindObjectOfType<Fader>();

    yield return fader.FadeOut(2.0f);

    SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
    wrapper.Save();

    yield return SceneManager.LoadSceneAsync(NextScene);

    wrapper.Load();

    Portal otherPortal = GetOtherPortal();
    UpdatePlayer(otherPortal);

    wrapper.Save();

    yield return new WaitForSeconds(2.0f);
    yield return fader.FadeIn(2.0f);

    Destroy(gameObject); // has to be at the root of the scene to work.
  }
  private void UpdatePlayer(Portal otherPortal)
  {
    if (otherPortal == null) { return; }
    player = GameObject.FindWithTag("Player");
    player.GetComponent<CharacterController>().enabled = false;
    player.transform.SetPositionAndRotation(otherPortal.SpawnPoint.position, otherPortal.SpawnPoint.rotation);
    player.GetComponent<CharacterController>().enabled = true;
  }
  private Portal GetOtherPortal()
  {
    foreach (Portal portal in FindObjectsOfType<Portal>())
    {
      if (portal == this) { continue; }
      if (portal.destination != destination) { continue; }
      return portal;
    }
    return null;
  }
}
