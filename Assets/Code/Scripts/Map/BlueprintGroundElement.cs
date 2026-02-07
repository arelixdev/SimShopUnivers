using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.AI;

public class BlueprintGroundElement : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int gridIndex;
    private bool isSelected;
    public bool IsSelected => isSelected;

    [SerializeField] private GameObject objGround;

    [SerializeField] private Material matUnselected;
    [SerializeField] private Material matSelected;
    [SerializeField] private Material matBuy;

    public bool isMall;


    public bool isBuy;
    public string nameShop;

    private MeshRenderer meshRenderer;
    private float currentOpacity = 1f;

    //TODO savoir quelle magasin l'a acheter peut etre utile pour eviter de faire n'importe quoi avec la pose des portes 

    private void Awake()
    {
        float cellSize = 2.5f;

        gridIndex = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / cellSize),
            Mathf.RoundToInt(transform.position.z / cellSize)
        );

        meshRenderer = objGround.GetComponent<MeshRenderer>();

        meshRenderer.material = new Material(meshRenderer.material);
    }

    public void SetOpacity(float alpha)
    {
        currentOpacity = Mathf.Clamp01(alpha);

        Material mat = meshRenderer.material;
        Color c = mat.color;
        c.a = currentOpacity;
        mat.color = c;

        mat.SetFloat("_Mode", 2); // Fade
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    public void ToogleSelected()
    {
        isSelected = !isSelected;

        if (!isBuy)
        {
            meshRenderer.material = isSelected ? matSelected : matUnselected;
            SetOpacity(currentOpacity); // 🔹 conserve l’opacité
        }
    }

    public void CleanGround()
    {
        objGround.GetComponent<MeshRenderer>().material = matUnselected;
        nameShop = null;
        isBuy = false;
        isSelected = false;
        
    }

    public void GroundBuy(string nsi)
    {
        isSelected = false;
        nameShop = nsi;
        isBuy = true;
        objGround.GetComponent<MeshRenderer>().material = matBuy;
    }

    public List<BlueprintGroundElement> GetNeighbors(Dictionary<Vector2Int, BlueprintGroundElement> grid)
    {
        List<BlueprintGroundElement> neighbors = new();

        Vector2Int[] dirs =
        {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
        };

        foreach (var dir in dirs)
        {
            Vector2Int check = gridIndex + dir;
            if (grid.TryGetValue(check, out var neighbor))
            {
                if (neighbor != this)
                    neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    // Optionnel : world position
    public Vector3 WorldPosition(float cellSize = 2.5f)
    {
        return new Vector3(gridIndex.x * cellSize, 0, gridIndex.y * cellSize);
    }
}
