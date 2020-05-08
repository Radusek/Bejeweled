using UnityEngine;
using UnityEngine.UI;

public class JewelGrid : MonoBehaviour
{
    [SerializeField]
    private GameObject jewelPrefab;

    [SerializeField]
    private Vector2Int gridSize;

    private Jewel[,] jewelGrid;


    private void Awake()
    {
        jewelGrid = new Jewel[gridSize.x, gridSize.y];

        for (int i = 0; i < gridSize.y; i++)
        {
            for (int j = 0; j < gridSize.x; j++)
            {
                GameObject newJewel = Instantiate(jewelPrefab, transform);
                Jewel jewelScript = newJewel.GetComponent<Jewel>();

                jewelScript.GridIndex = new Vector2Int(i, j);
                jewelGrid[i, j] = jewelScript;
            }
        }

        jewelGrid[2, 0].GetComponent<Image>().color = Color.black;
        jewelGrid[3, 5].GetComponent<Image>().color = Color.yellow;
    }
}
