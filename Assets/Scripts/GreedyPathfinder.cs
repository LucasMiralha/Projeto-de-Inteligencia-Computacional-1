using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Implementa o algoritmo A* para encontrar o caminho mais curto em um grid.
/// </summary>
public class GreedyPathfinder : MonoBehaviour
{
    GreedyGridManager greedyGridManager;

    [HideInInspector]
    public List<GreedyNode> openSet_ForGizmos;
    [HideInInspector]
    public HashSet<GreedyNode> closedSet_ForGizmos;
    [HideInInspector]
    public List<GreedyNode> finalPath_ForGizmos;

    void Awake()
    {
        greedyGridManager = GetComponent<GreedyGridManager>();
    }

    /// <summary>
    /// Encontra e retorna um caminho de uma posição inicial para uma final.
    /// </summary>
    /// <param name="startPos">A posição inicial no mundo.</param>
    /// <param name="targetPos">A posição final no mundo.</param>
    /// <returns>Uma lista de nós representando o caminho, ou null se nenhum caminho for encontrado.</returns>
    public List<GreedyNode> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        GreedyNode startGreedyNode = greedyGridManager.greedyNodeFromWorldPoint(startPos);
        GreedyNode targetGreedyNode = greedyGridManager.greedyNodeFromWorldPoint(targetPos);

        if (!startGreedyNode.isWalkable || !targetGreedyNode.isWalkable)
        {
            return null; ///Se não é possível encontrar um caminho se o início ou o fim não forem caminháveis
        }

        List<GreedyNode> openSet = new List<GreedyNode>();
        HashSet<GreedyNode> closedSet = new HashSet<GreedyNode>();
        openSet.Add(startGreedyNode);

        while (openSet.Count > 0)
        {
            GreedyNode currentGreedyNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentGreedyNode.fCost || (openSet[i].fCost == currentGreedyNode.fCost && openSet[i].hCost < currentGreedyNode.hCost))
                {
                    currentGreedyNode = openSet[i];
                }
            }

            openSet.Remove(currentGreedyNode);
            closedSet.Add(currentGreedyNode);

            if (currentGreedyNode == targetGreedyNode)
            {
                openSet_ForGizmos = openSet;
                closedSet_ForGizmos = closedSet;
                return RetracePath(startGreedyNode, targetGreedyNode);
            }

            foreach (GreedyNode neighbour in greedyGridManager.GetNeighbours(currentGreedyNode))
            {
                if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentGreedyNode.gCost + GetDistance(currentGreedyNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;           ///Cálculo de custo entre vizinhos g(n) (10 ou 14(diagonal))
                    neighbour.hCost = GetDistance(neighbour, targetGreedyNode);   ///Cálculo de custo heuristico é feito na hora pegando a distância entre o nó visualizado e o nó final
                    neighbour.parent = currentGreedyNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return null; ///Retorna null se o loop terminar e nenhum caminho for encontrado
        openSet_ForGizmos = openSet;
        closedSet_ForGizmos = closedSet;
        finalPath_ForGizmos = null;
    }

    /// <summary>
    /// Reconstrói e retorna o caminho final a partir dos nós pais.
    /// </summary>
    List<GreedyNode> RetracePath(GreedyNode startGreedyNode, GreedyNode endGreedyNode)
    {
        List<GreedyNode> path = new List<GreedyNode>();
        GreedyNode currentGreedyNode = endGreedyNode;

        while (currentGreedyNode != startGreedyNode)
        {
            path.Add(currentGreedyNode);
            currentGreedyNode = currentGreedyNode.parent;
        }
        path.Reverse();

        ///Armazena o caminho no GreedyGridManager para visualização com Gizmos
        finalPath_ForGizmos = path;
        return path;
    }

    int GetDistance(GreedyNode GreedyNodeA, GreedyNode GreedyNodeB)
    {
        int dstX = Mathf.Abs(GreedyNodeA.gridX - GreedyNodeB.gridX);
        int dstY = Mathf.Abs(GreedyNodeA.gridY - GreedyNodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}