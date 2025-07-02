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
    readonly float radius = 1f; // Radius of the hexagon

    bool allowWrapAroundEastWest = true; // Allow wrap-around for east-west edges
    bool allowWrapAroundNorthSouth = false; // OPTIONAL: Allow wrap-around for north-south edges, doesn't really make sense

    public Hex(int q, int r)
    {
        Q = q;
        R = r;
        S = -Q - R; // Ensures the hex coordinates are valid
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

        if (allowWrapAroundEastWest)
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

        if (allowWrapAroundNorthSouth)
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
}
