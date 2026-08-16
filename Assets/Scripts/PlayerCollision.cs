using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    [Header("Configurações de Colisão")]
    [Tooltip("A Tag configurada no objeto do buraco.")]
    [SerializeField] private string holeTag = "Buraco";

    // Este método é chamado automaticamente pelo Unity quando o Huguinho entra em um Trigger
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto em que entramos tem a tag "Buraco"
        if (other.CompareTag(holeTag))
        {
            ResetarFase();
        }
    }

    private void ResetarFase()
    {
        // Obtém o índice da cena ativa atual
        int cenaAtual = SceneManager.GetActiveScene().buildIndex;
        
        // Recarrega a cena atual
        SceneManager.LoadScene(cenaAtual);
    }
}
