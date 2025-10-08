using System.Collections.Generic;
using UnityEngine;

public class RecoveryZoneManager : MonoBehaviour
{
    ///Padrão Singleton para garantir uma única instância
    public static RecoveryZoneManager Instance { get; private set; }

    public readonly List<RecoveryZone> _activeZones = new List<RecoveryZone>();

    private void Awake()
    {
        ///Implementação do Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Adiciona uma zona à lista de zonas ativas.
    /// </summary>
    public void RegisterZone(RecoveryZone zone)
    {
        if (!_activeZones.Contains(zone))
        {
            _activeZones.Add(zone);
        }
    }

    /// <summary>
    /// Remove uma zona da lista de zonas ativas.
    /// </summary>
    public void DeregisterZone(RecoveryZone zone)
    {
        if (_activeZones.Contains(zone))
        {
            _activeZones.Remove(zone);
        }
    }

    /// <summary>
    /// Encontra e retorna a zona de recuperação mais próxima da posição fornecida.
    /// </summary>
    /// <param name="agentPosition">A posição a partir da qual procurar.</param>
    /// <returns>O componente RecoveryZone mais próximo, ou null se não houver zonas ativas.</returns>
    public RecoveryZone GetNearestZone(Vector3 agentPosition)
    {
        if (_activeZones.Count == 0)
        {
            return null;
        }

        RecoveryZone nearestZone = null;
        float minSqrDistance = float.MaxValue;

        foreach (RecoveryZone zone in _activeZones)
        {
            float sqrDistance = (zone.transform.position - agentPosition).sqrMagnitude;
            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                nearestZone = zone;
            }
        }

        return nearestZone;
    }
}