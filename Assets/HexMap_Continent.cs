using UnityEngine;

public class HexMap_Continent : HexMap
{
	override public void GenerateMap()
	{
		base.GenerateMap();

		Debug.Log($"Generating a [CONTINENT] hex map of size {NumColumns}x{NumRows}");

		// Generate raised area
		int numContintents = Random.Range(2, 3);
		Debug.Log($"Generating {numContintents} continents");
		int continentSpacing = NumColumns / numContintents;

		// Random.InitState(0); // For reproducibility, remove in production

		for (int c = 0; c < numContintents; c++)
		{
			int numSplats = Random.Range(4, 8);
			for (int i = 0; i < numSplats; i++)
			{
				int range = Random.Range(5, 8);
				int r = Random.Range(range, NumRows - range);
				int q = Random.Range(0, 10) - r / 2 + (c * continentSpacing);

				ElevateArea(q, r, range);
			}
		}

		// Add lumpiness using Perlin noise
		float noiseScale = 2f;  // Larger values create more pronounced islands and lakes
		float noiseResolution = 0.1f;
		Vector2 noiseOffset = new(Random.Range(0f, 1f), Random.Range(0f, 1f));

		for (int column = 0; column < NumColumns; column++)
		{
			for (int row = 0; row < NumRows; row++)
			{
				Hex hex = GetHexAt(column, row);
				if (hex != null)
				{
					float noise = Mathf.PerlinNoise(
						((float)column / Mathf.Max(NumColumns, NumRows) / noiseResolution) + noiseOffset.x,
						((float)row / Mathf.Max(NumColumns, NumRows) / noiseResolution) + noiseOffset.y
						) - 0.5f;

					hex.Elevation += noise * noiseScale;
				}
			}
		}

		// Set meshes and materials for different hex types based on height
		// Simulate rainfall and temperature to determine hex types
		UpdateHexVisuals();
	}

	void ElevateArea(int q, int r, int range, float centerHeight = 0.8f)
	{
		Hex centerHex = GetHexAt(q, r);
		Hex[] hexesInRange = GetHexesWithinRangeOf(centerHex, range);

		foreach (Hex hex in hexesInRange)
		{
			if (hex != null)
			{
				hex.Elevation = centerHeight * Mathf.Lerp(1f, 0.25f, Mathf.Pow(Hex.Distance(centerHex, hex) / range, 2f));
			}
		}
	}
}

// https://youtu.be/eOYGOD1GnDI?si=Qml9AUosPrnrt6EU&t=809