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

public class FallingStar : MonoBehaviour
{
  private Vector3 m_center;

  private float m_xyRatio;

  private float m_positionPhase;
  private float m_rotationPhase;

  private float m_positionPhaseSpeed;
  private float m_rotationPhaseSpeed;

  private void Reset()
  {
    m_xyRatio = Random.Range(0.9f, 1.1f);

    m_center.x = Random.Range(-12.0f, 12.0f);
    m_center.y = Random.Range(-2.0f, 2.0f);
    m_center.z = Random.Range(-5.0f, 5.0f);

    m_positionPhase = -0.5f * Mathf.PI;
    m_rotationPhase = Random.Range(0.0f, 2.0f * Mathf.PI);

    m_positionPhaseSpeed = Random.Range(0.3f * Mathf.PI, 0.6f * Mathf.PI);
    m_rotationPhaseSpeed = Random.Range(Mathf.PI, 3.0f * Mathf.PI);

    float tint = ((m_center.z - 0.5f) / 10.0f);
    GetComponent<SpriteRenderer>().color = Color.HSVToRGB(0.1f, tint, 1.0f);
    GetComponent<SquashAndStretch>().Reboot();
  }

  void Start()
  {
    Reset();

    m_positionPhase = Random.Range(-0.5f * Mathf.PI, 0.5f * Mathf.PI);
  }

  void Update()
  {
    m_positionPhase += m_positionPhaseSpeed * Time.deltaTime;
    float tan = Mathf.Tan(m_positionPhase);
    tan = Mathf.Sign(tan) * Mathf.Pow(Mathf.Abs(tan), 3.0f);
    transform.position = m_center + (new Vector3(-3.0f * tan * m_xyRatio, -3.0f * tan / m_xyRatio, 0.0f));

    m_rotationPhase += m_rotationPhaseSpeed * Time.deltaTime;
    transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * m_rotationPhase, Vector3.forward);

    if (m_positionPhase >= 0.5f * Mathf.PI)
      Reset();

    if (Input.GetKeyDown(KeyCode.Space))
    {
      SquashAndStretch squashAndStretch = GetComponent<SquashAndStretch>();
      squashAndStretch.enableStretch = !squashAndStretch.enableStretch;
    }
  }
}
