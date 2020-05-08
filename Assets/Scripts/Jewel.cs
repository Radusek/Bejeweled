using UnityEngine;

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
    public Vector2Int GridIndex { get; set; }
    public JewelType Type { get; private set; }


    private void Awake()
    {
        Type = (JewelType)Random.Range(0, (int)JewelType.Count);
    }

    public void OnClick()
    {
        Debug.Log($"Clicked on {Type} at {GridIndex}");
    }
}
