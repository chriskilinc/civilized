using System;
using UnityEngine;

//  Defines grid position, world space position, size
//  Does not Interact with Unity
public class Hex
{
    // Q + R + S = 0
    // S = -(Q + R)
    public readonly int Q;  // Column
    public readonly int R;  // Row
    public readonly int S;

    readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;
    readonly float radius = 1f;

    // Data for map generation
    public float Elevation;
    public float Moisture;
    private readonly HexMap _hexMap;    // Maybe there is a more elegant way to do this (maybe add a settings class or something)

    public Hex(int q, int r, HexMap hexMap)
    {
        Q = q;
        R = r;
        S = -Q - R; // Ensures the hex coordinates are valid
        _hexMap = hexMap;
    }

    public float HexHeight()
    {
        return radius * 2;
    }

    public float HexWidth()
    {
        return WIDTH_MULTIPLIER * HexHeight();
    }

    public float HexVerticalSpacing()
    {
        return HexHeight() * 0.75f; // Vertical spacing between hexes
    }

    public float HexHorizontalSpacing()
    {
        return HexWidth(); // Horizontal spacing between hexes
    }

    // World space position of the hex
    public Vector3 Position()
    {
        return new Vector3(
            HexHorizontalSpacing() * (Q + R / 2f),
            0f,
            HexVerticalSpacing() * R
        );
    }

    public Vector3 PositionFromCamera(Vector3 cameraPosition, float numRows, float numColumns)
    {
        Vector3 position = Position();

        float mapHeight = numRows * HexVerticalSpacing();
        float mapWidth = numColumns * HexHorizontalSpacing();

        if (_hexMap.allowWrapAroundEastWest)
        {
            float wfc = (position.x - cameraPosition.x) / mapWidth;

            if (wfc > 0)
            {
                wfc += 0.5f;
            }
            else
            {
                wfc -= 0.5f;
            }

            int wtf = (int)wfc;
            position.x -= wtf * mapWidth;
        }

        if (_hexMap.allowWrapAroundNorthSouth)
        {
            float hfc = (position.z - cameraPosition.z) / mapHeight;

            if (hfc > 0)
            {
                hfc += 0.5f;
            }
            else
            {
                hfc -= 0.5f;
            }

            int htf = (int)hfc;
            position.z -= htf * mapHeight;
        }

        return position;
    }

    /// <summary>
    /// Calculates the distance between two hexes using axial coordinates.
    /// This method accounts for optional wrap-around logic on the Q axis (east-west) and R axis (north-south),
    /// </summary>
    public static float Distance(Hex a, Hex b)
    {
        int dq = Mathf.Abs(a.Q - b.Q);
        int dr = Mathf.Abs(a.R - b.R);
        int ds = Mathf.Abs(a.S - b.S);

        // Use HexMap settings for wrap-around
        if (a._hexMap.allowWrapAroundEastWest)
        {
            dq = Mathf.Min(dq, a._hexMap.NumColumns - dq);
        }
        if (a._hexMap.allowWrapAroundNorthSouth)
        {
            dr = Mathf.Min(dr, a._hexMap.NumRows - dr);
        }

        return Mathf.Max(dq, dr, ds);
    }
}
