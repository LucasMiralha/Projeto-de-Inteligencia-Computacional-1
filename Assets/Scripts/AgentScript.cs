using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controla o agente, solicitando caminhos e movendo-o ao longo deles.
/// </summary>
public class AgentScript : MonoBehaviour
{
    public Transform target;        // O alvo que o agente deve seguir.
    public float speed = 5f;        // A velocidade de movimento do agente.
    List<Node> path;
    int targetIndex;
    Pathfinder pathfinder;
    GridManager gridManager;

    void Start()
    {
        pathfinder = FindObjectOfType<Pathfinder>();
        gridManager = FindObjectOfType<GridManager>();
    }

    void Update()
    {
        // Solicita um novo caminho a cada frame (pode ser otimizado).
        pathfinder.FindPath(transform.position, target.position);
        path = gridManager.path;
        if (path != null && path.Count > 0)
        {
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    /// <summary>
    /// Coroutine que move o agente ao longo do caminho encontrado.
    /// </summary>
    IEnumerator FollowPath()
    {
        if (path != null && path.Count > 0)
        {
            Node currentWaypointNode = path[0];
            targetIndex = 0;

            while (true)
            {
                if (transform.position == currentWaypointNode.worldPosition)
                {
                    targetIndex++;
                    if (targetIndex >= path.Count)
                    {
                        yield break; // Fim do caminho
                    }
                    currentWaypointNode = path[targetIndex];
                }

                transform.position = Vector3.MoveTowards(transform.position, currentWaypointNode.worldPosition, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}