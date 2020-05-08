using UnityEngine;
using UnityEngine.UI;

public enum JewelType
{
    Emerald,
    Sapphire,
    Diamond,
    Ruby,
    Count //często dodaję Count na koniec enuma żeby móc uzyskać liczbę elementów tworzących enuma
}

public class Jewel : MonoBehaviour
{
    [SerializeField]
    private Color[] jewelColors;

    public JewelGrid Grid { get; set; }
    public Vector2Int GridIndex { get; set; }
    public JewelType Type { get; private set; }

    

    private void Awake()
    {
        Type = (JewelType)Random.Range(0, (int)JewelType.Count);
        GetComponent<Image>().color = jewelColors[(int)Type];
    }

    public void OnClick()
    {
        Grid.JewelClicked(GridIndex);

        Debug.Log($"Clicked on {Type} at {GridIndex}");
    }

    public void OnMouseButtonRelease()
    {
        Grid.MouseButtonReleased();
    }

    public void OnMousePointerEnter()
    {
        Grid.JewelPointerEnter(GridIndex);
    }
}
