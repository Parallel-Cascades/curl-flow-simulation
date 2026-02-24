using CurlFlow.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CurlFlow.Editor
{
    [CustomEditor(typeof(CustomRenderTextureSimulation))]
    public class CustomRenderTextureSimulationEditor : UnityEditor.Editor
    {
        private MaterialEditor m_materialEditor;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            AddRestartButton(root);

            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            
            // This draws the custom render texture material inspector below the simulation component, which
            // allows us to adjust the material without having to select the render texture asset every time
            var customRenderTextureProperty = serializedObject.FindProperty("m_customRenderTexture");
            var customRenderTexture = customRenderTextureProperty.objectReferenceValue as CustomRenderTexture;

            if (customRenderTexture == null || customRenderTexture.material == null)
                return root;
            
            AddRenderTextureMaterialInspector(customRenderTexture, root);

            return root;
        }

        private void AddRenderTextureMaterialInspector(CustomRenderTexture customRenderTexture, VisualElement root)
        {
            var material = customRenderTexture.material;
            if (m_materialEditor == null)
            {
                m_materialEditor = UnityEditor.Editor.CreateEditor(material) as MaterialEditor;
            }
            
            var container = new IMGUIContainer(() =>
            {
                m_materialEditor.DrawHeader();
                EditorGUILayout.BeginVertical();
                m_materialEditor.OnInspectorGUI();
                EditorGUILayout.EndVertical();
            });
            
            container.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            root.Add(container);
        }

        private void AddRestartButton(VisualElement root)
        {
            var button = new Button(() =>
            {
                var simulation = target as CustomRenderTextureSimulation;
                simulation.RestartSimulation();
            })
            {
                text = "Restart Simulation"
            };
            root.Add(button);
        }
    }
}