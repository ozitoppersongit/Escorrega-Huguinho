using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class HuguinhoController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float normalSpeed = 4f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float slideSpeed = 12f;
    [SerializeField] private float cellSize = 1f; // Tamanho de cada bloco da grade
    [SerializeField] private float stopCooldown = 0.5f;

    [Header("Detecção de Camadas")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask iceLayer;

    private bool isMoving = false;
    private bool isSliding = false;
    private bool canMove = true;

    private void Update()
    {
        // Impede qualquer input se já estiver se movendo ou em cooldown
        if (!canMove || isMoving) return;

        Vector2 input = GetInput();
        if (input != Vector2.zero)
        {
            Vector3 direction = new Vector3(input.x, 0, input.y).normalized;
            StartCoroutine(MoveRoutine(direction));
        }
    }

    // Lê os inputs WASD usando o Novo Input System de forma direta
    private Vector2 GetInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return Vector2.zero;

        if (keyboard.wKey.wasPressedThisFrame) return Vector2.up;
        if (keyboard.sKey.wasPressedThisFrame) return Vector2.down;
        if (keyboard.aKey.wasPressedThisFrame) return Vector2.left;
        if (keyboard.dKey.wasPressedThisFrame) return Vector2.right;

        return Vector2.zero;
    }

    private IEnumerator MoveRoutine(Vector3 direction)
    {
        isMoving = true;

        // Rotaciona o personagem instantaneamente para a direção do movimento
        transform.rotation = Quaternion.LookRotation(direction);

        bool keepMoving = true;

        while (keepMoving)
        {
            Vector3 targetPosition = transform.position + direction * cellSize;

            // 1. Detectar Obstáculo (Parede) antes de mover para a próxima casa
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, cellSize, obstacleLayer))
            {
                if (isSliding)
                {
                    isSliding = false;
                    yield return StartCoroutine(CooldownRoutine());
                }
                break; // Interrompe o movimento imediatamente
            }

            // 2. Verificar tipo de terreno na posição alvo (Gelo ou Normal)
            bool isOnIce = CheckTerrain(targetPosition, iceLayer);

            // 3. Determinar velocidade e regras de corrida (Sprint)
            float currentSpeed = normalSpeed;
            if (isOnIce)
            {
                isSliding = true;
                currentSpeed = slideSpeed;
            }
            else
            {
                isSliding = false;
                // Apenas permite correr com Shift se NÃO estiver no gelo
                if (Keyboard.current.shiftKey.isPressed)
                {
                    currentSpeed = sprintSpeed;
                }
            }

            // 4. Mover suavemente até a célula alvo
            yield return MoveToCell(targetPosition, currentSpeed);

            // Se não for gelo, ele para após dar um passo
            if (!isOnIce)
            {
                keepMoving = false;
            }
        }

        isMoving = false;
    }

    private IEnumerator MoveToCell(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = target; // Garante o alinhamento perfeito na grade
    }

    private IEnumerator CooldownRoutine()
    {
        canMove = false;
        yield return new WaitForSeconds(stopCooldown);
        canMove = true;
    }

    private bool CheckTerrain(Vector3 position, LayerMask layer)
    {
        // Atira um raio para baixo a partir do centro da célula alvo para detectar o chão
        return Physics.Raycast(position + Vector3.up * 0.5f, Vector3.down, 1.5f, layer);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Passa de fase ao colidir com o Trigger de tag "Oto"
        if (other.CompareTag("Oto"))
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Não há mais fases configuradas no Build Settings!");
        }
    }
}
