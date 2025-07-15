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

    public GameObject ForestPrefab;
    public GameObject JunglePrefab;


    public Material MatOcean;
    public Material MatPlains;
    public Material MatGrassland;
    public Material MatJungle;
    public Material MatMountain;
    public Material MatDesert;

    public float HeightMountain = 1f;
    public float HeightHill = 0.6f;
    public float HeightFlat = 0.0f;
    public float MoistureJungle = 1f;
    public float MoistureForest = 0.5f;
    public float MoistureGrasslands = 0f;
    public float MoisturePlains = -0.75f;

    public readonly int NumRows = 30;
    public readonly int NumColumns = 60;

    public readonly bool allowWrapAroundEastWest = true; // Allow wrap-around for east-west edges
    public readonly bool allowWrapAroundNorthSouth = false; // OPTIONAL: Allow wrap-around for north-south edges, doesn't really make sense

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
                Hex hex = new(column, row, this)
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

                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();

                if (hex.Elevation >= HeightFlat && hex.Elevation < HeightMountain)
                {
                    if (hex.Moisture >= MoistureJungle)
                    {
                        mr.material = MatJungle;
                        // Spawn trees
                        Vector3 p = hexGO.transform.position;
                        if (hex.Elevation >= HeightHill)
                        {
                            p.y += 0.25f;
                        }
                        Instantiate(JunglePrefab, p, Quaternion.identity, hexGO.transform);
                    }
                    else if (hex.Moisture >= MoistureForest)
                    {
                        mr.material = MatGrassland;
                        // Spawn trees
                        Vector3 p = hexGO.transform.position;
                        if (hex.Elevation >= HeightHill)
                        {
                            p.y += 0.25f;
                        }
                        Instantiate(ForestPrefab, p, Quaternion.identity, hexGO.transform);
                    }
                    else if (hex.Moisture >= MoistureGrasslands)
                    {
                        mr.material = MatGrassland;
                    }
                    else if (hex.Moisture >= MoisturePlains)
                    {
                        mr.material = MatPlains;
                    }
                    else
                    {
                        mr.material = MatDesert;
                    }
                }

                if (hex.Elevation >= HeightMountain)
                {
                    mr.material = MatMountain;
                    mf.mesh = MeshMountain;
                }
                else if (hex.Elevation >= HeightHill)
                {
                    mf.mesh = MeshHill;
                }
                else if (hex.Elevation >= HeightFlat)
                {
                    mf.mesh = MeshFlat;
                }
                else
                {
                    mr.material = MatOcean;
                    mf.mesh = MeshWater;
                }
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
