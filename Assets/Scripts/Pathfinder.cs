using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Implementa o algoritmo A* para encontrar o caminho mais curto em um grid.
/// </summary>
public class Pathfinder : MonoBehaviour
{
    GridManager gridManager;

    void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    /// <summary>
    /// Inicia o processo de busca de caminho de uma posição inicial para uma final.
    /// </summary>
    /// <param name="startPos">A posição inicial no mundo.</param>
    /// <param name="targetPos">A posição final no mundo.</param>
    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = gridManager.NodeFromWorldPoint(startPos);
        Node targetNode = gridManager.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            int i = 1;
            Node currentNode = openSet[0];
            for (i = 1; i < openSet.Count; i++)
            {
                // Encontra o nó com o menor fCost (ou menor hCost em caso de empate).
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Caminho encontrado
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in gridManager.GetNeighbours(currentNode))
            {
                // Ignora vizinhos que não são caminháveis ou já estão na lista fechada.
                if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                // Se um caminho melhor for encontrado ou o vizinho não estiver na lista aberta
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    /// <summary>
    /// Reconstrói o caminho final a partir dos nós pais.
    /// </summary>
    /// <param name="startNode">O nó inicial.</param>
    /// <param name="endNode">O nó final.</param>
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        gridManager.path = path;
    }

    /// <summary>
    /// Calcula o custo de distância entre dois nós (10 para ortogonal, 14 para diagonal).
    /// </summary>
    /// <param name="nodeA">Primeiro nó.</param>
    /// <param name="nodeB">Segundo nó.</param>
    /// <returns>O custo de movimento inteiro.</returns>
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}