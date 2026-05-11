// Assets/_Project/Scripts/Visual/CardGameArtThemeProvider.cs

using UnityEngine;

namespace CardGame.Visual
{
    public sealed class CardGameArtThemeProvider : MonoBehaviour
    {
        public static CardGameArtTheme Current { get; private set; }

        [Header("Theme")]
        [SerializeField] private CardGameArtTheme theme;

        [Header("Behaviour")]
        [SerializeField] private bool dontDestroyOnLoad = true;
        [SerializeField] private bool validateOnAwake = true;

        private void Awake()
        {
            if (theme == null)
            {
                Debug.LogError(
                    "[CardGameArtThemeProvider] Nenhum CardGameArtTheme foi atribuído."
                );
                return;
            }

            Current = theme;

            if (validateOnAwake)
            {
                theme.ValidateTheme();
            }

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        public void SetTheme(CardGameArtTheme newTheme)
        {
            if (newTheme == null)
            {
                Debug.LogWarning("[CardGameArtThemeProvider] Tentou definir tema nulo.");
                return;
            }

            theme = newTheme;
            Current = theme;
            theme.ValidateTheme();
        }
    }
}