using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
/// <summary>
/// Gerencia a criação, armazenamento e acesso ao grid de nós para pathfinding.
/// </summary>
public class GreedyGridManager : MonoBehaviour
{
    public bool displayGridGizmos;
    public GreedyPathfinder greedypathfinder;       ///Pathfinder do metodo
    public LayerMask unwalkableMask;    ///Layer que contém os obstáculos.
    public Transform plane;             ///O plano que define a área do grid.
    public float greedyNodeRadius;            ///O raio de cada nó, define a resolução.
    GreedyNode[,] grid;                       ///O array 2D que armazena o grid.

    float greedyNodeDiameter;
    int gridSizeX, gridSizeY;

    void Awake()
    {
        greedyNodeDiameter = greedyNodeRadius * 2;
        // Calcula as dimensões do grid com base no tamanho do plano e no diâmetro do nó.
        gridSizeX = Mathf.RoundToInt(plane.localScale.x * 10 / greedyNodeDiameter);
        gridSizeY = Mathf.RoundToInt(plane.localScale.z * 10 / greedyNodeDiameter);
        CreateGrid();
    }

    void Update()
    {
        ///Habilitar essa linha para poder mudar o ambiente em tempo real
        ///e torna-lo responsivo na visão do agente
        ///CreateGrid();
    }

    /// <summary>
    /// Cria o grid de nós, verificando cada posição em busca de obstáculos.
    /// </summary>
    void CreateGrid()
    {
        grid = new GreedyNode[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * plane.localScale.x * 10 / 2 - Vector3.forward * plane.localScale.z * 10 / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * greedyNodeDiameter + greedyNodeRadius) + Vector3.forward * (y * greedyNodeDiameter + greedyNodeRadius);
                // Verifica se há colisores na camada de obstáculos na posição do nó.
                bool walkable = !(Physics.CheckSphere(worldPoint, greedyNodeRadius, unwalkableMask));
                grid[x, y] = new GreedyNode(walkable, worldPoint, x, y);
            }
        }
    }

    /// <summary>
    /// Converte uma posição do mundo em um nó correspondente no grid.
    /// </summary>
    /// <param name="worldPosition">A posição no espaço do mundo.</param>
    /// <returns>O nó do grid na posição especificada.</returns>
    public GreedyNode greedyNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x - transform.position.x + plane.localScale.x * 10 / 2) / (plane.localScale.x * 10);
        float percentY = (worldPosition.z - transform.position.z + plane.localScale.z * 10 / 2) / (plane.localScale.z * 10);
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    /// <summary>
    /// Obtém os nós vizinhos de um dado nó.
    /// </summary>
    /// <param name="greedyNode">O nó central.</param>
    /// <returns>Uma lista de nós vizinhos.</returns>
    public List<GreedyNode> GetNeighbours(GreedyNode greedyNode)
    {
        List<GreedyNode> neighbours = new List<GreedyNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = greedyNode.gridX + x;
                int checkY = greedyNode.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public List<GreedyNode> path; // Para visualizar o caminho encontrado

    /// <summary>
    /// Desenha gizmos na Scene View para visualização do grid e do caminho.
    /// </summary>
    void OnDrawGizmos()
    {
        if (!displayGridGizmos)
            return;

        ///Desenha um wireframe representando a área total do grid.
        Gizmos.DrawWireCube(transform.position, new Vector3(plane.localScale.x * 10, 1, plane.localScale.z * 10));

        if (grid != null && greedypathfinder != null)
        {
            // Obtém as listas de visualização do Pathfinder
            List<GreedyNode> openSet = greedypathfinder.openSet_ForGizmos;
            HashSet<GreedyNode> closedSet = greedypathfinder.closedSet_ForGizmos;
            List<GreedyNode> path = greedypathfinder.finalPath_ForGizmos;

            // Desenha o Closed Set (vermelho)
            if (closedSet != null)
            {
                Gizmos.color = Color.red;
                foreach (GreedyNode n in closedSet)
                {
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (greedyNodeDiameter - .1f));
                }
            }

            // Desenha o Open Set (amarelo)
            if (openSet != null)
            {
                Gizmos.color = Color.yellow;
                foreach (GreedyNode n in openSet)
                {
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (greedyNodeDiameter - .1f));
                }
            }

            // Desenha o Caminho Final (verde)
            if (path != null)
            {
                Gizmos.color = Color.green;
                foreach (GreedyNode n in path)
                {
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (greedyNodeDiameter - .1f));
                }
            }
        }
    }
}