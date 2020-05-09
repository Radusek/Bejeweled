using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class JewelGrid : MonoBehaviour
{
    [SerializeField]
    private GameObject jewelPrefab;

    [Tooltip("X should not be a smaller value than Y")]
    [SerializeField]
    private Vector2Int gridSize;

    [SerializeField]
    private float panelSize = 1000f;

    private Jewel[,] jewelGrid;

    private int swappingAnimations;

    private bool mouseButtonHeld;
    private Vector2Int clickedJewelIndex;

    [SerializeField]
    private float swappingTime = 0.5f;


    private void Awake()
    {
        AdjustGridLayoutGroup();

        jewelGrid = new Jewel[gridSize.y, gridSize.x];

        for (int i = 0; i < gridSize.y; i++)
        {
            for (int j = 0; j < gridSize.x; j++)
            {
                CreateJewel(i, j);
            }
        }
    }

    private void AdjustGridLayoutGroup()
    {
        int gridSizeMax = Mathf.Max(gridSize.x, gridSize.y);
        float cellSize = panelSize / gridSizeMax;
        GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
    }

    private void CreateJewel(int i, int j)
    {
        GameObject newJewel = Instantiate(jewelPrefab, transform);
        Jewel jewelScript = newJewel.GetComponent<Jewel>();

        jewelScript.Grid = this;
        jewelScript.GridIndex = new Vector2Int(i, j);
        jewelGrid[i, j] = jewelScript;
    }

    private void OnValidate()
    {
        if (gridSize.x < gridSize.y)
            gridSize.x = gridSize.y;
    }

    public void JewelClicked(Vector2Int index)
    {
        mouseButtonHeld = true;
        clickedJewelIndex = index;
    }

    public void MouseButtonReleased()
    {
        mouseButtonHeld = false;
    }

    public void JewelPointerEnter(Vector2Int index)
    {
        if (!mouseButtonHeld || swappingAnimations > 0)
            return;

        int vectorsDifference = 0;
        vectorsDifference += Mathf.Abs(index.x - clickedJewelIndex.x);
        vectorsDifference += Mathf.Abs(index.y - clickedJewelIndex.y);

        if (vectorsDifference == 1)
            SwapJewels(index, clickedJewelIndex);

        mouseButtonHeld = false;
    }

    public void SwapJewels(Vector2Int indexA, Vector2Int indexB)
    {
        Jewel jewelA = jewelGrid[indexA.x, indexA.y];
        Jewel jewelB = jewelGrid[indexB.x, indexB.y];

        swappingAnimations = 2;

        jewelA.transform.DOLocalMove(jewelB.transform.localPosition, swappingTime).OnComplete(OnSwappingAnimationCompleted);
        jewelB.transform.DOLocalMove(jewelA.transform.localPosition, swappingTime).OnComplete(OnSwappingAnimationCompleted);

        Vector2Int jewelAIndex = jewelA.GridIndex;
        jewelA.GridIndex = jewelB.GridIndex;
        jewelB.GridIndex = jewelAIndex;

        jewelGrid[indexA.x, indexA.y] = jewelB;
        jewelGrid[indexB.x, indexB.y] = jewelA;
    }

    private void OnSwappingAnimationCompleted()
    {
        swappingAnimations--;
    }
}
