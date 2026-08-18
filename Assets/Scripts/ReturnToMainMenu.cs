using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Nova API de Input padrão do Unity 6

public class ReturnToMainMenu : MonoBehaviour
{
    [Header("Configurações de Navegação")]
    [Tooltip("Cena da tela inicial.")]
    [SerializeField] private string mainMenuSceneName;

    void Update()
    {
        // Verifica se o teclado está ativo e se a tecla Esc foi pressionada neste frame
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            VoltarParaMenu();
        }
    }

    private void VoltarParaMenu()
    {
        // Carrega a cena do menu principal
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
