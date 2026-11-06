using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace AppMana.SoftwareStreamingCanary
{
  /// <summary>
  /// Opt-in runtime scroll-capture canary experience (-appmana-scroll-capture-test).
  /// Spawns an EventSystem, a right-side overflowing UGUI ScrollRect, and a toggleable
  /// &lt;Mouse&gt;/scroll InputAction so ScrollCaptureDetector and InputSystemLatencyTracker
  /// can be exercised end-to-end in a live Unity stream.
  /// </summary>
  public sealed class ScrollCaptureCanaryProbe : MonoBehaviour
  {
    public static bool Active { get; private set; }
    public static float LastZoomDelta { get; private set; }

    private InputAction m_ZoomScrollAction;
    private ScrollRect m_ScrollRect;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
      if (Array.IndexOf(Environment.GetCommandLineArgs(), "-appmana-scroll-capture-test") < 0) return;
      var host = new GameObject(nameof(ScrollCaptureCanaryProbe));
      DontDestroyOnLoad(host);
      host.AddComponent<ScrollCaptureCanaryProbe>();
    }

    private void Awake()
    {
      Active = true;
      EnsureEventSystem();
      BuildScrollCaptureCanvas();
      SetupZoomScrollAction(enabledByDefault: false);
    }

    public void SetZoomScrollActionEnabled(bool enabled)
    {
      if (m_ZoomScrollAction == null) return;
      if (enabled) m_ZoomScrollAction.Enable();
      else m_ZoomScrollAction.Disable();
    }

    private static void EnsureEventSystem()
    {
      if (EventSystem.current != null) return;
      var esGo = new GameObject("ScrollCaptureEventSystem");
      DontDestroyOnLoad(esGo);
      esGo.AddComponent<EventSystem>();
      esGo.AddComponent<InputSystemUIInputModule>();
    }

    private void SetupZoomScrollAction(bool enabledByDefault)
    {
      m_ZoomScrollAction = new InputAction("CameraZoomScroll", InputActionType.Value, "<Mouse>/scroll");
      m_ZoomScrollAction.performed += ctx =>
      {
        var scroll = ctx.ReadValue<Vector2>();
        LastZoomDelta = scroll.y;
      };
      if (enabledByDefault)
      {
        m_ZoomScrollAction.Enable();
      }
    }

    private void BuildScrollCaptureCanvas()
    {
      var canvasGo = new GameObject("ScrollCaptureCanaryCanvas");
      canvasGo.transform.SetParent(transform, false);
      var canvas = canvasGo.AddComponent<Canvas>();
      canvas.renderMode = RenderMode.ScreenSpaceOverlay;
      canvasGo.AddComponent<CanvasScaler>();
      canvasGo.AddComponent<GraphicRaycaster>();

      // Right-half overflowing ScrollRect (x: 0.55..0.96, y: 0.12..0.88)
      var panelGo = new GameObject("CanaryScrollRectPanel");
      panelGo.transform.SetParent(canvasGo.transform, false);
      var panelRect = panelGo.AddComponent<RectTransform>();
      panelRect.anchorMin = new Vector2(0.55f, 0.12f);
      panelRect.anchorMax = new Vector2(0.96f, 0.88f);
      panelRect.offsetMin = Vector2.zero;
      panelRect.offsetMax = Vector2.zero;
      var panelImg = panelGo.AddComponent<Image>();
      panelImg.color = new Color(0.08f, 0.11f, 0.18f, 0.92f);

      var viewportGo = new GameObject("Viewport");
      viewportGo.transform.SetParent(panelGo.transform, false);
      var viewportRect = viewportGo.AddComponent<RectTransform>();
      viewportRect.anchorMin = Vector2.zero;
      viewportRect.anchorMax = Vector2.one;
      viewportRect.offsetMin = new Vector2(6f, 6f);
      viewportRect.offsetMax = new Vector2(-6f, -6f);
      var viewportImg = viewportGo.AddComponent<Image>();
      viewportImg.color = new Color(0f, 0f, 0f, 0.01f);
      viewportGo.AddComponent<RectMask2D>();

      var contentGo = new GameObject("Content");
      contentGo.transform.SetParent(viewportGo.transform, false);
      var contentRect = contentGo.AddComponent<RectTransform>();
      contentRect.anchorMin = new Vector2(0f, 1f);
      contentRect.anchorMax = new Vector2(1f, 1f);
      contentRect.pivot = new Vector2(0.5f, 1f);
      contentRect.anchoredPosition = Vector2.zero;
      contentRect.sizeDelta = new Vector2(0f, 960f);

      for (var i = 0; i < 14; i++)
      {
        var rowGo = new GameObject($"Row_{i}");
        rowGo.transform.SetParent(contentGo.transform, false);
        var rowRect = rowGo.AddComponent<RectTransform>();
        rowRect.anchorMin = new Vector2(0f, 1f);
        rowRect.anchorMax = new Vector2(1f, 1f);
        rowRect.pivot = new Vector2(0.5f, 1f);
        rowRect.anchoredPosition = new Vector2(0f, -i * 64f);
        rowRect.sizeDelta = new Vector2(0f, 56f);
        var rowImg = rowGo.AddComponent<Image>();
        rowImg.color = i % 2 == 0
          ? new Color(0.14f, 0.20f, 0.30f, 0.95f)
          : new Color(0.18f, 0.25f, 0.38f, 0.95f);
      }

      m_ScrollRect = panelGo.AddComponent<ScrollRect>();
      m_ScrollRect.viewport = viewportRect;
      m_ScrollRect.content = contentRect;
      m_ScrollRect.horizontal = false;
      m_ScrollRect.vertical = true;
      m_ScrollRect.movementType = ScrollRect.MovementType.Clamped;
    }

    private void OnDestroy()
    {
      Active = false;
      if (m_ZoomScrollAction != null)
      {
        m_ZoomScrollAction.Disable();
        m_ZoomScrollAction.Dispose();
        m_ZoomScrollAction = null;
      }
    }
  }
}
