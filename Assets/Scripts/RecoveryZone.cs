using UnityEngine;

public class RecoveryZone : MonoBehaviour
{
    private void OnEnable()
    {
        ///Registra esta zona no manager quando ela se torna ativa
        if (RecoveryZoneManager.Instance != null)
        {
            RecoveryZoneManager.Instance.RegisterZone(this);
        }
    }

    private void OnDisable()
    {
        ///Remove esta zona do manager quando ela é desativada ou destruída
        if (RecoveryZoneManager.Instance != null)
        {
            RecoveryZoneManager.Instance.DeregisterZone(this);
        }
    }
}