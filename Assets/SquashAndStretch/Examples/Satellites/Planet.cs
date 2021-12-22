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

public class Planet : MonoBehaviour
{
  public GameObject m_satellite;
  private GameObject[] m_aSatellite;
  private Quaternion m_satelliteRot;

  void Start()
  {
    m_aSatellite = new GameObject[8];

    for (int i = 0; i < m_aSatellite.Length; ++i)
    {
      GameObject newSatellite = Instantiate(m_satellite);
      newSatellite.name = "Satellite" + i;
      newSatellite.transform.SetParent(transform, false);
      newSatellite.transform.localPosition = 
        new Vector3
        (
          (i & 1) != 0 ? -1.0f : 1.0f, 
          (i & 2) != 0 ? -1.0f : 1.0f, 
          (i & 4) != 0 ? -1.0f : 1.0f
        );

      m_aSatellite[i] = newSatellite;
    }

    m_satelliteRot = Quaternion.AngleAxis(45.0f, Vector3.left) * Quaternion.AngleAxis(45.0f, Vector3.up);
    transform.rotation = m_satelliteRot;
  }

  void LateUpdate()
  {
    foreach (GameObject satellite in m_aSatellite)
    {
      satellite.transform.rotation = m_satelliteRot;
    }
  }
}
