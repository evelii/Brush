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

public class StarSpawner : MonoBehaviour
{
  public GameObject m_star;

  void Start()
  {
    for (int i = 0; i < 20; ++i)
    {
      Instantiate(m_star);
    }
  }
}
