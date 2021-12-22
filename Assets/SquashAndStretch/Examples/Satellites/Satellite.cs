/******************************************************************************/
/*
  Project   - Squash And Stretch Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using SquashAndStretchKit;
using UnityEngine;

public class Satellite : MonoBehaviour
{
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      SquashAndStretch squashAndStretch = GetComponent<SquashAndStretch>();
      squashAndStretch.enableStretch = !squashAndStretch.enableStretch;
    }
  }
}
