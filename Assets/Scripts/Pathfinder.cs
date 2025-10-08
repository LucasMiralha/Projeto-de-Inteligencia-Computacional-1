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
    /// Encontra e retorna um caminho de uma posição inicial para uma final.
    /// </summary>
    /// <param name="startPos">A posição inicial no mundo.</param>
    /// <param name="targetPos">A posição final no mundo.</param>
    /// <returns>Uma lista de nós representando o caminho, ou null se nenhum caminho for encontrado.</returns>
    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = gridManager.NodeFromWorldPoint(startPos);
        Node targetNode = gridManager.NodeFromWorldPoint(targetPos);

        if (!startNode.isWalkable || !targetNode.isWalkable)
        {
            return null; ///Se não é possível encontrar um caminho se o início ou o fim não forem caminháveis
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in gridManager.GetNeighbours(currentNode))
            {
                if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;           ///Cálculo de custo entre vizinhos g(n) (10 ou 14(diagonal))
                    neighbour.hCost = GetDistance(neighbour, targetNode);   ///Cálculo de custo heuristico é feito na hora pegando a distância entre o nó visualizado e o nó final
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return null; ///Retorna null se o loop terminar e nenhum caminho for encontrado
    }

    /// <summary>
    /// Reconstrói e retorna o caminho final a partir dos nós pais.
    /// </summary>
    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        ///Armazena o caminho no GridManager para visualização com Gizmos
        gridManager.path = path;
        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}