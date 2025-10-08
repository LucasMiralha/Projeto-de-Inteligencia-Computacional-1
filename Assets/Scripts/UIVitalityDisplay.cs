using UnityEngine;
using UnityEngine.UI;

public class UIVitalityDisplay : MonoBehaviour
{


    [SerializeField] private AgentScript agentVitality;

    [SerializeField] private Slider vitalitySlider;

    

    private void OnEnable()
    {
        if (agentVitality != null)
        {
            ///Inscreve o método UpdateDisplay no evento OnVitalityChanged
            agentVitality.OnVitalityChanged += UpdateDisplay;
            ///Atualiza a UI com o valor inicial
            UpdateDisplay(agentVitality.CurrentVitality, agentVitality.maxVitality);
        }
        else
        {
            Debug.LogError("Referência de AgentVitality não definida no UIVitalityDisplay.", this);
        }
    }

    private void OnDisable()
    {
        if (agentVitality != null)
        {
            ///Desinscreve para evitar vazamentos de memória quando o objeto é desativado/destruído
            agentVitality.OnVitalityChanged -= UpdateDisplay;
        }
    }

    /// <summary>
    /// Este método é chamado pelo evento OnVitalityChanged.
    /// </summary>
    /// <param name="current">A vitalidade atual recebida do evento.</param>
    /// <param name="max">A vitalidade máxima recebida do evento.</param>
    private void UpdateDisplay(float current, float max)
    {
        ///O valor do slider vai de 0 a 1, então normalizamos a vitalidade
        if (max > 0)
        {
            vitalitySlider.value = current / max;
        }
    }
}