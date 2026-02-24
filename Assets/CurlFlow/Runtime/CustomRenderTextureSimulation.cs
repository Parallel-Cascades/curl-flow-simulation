using UnityEngine;

namespace CurlFlow.Runtime
{
    [RequireComponent(typeof(MeshRenderer))]
    public class CustomRenderTextureSimulation : MonoBehaviour
    {
        [SerializeField] private CustomRenderTexture m_customRenderTexture;
        
        private MeshRenderer m_meshRenderer;
        
        private void Awake()
        {
            m_meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            if (m_meshRenderer.isVisible)
            {
                m_customRenderTexture.Update();
            }
        }

        [ContextMenu("Restart Simulation")]
        public void RestartSimulation()
        {
            m_customRenderTexture.Initialize();
        }
    }
}