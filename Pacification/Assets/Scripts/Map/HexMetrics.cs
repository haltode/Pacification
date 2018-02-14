﻿using UnityEngine;

public static class HexMetrics
{
    public const float OuterRadius = 10f;
    public const float InnerRadius = OuterRadius * 0.866025404f;

    public const int ChunkSizeX = 5;
    public const int ChunkSizeZ = 5;

    public const float ElevationStep = 5f;
    public const int MaxRoadElevation = 1;

    // Blending colored regions factors
    public const float SolidFactor = 0.75f;
    public const float BlendFactor = 1f - SolidFactor;

    public const float RoadSizeFactor = 0.25f;

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
}