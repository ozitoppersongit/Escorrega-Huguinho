using UnityEngine;

namespace Teste.Camera
{
    [AddComponentMenu("Teste/Camera Follow")]
    [SelectionBase]
    public sealed class CameraFollow : MonoBehaviour
    {
        [Header("Alvo")]
        [Tooltip("Arraste o Huguinho para cá.")]
        [SerializeField] private Transform target;

        [Header("Configurações de Distância")]
        [Tooltip("Distância constante que a câmera manterá do Huguinho.")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -10f);

        [Header("Suavização")]
        [Range(0.01f, 1f)]
        [Tooltip("Quanto menor o valor, mais rápido a câmera segue o alvo.")]
        [SerializeField] private float smoothTime = 0.2f;

        private Vector3 _currentVelocity = Vector3.zero;

        private void LateUpdate()
        {
            // Impede erros no console caso o alvo não esteja definido
            if (target == null) return;

            // Calcula a posição final desejada mantendo a rotação fixa (apenas aplicando o offset)
            Vector3 targetPosition = target.position + offset;

            // Aplica a interpolação suave (SmoothDamp)
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                targetPosition, 
                ref _currentVelocity, 
                smoothTime
            );
        }
    }
}
