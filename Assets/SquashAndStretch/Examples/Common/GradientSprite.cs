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

public class GradientSprite : MonoBehaviour
{
  public Color m_topColor = Color.white;
  public Color m_bottomColor = Color.black;

  private Material m_material;

  private void Start()
  {
    SpriteRenderer renderer = GetComponent<SpriteRenderer>();
    m_material = new Material(renderer.material);
    renderer.material = m_material;
  }

  void LateUpdate()
  {
    SpriteRenderer renderer = GetComponent<SpriteRenderer>();
    float height = renderer.bounds.max.y - renderer.bounds.min.y;
    Vector4 bounds = new Vector4(transform.position.y - 0.6f * height, transform.position.y + 0.6f * height);

    Vector3 c0 = transform.position;
    Vector3 c1 = transform.position;
    c0.y = bounds.x;
    c1.y = bounds.y;

    m_material.SetColor("_TopColor", m_topColor);
    m_material.SetColor("_BottomColor", m_bottomColor);
    m_material.SetVector("_Bounds", bounds);
  }
}
