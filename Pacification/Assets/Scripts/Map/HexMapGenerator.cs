using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : MonoBehaviour
{
    public HexGrid grid;
    public int seed;
    public bool useFixedSeed;
    int cellCount;

    [Range(0f, 0.5f)]
    public float jitterProbability = 0.25f;
    [Range(20, 200)]
    public int chunkSizeMin = 30;
    [Range(20, 200)]
    public int chunkSizeMax = 100;
    [Range(5, 95)]
    public int landPercentage = 50;
    [Range(0f, 1f)]
    public float highRiseProbability = 0.25f;
    [Range(6, 10)]
    public int elevationMaximum = 8;

    public void GenerateMap(int sizeX, int sizeZ)
    {
        Random.State originalRandomState = Random.state;
        if(!useFixedSeed)
        {
            seed = Random.Range(0, int.MaxValue);
            seed ^= (int)System.DateTime.Now.Ticks;
            seed ^= (int)Time.unscaledTime;
            // Ensure the seed is positive
            seed &= int.MaxValue;
        }
        Random.InitState(seed);

        cellCount = sizeX * sizeZ;
        grid.CreateMap(sizeX, sizeZ);

        CreateLand();
        SetTerrainType();
        AddOcean();

        for(int i = 0; i < cellCount; ++i)
            grid.GetCell(i).Distance = int.MaxValue;

        Random.state = originalRandomState;
    }

    void CreateLand()
    {
        int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
        while(landBudget > 0)
        {
            landBudget = RaiseTerrain(
                Random.Range(chunkSizeMin, chunkSizeMax + 1), landBudget);
        }
    }

    int RaiseTerrain(int chunkSize, int budget)
    {
        HexCell firstCell = GetRandomCell();
        firstCell.Distance = 0;
        firstCell.SearchHeuristic = 0;
        PriorityQueue<HexCell> searchQueue = new PriorityQueue<HexCell>(HexCell.CompareCells);
        searchQueue.Enqueue(firstCell);

        HexCoordinates center = firstCell.coordinates;
        int size = 0;
        int rise = Random.value < highRiseProbability ? 2 : 1;

        grid.ClearPath();

        while(size < chunkSize && !searchQueue.IsEmpty())
        {
            HexCell current = searchQueue.Dequeue();
            int newElevation = current.Elevation + rise;
            if(newElevation > elevationMaximum)
                continue;

            current.Elevation = newElevation;
            if(newElevation >= 1)
            {
                --budget;
                if(budget == 0)
                    break;
            }
            ++size;

            for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
            {
                HexCell neighbor = current.GetNeighbor(dir);
                if(neighbor == null || neighbor.Distance != int.MaxValue)
                    continue;

                neighbor.Distance = neighbor.coordinates.DistanceTo(center);
                neighbor.SearchHeuristic = Random.value < jitterProbability ? 1 : 0;
                searchQueue.Enqueue(neighbor);
            }
        }

        return budget;
    }

    void SetTerrainType()
    {
        for(int i = 0; i < cellCount; ++i)
        {
            HexCell cell = grid.GetCell(i);
            cell.TerrainBiomeIndex = cell.Elevation;
        }
    }

    void AddOcean()
    {
        for(int i = 0; i < cellCount; ++i)
        {
            HexCell cell = grid.GetCell(i);
            if(cell.Elevation == 0)
                cell.IsUnderWater = true;
        }
    }

    HexCell GetRandomCell()
    {
        return grid.GetCell(Random.Range(0, cellCount));
    }
}