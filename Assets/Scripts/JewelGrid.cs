using DG.Tweening;
using UnityEngine;

public class JewelGrid : MonoBehaviour
{
    [SerializeField]
    private GameObject jewelPrefab;

    [SerializeField]
    private Vector2Int gridSize;

    private Jewel[,] jewelGrid;

    private int swappingAnimations;

    private bool mouseButtonHeld;
    private Vector2Int clickedJewelIndex;

    [SerializeField]
    private float swappingTime = 0.5f;


    private void Awake()
    {
        jewelGrid = new Jewel[gridSize.x, gridSize.y];

        for (int i = 0; i < gridSize.y; i++)
        {
            for (int j = 0; j < gridSize.x; j++)
            {
                CreateJewel(i, j);
            }
        }
    }

    private void CreateJewel(int i, int j)
    {
        GameObject newJewel = Instantiate(jewelPrefab, transform);
        Jewel jewelScript = newJewel.GetComponent<Jewel>();

        jewelScript.Grid = this;
        jewelScript.GridIndex = new Vector2Int(i, j);
        jewelGrid[i, j] = jewelScript;
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
