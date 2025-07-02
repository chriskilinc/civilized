using Unity.VisualScripting;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    public GameObject HexPrefab;
    public Material[] HexMaterials;

    public readonly int NumRows = 20;
    public readonly int NumColumns = 40;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        // Logic to generate a hexagonal map of specified width and height
        Debug.Log($"Generating a hex map of size {NumColumns}x{NumRows}");

        for (int col = 0; col < NumColumns; col++)
        {
            for (int row = 0; row < NumRows; row++)
            {
                Hex hex = new(col, row);
                Vector3 pos = hex.PositionFromCamera(Camera.main.transform.position, NumRows, NumColumns);

                GameObject hexGO = Instantiate(HexPrefab, pos, Quaternion.identity, transform);
                hexGO.GetComponent<HexComponent>().hex = hex;
                hexGO.GetComponent<HexComponent>().hexMap = this;

                MeshRenderer meshRenderer = hexGO.GetComponentInChildren<MeshRenderer>();
                meshRenderer.material = HexMaterials[Random.Range(0, HexMaterials.Length)];
            }
        }

        // StaticBatchingUtility.Combine(gameObject);
    }
}
