using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gerencia a criação, armazenamento e acesso ao grid de nós para pathfinding.
/// </summary>
public class GridManager : MonoBehaviour
{
    public LayerMask unwalkableMask;    // Layer que contém os obstáculos.
    public Transform plane;             // O plano que define a área do grid.
    public float nodeRadius;            // O raio de cada nó, define a resolução.
    Node[,] grid;                       // O array 2D que armazena o grid.

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        // Calcula as dimensões do grid com base no tamanho do plano e no diâmetro do nó.
        gridSizeX = Mathf.RoundToInt(plane.localScale.x * 10 / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(plane.localScale.z * 10 / nodeDiameter);
        CreateGrid();
    }

    /// <summary>
    /// Cria o grid de nós, verificando cada posição em busca de obstáculos.
    /// </summary>
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * plane.localScale.x * 10 / 2 - Vector3.forward * plane.localScale.z * 10 / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                // Verifica se há colisores na camada de obstáculos na posição do nó.
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    /// <summary>
    /// Converte uma posição do mundo em um nó correspondente no grid.
    /// </summary>
    /// <param name="worldPosition">A posição no espaço do mundo.</param>
    /// <returns>O nó do grid na posição especificada.</returns>
    public Node NodeFromWorldPoint(Vector3 worldPosition)
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
    /// <param name="node">O nó central.</param>
    /// <returns>Uma lista de nós vizinhos.</returns>
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public List<Node> path; // Para visualizar o caminho encontrado

    /// <summary>
    /// Desenha gizmos na Scene View para visualização do grid e do caminho.
    /// </summary>
    void OnDrawGizmos()
    {
        // Desenha um wireframe representando a área total do grid.
        Gizmos.DrawWireCube(transform.position, new Vector3(plane.localScale.x * 10, 1, plane.localScale.z * 10));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                // Define a cor do gizmo com base na transitabilidade do nó.
                Gizmos.color = (n.isWalkable) ? Color.white : Color.red;
                if (path != null && path.Contains(n))
                    Gizmos.color = Color.black; // Cor para nós no caminho final.

                // Desenha um cubo no local de cada nó.
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}