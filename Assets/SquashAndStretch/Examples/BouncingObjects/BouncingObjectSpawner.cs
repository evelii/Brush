/******************************************************************************/
/*
  Project   - Squash And Stretch Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using UnityEngine;

public class BouncingObjectSpawner : MonoBehaviour
{
  public GameObject BouncingBall;

  void Start()
  {
    int numObjects = 24;
    for (int i = 0; i < numObjects; ++i)
    {
      Instantiate(BouncingBall);
    }
  }
}
