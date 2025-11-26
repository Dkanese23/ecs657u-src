using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    static WinUI _instance;

    public static void Show(string title = "Congrats! You Win 🎉",
                            string note  = "Boss coming later…")
    {
        if (_instance == null)
        {
            var go = new GameObject("WinUI");
            _instance = go.AddComponent<WinUI>();
            DontDestroyOnLoad(go);
        }
        _instance.Build(title, note);
    }

    Canvas canvas;
    GameObject blocker;
    Button quitBtn;

    void Build(string title, string note)
    {
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
            dim.color = new Color(0, 0, 0, 0.6f);
            var dimRT = dim.rectTransform; dimRT.anchorMin = Vector2.zero; dimRT.anchorMax = Vector2.one;
            dimRT.offsetMin = Vector2.zero; dimRT.offsetMax = Vector2.zero;

            var panel = new GameObject("Panel", typeof(Image));
            panel.transform.SetParent(canvas.transform, false);
            var panelImg = panel.GetComponent<Image>(); panelImg.color = new Color(0.12f,0.12f,0.12f,0.95f);
            var prt = panelImg.rectTransform; prt.sizeDelta = new Vector2(560, 300);
            prt.anchorMin = prt.anchorMax = new Vector2(0.5f, 0.5f); prt.anchoredPosition = Vector2.zero;

            var titleGO = new GameObject("Title", typeof(Text));
            titleGO.transform.SetParent(panel.transform, false);
            var titleTxt = titleGO.GetComponent<Text>();
            titleTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleTxt.alignment = TextAnchor.MiddleCenter; titleTxt.fontSize = 36; titleTxt.color = Color.white;
            var trt = titleTxt.rectTransform; trt.anchorMin = new Vector2(0.1f,0.7f); trt.anchorMax=new Vector2(0.9f,0.93f);

            var noteGO = new GameObject("Note", typeof(Text));
            noteGO.transform.SetParent(panel.transform, false);
            var noteTxt = noteGO.GetComponent<Text>();
            noteTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            noteTxt.alignment = TextAnchor.UpperCenter; noteTxt.fontSize = 20; noteTxt.color = new Color(1f,1f,1f,0.85f);
            var nrt = noteTxt.rectTransform; nrt.anchorMin = new Vector2(0.12f,0.40f); nrt.anchorMax=new Vector2(0.88f,0.62f);

            var btnGO = new GameObject("QuitButton", typeof(Image), typeof(Button));
            btnGO.transform.SetParent(panel.transform, false);
            var btnImg = btnGO.GetComponent<Image>(); btnImg.color = new Color(0.25f,0.55f,0.35f,1f);
            var brt = btnImg.rectTransform; brt.sizeDelta = new Vector2(180,48);
            brt.anchorMin = brt.anchorMax = new Vector2(0.5f,0.18f); brt.anchoredPosition = Vector2.zero;

            var btnTextGO = new GameObject("Text", typeof(Text));
            btnTextGO.transform.SetParent(btnGO.transform, false);
            var btnText = btnTextGO.GetComponent<Text>();
            btnText.text = "Quit Game"; btnText.alignment = TextAnchor.MiddleCenter;
            btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); btnText.fontSize = 22; btnText.color = Color.white;
            var btr = btnText.rectTransform; btr.anchorMin = btr.anchorMax = new Vector2(0.5f,0.5f); btr.sizeDelta = new Vector2(180,48);

            quitBtn = btnGO.GetComponent<Button>();
            quitBtn.onClick.RemoveAllListeners();
            quitBtn.onClick.AddListener(QuitGame);
        }

        var titleTxtRef = canvas.transform.Find("Panel/Title")?.GetComponent<Text>();
        var noteTxtRef  = canvas.transform.Find("Panel/Note")?.GetComponent<Text>();
        if (titleTxtRef) titleTxtRef.text = title;
        if (noteTxtRef)  noteTxtRef.text  = note;

        canvas.gameObject.SetActive(true);
        if (blocker) blocker.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
