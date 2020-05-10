using DG.Tweening;
using System.Collections.Generic;
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

    private List<Jewel> matchingJewels;


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
        matchingJewels = new List<Jewel>();
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
            matchingJewels.Clear();
            matchingJewels.InsertRange(0, GetMatchingJewels(lastSwappedJewels[0]));
            matchingJewels.InsertRange(0, GetMatchingJewels(lastSwappedJewels[1]));

            if (matchingJewels.Count > 0)
            {
                foreach (var matchingJewel in matchingJewels)
                {
                    matchingJewel.MakeMatched();
                    //funkcja może być wywołana dwukrotnie dla pary klejnotów tego samego typu po ich zamianie, nie będzie się to więcej zdarzać gdy zrobimy kasowanie wszystkich istniejących rzędów na planszy
                }
                Debug.Log("Match Found!");
            }
            else
            {
                SwapJewels(lastSwappedJewels[0].GridIndex, lastSwappedJewels[1].GridIndex, false);
                Debug.Log("No match found.");
            }
        }
    }

    private List<Jewel> GetMatchingJewels(Jewel jewel)
    {
        List<Jewel> resultJewels = new List<Jewel>();

        Vector2Int jewelIndex = jewel.GridIndex;
        JewelType jewelType = jewel.Type;

        for (int i = 0; i < 2; i++)
        {
            List<Jewel> tmpJewels = new List<Jewel>();

            for (int j = jewelIndex[i] - 2; j <= jewelIndex[i] + 2; j++)
            {
                if (j < 0 || j >= gridSize[i])
                    continue;

                Jewel currentJewel = i == 0 ? jewelGrid[jewelIndex.y, j] : jewelGrid[j, jewelIndex.x];

                if (currentJewel.Type == jewelType)
                {
                    tmpJewels.Add(currentJewel);
                }
                else
                {
                    if (tmpJewels.Count < 3)
                        tmpJewels.Clear();
                    else
                    {
                        AddTmpToResultJewels(resultJewels, tmpJewels);
                        break;
                    }
                }
            }

            if (tmpJewels.Count >= 3)
                AddTmpToResultJewels(resultJewels, tmpJewels);
        }

        return resultJewels;
    }

    private static void AddTmpToResultJewels(List<Jewel> resultJewels, List<Jewel> tmpJewels)
    {
        foreach (var tmpJewel in tmpJewels)
        {
            if (!resultJewels.Contains(tmpJewel))
                resultJewels.Add(tmpJewel);
        }

        tmpJewels.Clear();
    }
}
