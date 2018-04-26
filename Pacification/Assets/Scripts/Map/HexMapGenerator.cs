using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : MonoBehaviour
{
    public HexGrid grid;
    public int seed;
    public bool useFixedSeed;
    int cellCount;

    [Range(0f, 0.5f)]
    public float jitterProbability = 0.2f;
    [Range(20, 200)]
    public int chunkSizeMin = 70;
    [Range(20, 200)]
    public int chunkSizeMax = 150;
    [Range(5, 95)]
    public int landPercentage = 80;
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

        while(size < chunkSize && !searchQueue.IsEmpty() && budget > 0)
        {
            HexCell current = searchQueue.Dequeue();

            if(current.Elevation + 1 > elevationMaximum)
                continue;
            ++current.Elevation;

            --budget;
            ++size;

            for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
            {
                HexCell neighbor = current.GetNeighbor(dir);
                if(neighbor == null)
                    continue;
                // Slight chance to go back to a visited cell (to create random elevation)
                if(neighbor.Distance != int.MaxValue && Random.value < 0.9)
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
            cell.TerrainBiomeIndex = cell.Elevation / 2;
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