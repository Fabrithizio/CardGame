// Caminho: Assets/_Project/Scripts/UI/ResponsiveUIUtility.cs
// Descrição: Utilitários de UI responsiva para o layout mobile vertical do CardGame.

using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public static class ResponsiveUIUtility
    {
        public static readonly Vector2 ReferenceResolution = new Vector2(1080f, 2400f);

        public static Canvas CreateOverlayCanvas(Transform owner, string objectName, int sortingOrder)
        {
            GameObject canvasObject = new GameObject(objectName);
            canvasObject.transform.SetParent(owner, false);

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.65f;

            canvasObject.AddComponent<GraphicRaycaster>();

            RectTransform root = canvasObject.GetComponent<RectTransform>();
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;

            return canvas;
        }

        public static Font GetDefaultFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            if (font == null)
            {
                font = Font.CreateDynamicFontFromOSFont("Arial", 16);
            }

            return font;
        }

        public static RectTransform StretchToParent(GameObject target)
        {
            RectTransform rect = target.GetComponent<RectTransform>();

            if (rect == null)
            {
                rect = target.AddComponent<RectTransform>();
            }

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return rect;
        }

        public static Rect GetSafeAreaInReferenceSpace()
        {
            Rect safeArea = Screen.safeArea;

            float scaleX = ReferenceResolution.x / Mathf.Max(1f, Screen.width);
            float scaleY = ReferenceResolution.y / Mathf.Max(1f, Screen.height);

            return new Rect(
                safeArea.x * scaleX,
                safeArea.y * scaleY,
                safeArea.width * scaleX,
                safeArea.height * scaleY
            );
        }
    }
}
