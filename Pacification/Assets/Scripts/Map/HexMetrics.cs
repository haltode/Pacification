using UnityEngine;

public static class HexMetrics
{
    public const float OuterRadius = 10f;
    public const float InnerRadius = OuterRadius * 0.866025404f;

    public const int ChunkSizeX = 5;
    public const int ChunkSizeZ = 5;

    public const float ElevationStep = 5f;
    public const int MaxElevationReach = 1;

    public const float WaterElevationOffset = 0.5f;

    public const int HashGrideSize = 256;
    static float[] hashGrid;

    // Blending colored regions factors
    public const float SolidFactor = 0.75f;
    public const float BlendFactor = 1f - SolidFactor;

    static Vector3[] corners =
    {
        new Vector3(0f, 0f, OuterRadius),
        new Vector3(InnerRadius, 0f, 0.5f * OuterRadius),
        new Vector3(InnerRadius, 0f, -0.5f * OuterRadius),
        new Vector3(0f, 0f, -OuterRadius),
        new Vector3(-InnerRadius, 0f, -0.5f * OuterRadius),
        new Vector3(-InnerRadius, 0f, 0.5f * OuterRadius),
        // Wrap back because the last vertex needs the first corner
        new Vector3(0f, 0f, OuterRadius)
    };

    public static Vector3 GetFirstCorner(HexDirection dir)
    {
        return corners[(int) dir];
    }

    public static Vector3 GetSecondCorner(HexDirection dir)
    {
        return corners[(int) dir + 1];
    }

    public static Vector3 GetFirstSolidCorner(HexDirection dir)
    {
        return corners[(int) dir] * SolidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection dir)
    {
        return corners[(int) dir + 1] * SolidFactor;
    }

    public static Vector3 GetBridge(HexDirection dir)
    {
        return (corners[(int) dir] + corners[(int) dir + 1]) * BlendFactor;
    }

    public static void InitializeHashGrid(int seed)
    {
        hashGrid = new float[HashGrideSize * HashGrideSize];
        Random.State current = Random.state;
        Random.InitState(seed);
        for(int i = 0; i < hashGrid.Length; ++i)
            hashGrid[i] = Random.value;
        Random.state = current;
    }

    public static float SampleHashGrid(Vector3 position)
    {
        int x = (int) position.x % HashGrideSize;
        if(x < 0)
            x += HashGrideSize;
        int z = (int) position.z % HashGrideSize;
        if(z < 0)
            z += HashGrideSize;
        return hashGrid[x + z * HashGrideSize];
    }
}