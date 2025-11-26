using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleHowToUI : MonoBehaviour
{
    static BattleHowToUI _i;
    public static void Show(string message, Action onClose)
    {
        if (_i == null)
        {
            var go = new GameObject("BattleHowToUI");
            _i = go.AddComponent<BattleHowToUI>();
            DontDestroyOnLoad(go);
        }
        _i.Build(message, onClose);
    }

    Canvas canvas;
    GameObject blocker, panel;
    Button closeBtn;
    Action _onClose;

    void Build(string message, Action onClose)
    {
        _onClose = onClose;

        if (!canvas)
        {
            var cgo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            cgo.transform.SetParent(transform, false);
            canvas = cgo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = cgo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            blocker = new GameObject("Dim", typeof(Image));
            blocker.transform.SetParent(canvas.transform, false);
            var dim = blocker.GetComponent<Image>();
            dim.color = new Color(0, 0, 0, 0.45f);
            var drt = dim.rectTransform; drt.anchorMin = Vector2.zero; drt.anchorMax = Vector2.one; drt.offsetMin = drt.offsetMax = Vector2.zero;

            panel = new GameObject("Panel", typeof(Image));
            panel.transform.SetParent(canvas.transform, false);
            var pimg = panel.GetComponent<Image>(); pimg.color = new Color(0.12f,0.12f,0.12f,0.96f);
            var prt = pimg.rectTransform; prt.sizeDelta = new Vector2(680, 220);
            prt.anchorMin = prt.anchorMax = new Vector2(0.5f, 0.5f); prt.anchoredPosition = Vector2.zero;

            // Title
            var titleGO = new GameObject("Title", typeof(Text));
            titleGO.transform.SetParent(panel.transform, false);
            var title = titleGO.GetComponent<Text>();
            title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            title.text = "How to Play";
            title.alignment = TextAnchor.MiddleCenter;
            title.fontSize = 30; title.color = Color.white;
            var trt = title.rectTransform; trt.anchorMin = new Vector2(0.1f,0.65f); trt.anchorMax = new Vector2(0.9f,0.92f); trt.offsetMin = trt.offsetMax = Vector2.zero;

            // Body
            var bodyGO = new GameObject("Body", typeof(Text));
            bodyGO.transform.SetParent(panel.transform, false);
            var body = bodyGO.GetComponent<Text>();
            body.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            body.alignment = TextAnchor.UpperCenter;
            body.fontSize = 20; body.color = new Color(1,1,1,0.93f);
            var brt = body.rectTransform; brt.anchorMin = new Vector2(0.08f,0.22f); brt.anchorMax = new Vector2(0.92f,0.64f); brt.offsetMin = brt.offsetMax = Vector2.zero;

            // Close
            var btnGO = new GameObject("CloseButton", typeof(Image), typeof(Button));
            btnGO.transform.SetParent(panel.transform, false);
            var bimg = btnGO.GetComponent<Image>(); bimg.color = new Color(0.25f,0.55f,0.85f,1f);
            closeBtn = btnGO.GetComponent<Button>();
            var brt2 = bimg.rectTransform; brt2.sizeDelta = new Vector2(140, 44);
            brt2.anchorMin = brt2.anchorMax = new Vector2(0.5f, 0.12f); brt2.anchoredPosition = Vector2.zero;

            var btxtGO = new GameObject("Text", typeof(Text));
            btxtGO.transform.SetParent(btnGO.transform, false);
            var btxt = btxtGO.GetComponent<Text>();
            btxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            btxt.text = "Close";
            btxt.alignment = TextAnchor.MiddleCenter;
            btxt.fontSize = 20; btxt.color = Color.white;
            var btr = btxt.rectTransform; btr.anchorMin = btr.anchorMax = new Vector2(0.5f,0.5f); btr.sizeDelta = new Vector2(140,44);

            closeBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.AddListener(Close);
        }

        var bodyRef = panel.transform.Find("Body")?.GetComponent<Text>();
        if (bodyRef) bodyRef.text = message;

        canvas.gameObject.SetActive(true);
        blocker.SetActive(true);
        panel.SetActive(true);

        // (Optional) freeze input under popup
        Cursor.visible = true; Cursor.lockState = CursorLockMode.None;
    }

    void Close()
    {
        if (panel) panel.SetActive(false);
        if (blocker) blocker.SetActive(false);
        _onClose?.Invoke();
    }
}
