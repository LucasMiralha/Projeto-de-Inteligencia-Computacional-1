using UnityEngine;

/// <summary>
/// Representa um único nó no grid de pathfinding.
/// Contém informações sobre sua posição, transitabilidade e custos para o algoritmo A*.
/// </summary>
public class GreedyNode
{
    public bool isWalkable;         ///Este nó é um obstáculo?
    public Vector3 worldPosition;   ///Posição no mundo 3D no centro do nó.
    public int gridX;               ///Coordenada X do nó no grid.
    public int gridY;               ///Coordenada Y do nó no grid.

    public int gCost;               ///Custo do caminho desde o início até este nó.
    public int hCost;               ///Custo heurístico estimado deste nó até o destino.
    public GreedyNode parent;             ///Nó anterior no caminho, usado para reconstrução.

    /// <summary>
    /// Construtor para criar uma nova instância de Nó.
    /// </summary>
    /// <param name="_isWalkable">Se o nó pode ser atravessado.</param>
    /// <param name="_worldPos">A posição do nó no espaço do mundo.</param>
    /// <param name="_gridX">A coordenada X do nó no grid.</param>
    /// <param name="_gridY">A coordenada Y do nó no grid.</param>
    public GreedyNode(bool _isWalkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        isWalkable = _isWalkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    /// <summary>
    /// Propriedade calculada para obter o custo F total do nó.
    /// O custo F é a soma do custo G e do custo H.
    /// </summary>
    public int fCost
    {
        get
        {
            return hCost;
        }
    }
}