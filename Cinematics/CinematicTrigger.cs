using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicTrigger : MonoBehaviour
{
  public bool hasTriggered = false;
  private void OnTriggerEnter(Collider other)
  {
    if (!hasTriggered && other.name == "Player")
    {
      GetComponent<PlayableDirector>().Play();
      hasTriggered = true;
    }
  }
}
