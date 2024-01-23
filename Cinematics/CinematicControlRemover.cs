using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicControlRemover : MonoBehaviour
{
  private GameObject Player;
  private void Start()
  {
    GetComponent<PlayableDirector>().played += DisableControl;
    GetComponent<PlayableDirector>().stopped += EnableControl;
    Player = GameObject.FindGameObjectWithTag("Player");
  }
  public void DisableControl(PlayableDirector playable)
  {
    if (!Player) { return; }
    Player.GetComponent<CharacterController>().enabled = false;
  }
  public void EnableControl(PlayableDirector playable)
  {
    if (!Player) { return; }
    Player.GetComponent<CharacterController>().enabled = true;
  }
}
