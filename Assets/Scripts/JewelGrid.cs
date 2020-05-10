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

    private Jewel[] lastSwappedJewels;

    private bool playerInducedAnimation;

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

        lastSwappedJewels = new Jewel[2];
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
        jewelScript.GridIndex = new Vector2Int(j, i);
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

    public void SwapJewels(Vector2Int indexA, Vector2Int indexB, bool playerInduced = true)
    {
        Jewel jewelA = jewelGrid[indexA.y, indexA.x];
        Jewel jewelB = jewelGrid[indexB.y, indexB.x];

        swappingAnimations = 2;
        playerInducedAnimation = playerInduced;

        jewelA.transform.DOLocalMove(jewelB.transform.localPosition, swappingTime).OnComplete(OnSwappingAnimationCompleted);
        jewelB.transform.DOLocalMove(jewelA.transform.localPosition, swappingTime).OnComplete(OnSwappingAnimationCompleted);

        Vector2Int jewelAIndex = jewelA.GridIndex;
        jewelA.GridIndex = jewelB.GridIndex;
        jewelB.GridIndex = jewelAIndex;

        jewelGrid[indexA.y, indexA.x] = jewelB;
        jewelGrid[indexB.y, indexB.x] = jewelA;

        lastSwappedJewels[0] = jewelA;
        lastSwappedJewels[1] = jewelB;
    }

    private void OnSwappingAnimationCompleted()
    {
        swappingAnimations--;

        if (playerInducedAnimation && swappingAnimations == 0)
        {
            if (IsMatchFound(lastSwappedJewels[0]) || IsMatchFound(lastSwappedJewels[1]))
            {
                Debug.Log("Match Found!");
            }
            else
            {
                SwapJewels(lastSwappedJewels[0].GridIndex, lastSwappedJewels[1].GridIndex, false);
                Debug.Log("No match found.");
            }
        }
    }

    private bool IsMatchFound(Jewel jewel)
    {
        Vector2Int jewelIndex = jewel.GridIndex;
        JewelType jewelType = jewel.Type;

        for (int i = 0; i < 2; i++)
        {
            int maxJewelsInARow = 0;
            int jewelsInARow = 0;

            for (int j = jewelIndex[i] - 2; j <= jewelIndex[i] + 2; j++)
            {
                if (j < 0 || j >= gridSize[i])
                    continue;

                Jewel currentJewel = i == 0 ? jewelGrid[jewelIndex.y, j] : jewelGrid[j, jewelIndex.x];

                if (currentJewel.Type == jewelType)
                {
                    jewelsInARow++;

                    if (jewelsInARow > maxJewelsInARow)
                        maxJewelsInARow = jewelsInARow;
                }
                else
                    jewelsInARow = 0;
            }

            if (maxJewelsInARow >= 3)
                return true;
        }

        return false;
    }
}
