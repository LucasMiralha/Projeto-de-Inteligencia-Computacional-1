using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gerencia o comportamento do agente
/// 
/// Notas:
/// Os gizmos para visualização dos caminhos entre diferentes agentes de diferentes algoritmos
/// podem se sobrepor, apagando o gizmos anterior e dando display no mais recente
/// </summary>
public class AgentScript : MonoBehaviour
{
    ///Utilizado para definir os estados possíveis do agente
    public enum AgentState
    {
        Idle,
        SeekingPrimaryObjective,
        SeekingRecovery,
        Lost,
        Set
    }

    public GameObject _gameObject;

    [Header("Algoritmo de Busca")]
    public PathfindingAlgorithm searchAlgorithm = PathfindingAlgorithm.AStar;

    [Header("Objetivos e Comportamento")]
    public Transform primaryObjective;

    public float lowVitalityThreshold = 30f;

    public float highVitalityThreshold = 95f;
    public float speed = 5f;

    [Header("Gestão de Vitalidade")]
    public float maxVitality = 100f;
    public float decayRatePerSecond = 2.5f;
    public float regenerationRatePerSecond = 10f;

    ///Propriedade pública para aceder à vitalidade atual
    public float CurrentVitality { get; private set; }

    ///Evento para notificar a UI sobre mudanças na vitalidade
    public event Action<float, float> OnVitalityChanged;

    public Pathfinder _pathfinder;

    ///Gestão de estado e caminho
    private AgentState _currentState;
    private bool _isInsideRecoveryZone = false;
    private List<Node> _currentPath;
    private Coroutine _followPathCoroutine;

    private void Awake()
    {
        CurrentVitality = maxVitality;
    }

    private void Start()
    {
        if (primaryObjective == null)
        {
            Debug.LogError("Objetivo não definido! Entrando no estado Idle", this);
            EnterState(AgentState.Idle);
        }
        else
        {
            EnterState(AgentState.SeekingPrimaryObjective);
        }
    }

    private void Update()
    {
        if (_currentState == AgentState.Lost)
        {
            ///Morre :(
            speed = 0;
            transform.rotation = Quaternion.Euler(new Vector3(180,0,0));
            _gameObject.transform.position = new Vector3(transform.position.x, transform.position.y+0.1f, transform.position.z);
        }

        if (_currentState == AgentState.Set)
        {
            speed = 0;
            CurrentVitality = 100;
        }

        ///Gestão do agente
        HandleVitality();
        UpdateStateMachine();
    }

    private void HandleVitality()
    {
        float oldVitality = CurrentVitality;

        if (_isInsideRecoveryZone)
        {
            ///Regenera se estiver dentro de uma zona
            CurrentVitality += regenerationRatePerSecond * Time.deltaTime;
        }
        else
        {
            ///Decai se estiver fora
            CurrentVitality -= decayRatePerSecond * Time.deltaTime;
        }

        CurrentVitality = Mathf.Clamp(CurrentVitality, 0f, maxVitality);

        ///Se o valor mudou, notifica os ouvintes (como a UI)
        if (CurrentVitality != oldVitality)
        {
            OnVitalityChanged?.Invoke(CurrentVitality, maxVitality);
        }
    }

    private void UpdateStateMachine()
    {
        if (CurrentVitality <= 0 && _currentState != AgentState.Lost)
        {
            EnterState(AgentState.Lost);
            return;
        }

        switch (_currentState)
        {
            case AgentState.SeekingPrimaryObjective:
                if (CurrentVitality < lowVitalityThreshold)
                {
                    EnterState(AgentState.SeekingRecovery);
                }
                break;

            case AgentState.SeekingRecovery:
                if (CurrentVitality >= highVitalityThreshold)
                {
                    EnterState(AgentState.SeekingPrimaryObjective);
                }
                break;
        }
    }

    private void EnterState(AgentState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;
        Debug.Log($"Agente entrou no estado: {newState}");

        if (_followPathCoroutine != null)
        {
            StopCoroutine(_followPathCoroutine);
        }

        Vector3? targetPosition = GetTargetPositionForState(newState);

        if (targetPosition.HasValue)
        {
            _currentPath = _pathfinder.FindPath(transform.position, targetPosition.Value, searchAlgorithm);
            _followPathCoroutine = StartCoroutine(FollowPath());
        }
        else
        {
            _currentPath = null;
        }
    }

    private Vector3? GetTargetPositionForState(AgentState state)
    {
        switch (state)
        {
            case AgentState.SeekingPrimaryObjective:
                return primaryObjective.position;

            case AgentState.SeekingRecovery:
                RecoveryZone nearestZone = RecoveryZoneManager.Instance.GetNearestZone(transform.position);
                if (nearestZone != null)
                {
                    return nearestZone.transform.position;
                }
                else
                {
                    Debug.LogWarning("Precisa de recuperação, mas nenhuma zona encontrada!");
                    return null; ///Retorna nulo se não houver para onde ir
                }

            case AgentState.Idle:
            case AgentState.Lost:
            default:
                return null;
        }
    }

    IEnumerator FollowPath()
    {
        if (_currentPath == null || _currentPath.Count == 0)
        {
            yield break;
        }

        int targetIndex = 0;
        while (true)
        {
            Node currentWaypoint = _currentPath[targetIndex];
            if (Vector3.Distance(transform.position, currentWaypoint.worldPosition) < 0.1f)
            {
                targetIndex++;
                if (targetIndex >= _currentPath.Count)
                {
                    yield break; ///Fim do caminho
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, _currentPath[targetIndex].worldPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RecoveryZone>() != null)
        {
            _isInsideRecoveryZone = true;
        }
        if (other.gameObject.CompareTag("Finish"))
        {
            EnterState(AgentState.Set);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<RecoveryZone>() != null)
        {
            _isInsideRecoveryZone = false;
        }
    }
}