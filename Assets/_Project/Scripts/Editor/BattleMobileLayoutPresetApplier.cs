// Caminho: Assets/_Project/Scripts/Editor/BattleMobileLayoutPresetApplier.cs
// Descrição: Garante o layout central na cena e remove a dependência de presets soltos por coordenada.

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace CardGame.EditorTools
{
    public static class BattleMobileLayoutPresetApplier
    {
        [MenuItem("Card Game/Layout/Aplicar Layout Central de Batalha")]
        public static void ApplyCentralBattleLayout()
        {
            EnsureObject("BattleScreenLayoutUI", "CardGame.UI.BattleScreenLayoutUI, Assembly-CSharp");
            EnsureObject("BattleBoardSkinUI", "CardGame.UI.BattleBoardSkinUI, Assembly-CSharp");

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            Debug.Log("Layout central de batalha aplicado. Agora os elementos usam zonas proporcionais em vez de posições soltas.");
        }

        [MenuItem("Card Game/Layout/Aplicar Preset Mobile Profissional")]
        public static void ApplyMobilePreset()
        {
            ApplyCentralBattleLayout();
        }

        private static void EnsureObject(string objectName, string typeName)
        {
            if (GameObject.Find(objectName) != null)
            {
                return;
            }

            GameObject created = new GameObject(objectName);
            System.Type componentType = System.Type.GetType(typeName);

            if (componentType != null)
            {
                created.AddComponent(componentType);
                Debug.Log($"{objectName} criado automaticamente.");
            }
            else
            {
                Debug.LogWarning($"{objectName} ainda não compilou. Rode o menu novamente depois da compilação.");
            }
        }
    }
}

#endif
