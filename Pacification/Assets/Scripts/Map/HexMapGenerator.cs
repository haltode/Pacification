using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : MonoBehaviour
{
    const int MaxIteration = 10000;

    public HexGrid grid;
    public int seed;
    public bool useFixedSeed;
    int cellCount;
    
    struct MapRegion {
        public int xMin, xMax, zMin, zMax;
    }

    List<MapRegion> regions;

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
    [Range(0, 10)]
    public int mapBorderX = 5;
    [Range(0, 10)]
    public int mapBorderZ = 5;
    [Range(0, 10)]
    public int regionBorder = 5;
    [Range(1, 4)]
    public int regionCount = 1;

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

        CreateRegions();
        CreateLand();
        SetTerrainType();
        AddOcean();

        grid.ResetDistances();

        Random.state = originalRandomState;
    }

    void CreateRegions()
    {
        if(regions == null)
            regions = new List<MapRegion>();
        else
            regions.Clear();

        MapRegion region;
        switch(regionCount)
        {
            default:
                region.xMin = mapBorderX;
                region.xMax = grid.cellCountX - mapBorderX;
                region.zMin = mapBorderZ;
                region.zMax = grid.cellCountZ - mapBorderZ;
                regions.Add(region);
            break;
            case 2:
                if(Random.value < 0.5f)
                {
                    region.xMin = mapBorderX;
                    region.xMax = grid.cellCountX / 2 - regionBorder;
                    region.zMin = mapBorderZ;
                    region.zMax = grid.cellCountZ - mapBorderZ;
                    regions.Add(region);
                    region.xMin = grid.cellCountX / 2 + regionBorder;
                    region.xMax = grid.cellCountX - mapBorderX;
                    regions.Add(region);
                }
                else
                {
                    region.xMin = mapBorderX;
                    region.xMax = grid.cellCountX - mapBorderX;
                    region.zMin = mapBorderZ;
                    region.zMax = grid.cellCountZ / 2 - regionBorder;
                    regions.Add(region);
                    region.zMin = grid.cellCountZ / 2 + regionBorder;
                    region.zMax = grid.cellCountZ - mapBorderZ;
                    regions.Add(region);
                }
            break;
            case 3:
                region.xMin = mapBorderX;
                region.xMax = grid.cellCountX / 3 - regionBorder;
                region.zMin = mapBorderZ;
                region.zMax = grid.cellCountZ - mapBorderZ;
                regions.Add(region);
                region.xMin = grid.cellCountX / 3 + regionBorder;
                region.xMax = grid.cellCountX * 2 / 3 - regionBorder;
                regions.Add(region);
                region.xMin = grid.cellCountX * 2 / 3 + regionBorder;
                region.xMax = grid.cellCountX - mapBorderX;
                regions.Add(region);
            break;
            case 4:
                region.xMin = mapBorderX;
                region.xMax = grid.cellCountX / 2 - regionBorder;
                region.zMin = mapBorderZ;
                region.zMax = grid.cellCountZ / 2 - regionBorder;
                regions.Add(region);
                region.xMin = grid.cellCountX / 2 + regionBorder;
                region.xMax = grid.cellCountX - mapBorderX;
                regions.Add(region);
                region.zMin = grid.cellCountZ / 2 + regionBorder;
                region.zMax = grid.cellCountZ - mapBorderZ;
                regions.Add(region);
                region.xMin = mapBorderX;
                region.xMax = grid.cellCountX / 2 - regionBorder;
                regions.Add(region);
            break;
        }
    }

    void CreateLand()
    {
        int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
        for(int guard = 0; guard < MaxIteration; ++guard)
        {
            for(int i = 0; i < regions.Count; i++)
            {
                MapRegion region = regions[i];
                int chunkSize = Random.Range(chunkSizeMin, chunkSizeMax - 1);
                landBudget = RaiseTerrain(chunkSize, landBudget, region);
                if(landBudget <= 0)
                    return;
            }
        }

        if(landBudget > 0)
            Debug.LogWarning("Failed to use up " + landBudget + " land budget.");
    }

    int RaiseTerrain(int chunkSize, int budget, MapRegion region)
    {
        HexCell firstCell = GetRandomCell(region);
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

    HexCell GetRandomCell(MapRegion region)
    {
        return grid.GetCell(Random.Range(region.xMin, region.xMax), 
                            Random.Range(region.zMin, region.zMax));
    }
}