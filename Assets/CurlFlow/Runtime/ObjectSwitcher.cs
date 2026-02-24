using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CurlFlow.Runtime
{
    public class ObjectSwitcher : MonoBehaviour
    {
        [SerializeField] private CustomRenderTextureSimulation[] m_simulations;
        [SerializeField] private float m_cameraDistance = 1f;
        [SerializeField] private float m_cameraRightOffset = 0f;

        [SerializeField] private Button m_restartButton;
        
        private const float k_transitionDuration = 1f;

        private int m_currentIndex;
        private Camera m_camera;
        private Coroutine m_transitionCoroutine;

        private void Awake()
        {
            m_camera = Camera.main;
        }

        public void SwitchToNextObject()
        {
            m_currentIndex = (m_currentIndex + 1) % m_simulations.Length;
            m_restartButton.onClick.RemoveAllListeners();
            m_restartButton.onClick.AddListener(m_simulations[m_currentIndex].RestartSimulation);
            GoToObject();
        }
        
        public void SwitchToPreviousObject()
        {
            m_currentIndex = (m_currentIndex - 1 + m_simulations.Length) % m_simulations.Length;
            m_restartButton.onClick.RemoveAllListeners();
            m_restartButton.onClick.AddListener(m_simulations[m_currentIndex].RestartSimulation);
            GoToObject();
        }

        private void GoToObject()
        {
            if (m_transitionCoroutine != null)
            {
                StopCoroutine(m_transitionCoroutine);
            }
            
            m_transitionCoroutine = StartCoroutine(TransitionToObject(m_simulations[m_currentIndex].transform));
        }
        
        private IEnumerator TransitionToObject(Transform target)
        {
            Vector3 startPosition = m_camera.transform.position;
            Vector3 targetPosition = target.position + Vector3.back * m_cameraDistance + Vector3.right * m_cameraRightOffset;
            
            float elapsedTime = 0f;
            
            while (elapsedTime < k_transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / k_transitionDuration);
                
                t = t * t * (3f - 2f * t); // Smoothstep
                
                m_camera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                
                yield return null;
            }
            
            m_camera.transform.position = targetPosition;
            m_transitionCoroutine = null;
        }
    }
}