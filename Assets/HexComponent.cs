using UnityEngine;

public class HexComponent : MonoBehaviour
{
    public Hex hex;
    public HexMap hexMap;
    public void UpdatePosition()
    {
        transform.position = hex.PositionFromCamera(Camera.main.transform.position, hexMap.NumRows, hexMap.NumColumns);
    }
}
