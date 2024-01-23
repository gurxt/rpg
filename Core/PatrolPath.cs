using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
  private void OnDrawGizmos()
  {
    for (int i = 0; i < transform.childCount; i++)
    {
      Gizmos.color = Color.white;
      Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(GetNextIndex(i)));
      if (i == transform.childCount) { continue; }
      Gizmos.color = Color.gray;
      Gizmos.DrawSphere(GetWaypoint(i), 0.3f);
    }
  }
  public int GetNextIndex(int i)
  {
    if (i + 1 == transform.childCount)
    {
      return 0;
    }
    return i + 1;
  }
  public Vector3 GetWaypoint(int i)
  {
    return transform.GetChild(i).position;
  }
}
