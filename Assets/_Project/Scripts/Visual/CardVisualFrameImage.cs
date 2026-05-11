// Assets/_Project/Scripts/Visual/CardVisualFrameImage.cs

using UnityEngine;
using UnityEngine.UI;

namespace CardGame.Visual
{
    [RequireComponent(typeof(Image))]
    public sealed class CardVisualFrameImage : MonoBehaviour
    {
        [Header("Card Visual")]
        [SerializeField] private CardVisualKind visualKind = CardVisualKind.Creature;

        [Header("Image Settings")]
        [SerializeField] private bool preserveAspect = true;
        [SerializeField] private bool applyOnStart = true;

        private Image cachedImage;

        private void Awake()
        {
            cachedImage = GetComponent<Image>();
        }

        private void Start()
        {
            if (applyOnStart)
            {
                Apply();
            }
        }

        public void SetVisualKind(CardVisualKind newVisualKind)
        {
            visualKind = newVisualKind;
            Apply();
        }

        public void Apply()
        {
            if (cachedImage == null)
            {
                cachedImage = GetComponent<Image>();
            }

            CardGameArtTheme theme = CardGameArtThemeProvider.Current;

            if (theme == null)
            {
                Debug.LogWarning(
                    $"[CardVisualFrameImage] Nenhum tema ativo encontrado para '{gameObject.name}'."
                );
                return;
            }

            Sprite frame = theme.GetFrame(visualKind);

            if (frame == null)
            {
                Debug.LogWarning(
                    $"[CardVisualFrameImage] Moldura '{visualKind}' não configurada no tema."
                );
                return;
            }

            cachedImage.sprite = frame;
            cachedImage.preserveAspect = preserveAspect;
        }
    }
}