using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Fader : MonoBehaviour
{
  private CanvasGroup canvasGroup;
  private void Awake()
  {
    canvasGroup = GetComponent<CanvasGroup>();
  }
  public void FadeOutImmediate()
  {
    canvasGroup.alpha = 1;
  }
  public IEnumerator FadeOut(float time)
  {
    if (!canvasGroup) { yield return null; }
    while (canvasGroup.alpha < 1)
    {
      canvasGroup.alpha += Time.deltaTime / time;
      yield return null;
    }
  }
  public IEnumerator FadeIn(float time)
  {
    if (!canvasGroup) { yield return null; }
    while (canvasGroup.alpha > 0)
    {
      canvasGroup.alpha -= Time.deltaTime / time;
      yield return null;
    }
  }
}

