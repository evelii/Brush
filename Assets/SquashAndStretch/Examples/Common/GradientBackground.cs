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
using UnityEngine.Rendering;

public class GradientBackground : MonoBehaviour
{
  public Color m_topColor    = Color.white;
  public Color m_bottomColor = Color.black;

  Mesh m_mesh;
  Shader m_shader;
  Material m_material;
  CommandBuffer m_commandBuffer;

  void Start()
  {
    m_mesh = new Mesh();
    m_mesh.vertices =
      new Vector3[]
      {
        new Vector3(-1.0f, -1.0f, 0.0f),
        new Vector3(-1.0f,  1.0f, 0.0f),
        new Vector3( 1.0f,  1.0f, 0.0f),
        new Vector3( 1.0f, -1.0f, 0.0f),
      };
    m_mesh.SetIndices(new int[] { 0, 1, 2, 0, 2, 3}, MeshTopology.Triangles, 0);

    m_shader = Shader.Find("Squash & Stretch Kit Example/GradientBackground");
    m_material = new Material(m_shader);
    m_material.SetColor("_TopColor", m_topColor);
    m_material.SetColor("_BottomColor", m_bottomColor);

    m_commandBuffer = new CommandBuffer();
    m_commandBuffer.name = "GradientBackground";
    m_commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
    m_commandBuffer.DrawMesh(m_mesh, Matrix4x4.identity, m_material);

    GetComponent<Camera>().AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_commandBuffer);
  }

  private void OnPreRender()
  {
    m_material.SetColor("_TopColor", m_topColor);
    m_material.SetColor("_BottomColor", m_bottomColor);
  }
}
