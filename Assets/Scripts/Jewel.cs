using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    public void MakeMatched()
    {
        transform.DOScale(0f, 0.5f).OnComplete(RegainBaseScale);
    }

    //funkcja pomocnicza dopóki nie mamy spadania klejnotów powyżej tych usuniętych
    private void RegainBaseScale()
    {
        transform.DOScale(1f, 0.5f);
    }
}
