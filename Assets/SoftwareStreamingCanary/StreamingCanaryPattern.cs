using UnityEngine;

namespace AppMana.SoftwareStreamingCanary
{
  [RequireComponent(typeof(Camera))]
  public sealed class StreamingCanaryPattern : MonoBehaviour
  {
    private static readonly Color[] Bars =
    {
      new(0.75f, 0.75f, 0.75f),
      new(0.75f, 0.75f, 0.0f),
      new(0.0f, 0.75f, 0.75f),
      new(0.0f, 0.75f, 0.0f),
      new(0.75f, 0.0f, 0.75f),
      new(0.75f, 0.0f, 0.0f),
      new(0.0f, 0.0f, 0.75f)
    };

    private Material m_Material;

    private void Awake()
    {
      Application.targetFrameRate = 30;
      // Batch Vulkan players have no presentation swapchain for the vSync setter.
      if (!Application.isBatchMode) QualitySettings.vSyncCount = 0;
      var camera = GetComponent<Camera>();
      camera.clearFlags = CameraClearFlags.SolidColor;
      camera.backgroundColor = Color.black;
      camera.allowHDR = false;
      camera.allowMSAA = false;
      m_Material = new Material(Shader.Find("Hidden/Internal-Colored"))
      {
        hideFlags = HideFlags.HideAndDontSave
      };
    }

    private void OnDestroy()
    {
      if (m_Material != null)
      {
        Destroy(m_Material);
      }
    }

    private void OnPostRender()
    {
      if (m_Material == null || !m_Material.SetPass(0))
      {
        return;
      }

      GL.PushMatrix();
      GL.LoadOrtho();
      GL.Begin(GL.QUADS);

      for (var index = 0; index < Bars.Length; index++)
      {
        Rect(index / 7f, 0.28f, (index + 1) / 7f, 1f, Bars[index]);
      }

      Rect(0f, 0.08f, 1f, 0.28f, new Color(0.035f, 0.035f, 0.035f));
      var progress = (Time.frameCount % 240) / 240f;
      Rect(progress, 0.08f, Mathf.Min(1f, progress + 0.025f), 0.28f, Color.white);

      var sequence = (ushort)Time.frameCount;
      for (var cell = 0; cell < 20; cell++)
      {
        var luminance = FramePattern.CellValue(cell, sequence) ? 0.95f : 0.05f;
        Rect(cell * 0.01f, 0f, (cell + 1) * 0.01f, 0.045f,
          new Color(luminance, luminance, luminance));
      }

      GL.End();
      GL.PopMatrix();
    }

    private static void Rect(float left, float bottom, float right, float top, Color color)
    {
      GL.Color(color);
      GL.Vertex3(left, bottom, 0f);
      GL.Vertex3(right, bottom, 0f);
      GL.Vertex3(right, top, 0f);
      GL.Vertex3(left, top, 0f);
    }
  }
}
