using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    public GameObject HexPrefab;

    public Mesh MeshWater;
    public Mesh MeshFlat;
    public Mesh MeshHill;
    public Mesh MeshMountain;


    public Material MatOcean;
    public Material MatPlains;
    public Material MatGrassland;
    public Material MatMountain;

    // Height above x is y
    public float HeightMountain = 0.85f;
    public float HeightHill = 0.6f;
    public float HeightFlat = 0.0f;

    public readonly int NumRows = 30;
    public readonly int NumColumns = 60;

    // TODO
    readonly bool allowWrapAroundEastWest = true; // Allow wrap-around for east-west edges
    readonly bool allowWrapAroundNorthSouth = false; // OPTIONAL: Allow wrap-around for north-south edges, doesn't really make sense

    private Hex[,] hexes;
    private Dictionary<Hex, GameObject> hexGameObjects;

    public Hex GetHexAt(int x, int y)
    {
        if (hexes == null)
        {
            Debug.LogError("Hexes array is not initialized. Call GenerateMap() first.");
            return null;
        }

        if (allowWrapAroundEastWest)
        {
            x %= NumColumns;
            if (x < 0)
            {
                x += NumColumns;
            }
        }

        if (allowWrapAroundNorthSouth)
        {
            y %= NumRows;
            if (y < 0)
            {
                y += NumRows;
            }
        }

        try
        {
            return hexes[x, y];
        }
        catch
        {
            Debug.LogError("GetHexAt: " + x + "," + y);
            return null;
        }
    }

    void Start()
    {
        GenerateMap();
    }

    /// <summary>
    /// Generates a hexagonal [OCEAN] base map with the specified dimensions.
    /// </summary>
    virtual public void GenerateMap()
    {
        Debug.Log($"Generating a [OCEAN] hex map of size {NumColumns}x{NumRows}");

        hexes = new Hex[NumColumns, NumRows];
        hexGameObjects = new Dictionary<Hex, GameObject>();


        for (int column = 0; column < NumColumns; column++)
        {
            for (int row = 0; row < NumRows; row++)
            {
                Hex hex = new(column, row)
                {
                    Elevation = -0.5f // (ocean)
                };

                hexes[column, row] = hex;

                Vector3 pos = hex.PositionFromCamera(Camera.main.transform.position, NumRows, NumColumns);

                GameObject hexGO = Instantiate(HexPrefab, pos, Quaternion.identity, transform);
                hexGameObjects[hex] = hexGO;

                hexGO.GetComponent<HexComponent>().hex = hex;
                hexGO.GetComponent<HexComponent>().hexMap = this;

                hexGO.GetComponentInChildren<TextMeshPro>().text = $"({hex.Q},{hex.R})";
            }
        }

        // StaticBatchingUtility.Combine(gameObject);
    }

    public void UpdateHexVisuals()
    {
        for (int column = 0; column < NumColumns; column++)
        {
            for (int row = 0; row < NumRows; row++)
            {
                Hex hex = hexes[column, row];
                GameObject hexGO = hexGameObjects[hex];

                MeshRenderer meshRenderer = hexGO.GetComponentInChildren<MeshRenderer>();

                if (hex.Elevation >= HeightMountain)
                {
                    meshRenderer.material = MatMountain;
                }
                else if (hex.Elevation >= HeightHill)
                {
                    meshRenderer.material = MatGrassland;   // TODO
                }
                else if (hex.Elevation >= HeightFlat)
                {
                    meshRenderer.material = MatPlains;
                }
                else
                {
                    meshRenderer.material = MatOcean;
                }

                MeshFilter meshFilter = hexGO.GetComponentInChildren<MeshFilter>();
                meshFilter.mesh = MeshWater;
            }
        }
    }

    public Hex[] GetHexesWithinRangeOf(Hex centerHex, int range)
    {
        if (centerHex == null || range < 0)
        {
            Debug.LogError("HexMap::GetHexesWithinRangeOf cannot be called with centerHex = " + centerHex + " and range = " + range);
            return new Hex[] { };
        }

        List<Hex> results = new();

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++)
            {
                results.Add(GetHexAt(centerHex.Q + dx, centerHex.R + dy));
            }
        }

        return results.ToArray();
    }
}
