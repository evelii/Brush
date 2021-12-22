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

public class BouncingObject : MonoBehaviour
{
  static private int s_direction = 1;
  static private int s_count = 0;

  private int m_direction;
  private float m_scale;
  private float m_life;

  private void Reset()
  {
    transform.position =
      new Vector3
      (
        m_direction * 14.0f, 
        Random.Range(3.0f, 5.0f), 
        Random.Range(-6.0f, 1.0f)
      );

    m_scale = Random.Range(0.3f, 1.0f);
    transform.localScale = new Vector3(m_scale, m_scale, m_scale);

    Rigidbody rigidBody = GetComponent<Rigidbody>();
    rigidBody.velocity =
      new Vector3
      (
        -m_direction * Random.Range(6.0f, 8.0f),
        Random.Range(-1.0f, 3.0f),
        0.0f
      );

    m_life = 0.3f * s_count;
  }

  void Start()
  {
    m_direction = s_direction;
    s_direction = -s_direction;
    ++s_count;

    Reset();

    transform.position =
      new Vector3
      (
        Random.Range(-14.0f, 14.0f),
        Random.Range(3.0f, 5.0f),
        Random.Range(-6.0f, 1.0f)
      );
  }

  void Update()
  {
    m_life -= Time.deltaTime;
    if (m_life < 1.0f)
      transform.localScale = new Vector3(m_scale * m_life, m_scale * m_life, m_scale * m_life);

    if (m_life < 0.0f)
      Reset();

    if (Input.GetKeyDown(KeyCode.Space))
    {
      SquashAndStretch squashAndStretch = GetComponent<SquashAndStretch>();
      squashAndStretch.enableSquash = !squashAndStretch.enableSquash;
      squashAndStretch.enableStretch = squashAndStretch.enableSquash;
    }
  }
}
