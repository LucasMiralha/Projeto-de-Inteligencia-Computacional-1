using UnityEngine;
using System;

public class AgentVitality : MonoBehaviour
{


    [SerializeField]  private float maxVitality = 100f;


    [SerializeField]  private float decayRatePerSecond = 2.5f;


    [SerializeField]  private float regenerationRatePerSecond = 10f;

    // Propriedade pública para acessar a vitalidade atual (somente leitura de fora)
    public float CurrentVitality { get; private set; }

    // Propriedade pública para acessar a vitalidade máxima
    public float MaxVitality => maxVitality;

    // Evento para notificar outros scripts sobre mudanças na vitalidade
    public event Action<float, float> OnVitalityChanged;

    private void Awake()
    {
        // Inicializa a vitalidade no máximo ao iniciar
        CurrentVitality = maxVitality;
    }

    private void Update()
    {
        // Aplica o decaimento contínuo, independente da taxa de quadros
        if (CurrentVitality > 0)
        {
            float oldVitality = CurrentVitality;
            CurrentVitality -= decayRatePerSecond * Time.deltaTime;
            CurrentVitality = Mathf.Clamp(CurrentVitality, 0f, maxVitality);

            // Se o valor mudou, invoca o evento
            if (CurrentVitality != oldVitality)
            {
                OnVitalityChanged?.Invoke(CurrentVitality, maxVitality);
            }
        }
    }

    /// <summary>
    /// Método público para ser chamado quando o agente está em uma zona de recuperação.
    /// </summary>
    public void Regenerate()
    {
        if (CurrentVitality < maxVitality)
        {
            float oldVitality = CurrentVitality;
            CurrentVitality += regenerationRatePerSecond * Time.deltaTime;
            CurrentVitality = Mathf.Clamp(CurrentVitality, 0f, maxVitality);

            // Se o valor mudou, invoca o evento
            if (CurrentVitality != oldVitality)
            {
                OnVitalityChanged?.Invoke(CurrentVitality, maxVitality);
            }
        }
    }

    /// <summary>
    /// Propriedade para verificar facilmente se a vitalidade se esgotou.
    /// </summary>
    public bool IsDepleted => CurrentVitality <= 0;
}