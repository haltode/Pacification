using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexMapGenerator : MonoBehaviour
{
    const int MaxIteration = 10000;

    public HexGrid grid;
    int cellCount;
    int seed = 0;
    
    struct MapRegion {
        public int xMin, xMax, zMin, zMax;
    }

    enum MapSize
    {
        TINY,
        SMALL, 
        NORMAL, 
        HUGE, 
        GIGANTIC
    }

    List<MapRegion> regions;

    float jitterProbability = 0.2f;
    int chunkSizeMin = 70;
    int chunkSizeMax = 150;
    int landPercentage = 80;
    int resourcesPercentage = 5;
    int elevationMaximum = 8;
    int regionBorder = 5;
    int regionCount = 1;
    float erosionPercentage = 50;
    int mapBorderX = 5;
    int mapBorderZ = 5;
    MapSize mapsize = MapSize.NORMAL;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void Save()
    {
        MapGeneratorPanel generatorPanel = FindObjectOfType<MapGeneratorPanel>();

        if(GameManager.Instance.gamemode == GameManager.Gamemode.SOLO)
        {
            mapsize = (MapSize)generatorPanel.SlidermapSizeS.value;
            jitterProbability = generatorPanel.SliderjitterProbabilityS.value;
            erosionPercentage = generatorPanel.SlidererosionPercentageS.value;
            chunkSizeMax = (int)generatorPanel.SliderchunkSizeMaxS.value;
            chunkSizeMin = (int)generatorPanel.SliderchunkSizeMinS.value;
            landPercentage = (int)generatorPanel.SliderlandPercentageS.value;
            elevationMaximum = (int)generatorPanel.SliderelevationMaximumS.value;
            regionBorder = (int)generatorPanel.SliderregionBorderS.value;
            regionCount = (int)generatorPanel.SliderregionCountS.value;
            resourcesPercentage = (int)generatorPanel.SliderResourcesS.value;

            if(generatorPanel.TextseedS.text != "")
                seed = int.Parse(generatorPanel.TextseedS.text);
        }
        else if (GameManager.Instance.gamemode == GameManager.Gamemode.MULTI)
        {
            mapsize = (MapSize)generatorPanel.SlidermapSizeM.value;
            jitterProbability = generatorPanel.SliderjitterProbabilityM.value;
            erosionPercentage = generatorPanel.SlidererosionPercentageM.value;
            chunkSizeMax = (int)generatorPanel.SliderchunkSizeMaxM.value;
            chunkSizeMin = (int)generatorPanel.SliderchunkSizeMinM.value;
            landPercentage = (int)generatorPanel.SliderlandPercentageM.value;
            elevationMaximum = (int)generatorPanel.SliderelevationMaximumM.value;
            regionBorder = (int)generatorPanel.SliderregionBorderM.value;
            regionCount = (int)generatorPanel.SliderregionCountM.value;
            resourcesPercentage = (int)generatorPanel.SliderResourcesM.value;

            if(generatorPanel.TextseedM.text != "")
                seed = int.Parse(generatorPanel.TextseedM.text);
        }
    }

    public void GenerateMap()
    {
        int sizeX = 0;
        int sizeZ = 0;
        switch(mapsize)
        {
            case MapSize.TINY:
                sizeX = 30;
                sizeZ = 30;
                break;

            case MapSize.SMALL:
                sizeX = 50;
                sizeZ = 50;
                break;

            case MapSize.NORMAL:
                sizeX = 90;
                sizeZ = 90;
                break;

            case MapSize.HUGE:
                sizeX = 160;
                sizeZ = 160;
                break;

            case MapSize.GIGANTIC:
                sizeX = 180;
                sizeZ = 180;
                break;

            default:
                sizeX = 5;
                sizeZ = 5;
                break;
        }
        GenerateMap(sizeX, sizeZ, seed);
    }

    public void GenerateMap(int sizeX, int sizeZ, int seed)
    {
        Random.State originalRandomState = Random.state;
        grid = FindObjectOfType<HexGrid>();
        if(seed == 0)
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
        ErodeLand();
        SetTerrainType();
        AddOcean();
        AddResources();

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

    // Erosion does not simply remove material, it moves it to the erosion target
    HexCell GetErosionTarget(HexCell cell)
    {
        List<HexCell> candidates = ListPool<HexCell>.Get();
        int erodibleElevation = cell.Elevation - 2;
        for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
        {
            HexCell neighbor = cell.GetNeighbor(dir);
            if(neighbor && neighbor.Elevation <= erodibleElevation)
                candidates.Add(neighbor);
        }
        HexCell target = candidates[Random.Range(0, candidates.Count)];
        ListPool<HexCell>.Add(candidates);
        return target;
    }

    bool IsErodible(HexCell cell)
    {
        int erodibleElevation = cell.Elevation - 2;
        for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
        {
            HexCell neighbor = cell.GetNeighbor(dir);
            if(neighbor && neighbor.Elevation <= erodibleElevation)
                return true;
        }
        return false;
    }

    void ErodeLand()
    {
        List<HexCell> erodibleCells = ListPool<HexCell>.Get();
        for(int i = 0; i < cellCount; ++i)
        {
            HexCell cell = grid.GetCell(i);
            if(IsErodible(cell))
                erodibleCells.Add(cell);
        }

        int targetErodibleCount = (int)(erodibleCells.Count * (100 - erosionPercentage) * 0.01f);
       
        while(erodibleCells.Count > targetErodibleCount)
        {
            int index = Random.Range(0, erodibleCells.Count);
            HexCell cell = erodibleCells[index];
            HexCell targetCell = GetErosionTarget(cell);

            --cell.Elevation;
            ++targetCell.Elevation;

            if(!IsErodible(cell))
            {
                erodibleCells[index] = erodibleCells[erodibleCells.Count - 1];
                erodibleCells.RemoveAt(erodibleCells.Count - 1);
            }

            for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
            {
                HexCell neighbor = cell.GetNeighbor(dir);
                if(neighbor && neighbor.Elevation == cell.Elevation + 2 &&
                    !erodibleCells.Contains(neighbor))
                    erodibleCells.Add(neighbor);       
            }

            if(IsErodible(targetCell) && !erodibleCells.Contains(targetCell))
                erodibleCells.Add(targetCell);

            for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
            {
                HexCell neighbor = targetCell.GetNeighbor(dir);
                if(neighbor && neighbor != cell && 
                    neighbor.Elevation == targetCell.Elevation + 1 &&
                    !IsErodible(neighbor))
                    erodibleCells.Remove(neighbor);
            }
        }

        ListPool<HexCell>.Add(erodibleCells);
    }

    void SetTerrainType()
    {
        for(int i = 0; i < cellCount; ++i)
        {
            HexCell cell = grid.GetCell(i);
            // For low-elevation, randomly choose between desert/plain
            if(cell.Elevation <= 1)
                cell.TerrainBiomeIndex = (Random.value < 0.65f) ? 0 : 1;
            else
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

    int GetMineType()
    {
        float val = Random.value;
        if(val < 0.55f)
            return (int)HexCell.FeatureType.IRON;
        else if(val < 0.85f)
            return (int)HexCell.FeatureType.GOLD;
        else
            return (int)HexCell.FeatureType.DIAMOND; 
    }

    void AddResources()
    {
        int nbValidCell = 0;
        for (int i = 0; i < cellCount; ++i)
            if (!grid.GetCell(i).IsUnderWater)
                nbValidCell++;
        int resourcesBudget = Mathf.RoundToInt(nbValidCell * resourcesPercentage * 0.01f);
        int guard = 0;
        while (resourcesBudget > 0 && guard < MaxIteration)
        {
            HexCell cell = GetRandomValidCell();
            resourcesBudget--;
            if (cell.TerrainBiomeIndex == (int)HexCell.BiomeType.DESERT && cell.IsHill)
            {
                if(Random.value < 0.85f)
                    cell.FeatureIndex = GetMineType();
                else
                    cell.FeatureIndex = (int)HexCell.FeatureType.HORSE;
            }
            else if (cell.TerrainBiomeIndex == (int)HexCell.BiomeType.ROCKY)
                cell.FeatureIndex = GetMineType();
            else if (cell.TerrainBiomeIndex == (int)HexCell.BiomeType.PLAIN)
            {
                float val = Random.value;
                if(val < 0.5f)
                {
                    cell.FeatureIndex = (int)HexCell.FeatureType.FOREST;
                    // Double forest!
                    if(Random.value < 0.4f)
                    {
                        for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
                        {
                            HexCell neighbor = cell.GetNeighbor(dir);
                            if(neighbor && !neighbor.IsUnderWater &&
                               !neighbor.HasFeature && neighbor.Elevation == cell.Elevation)
                            {
                                neighbor.FeatureIndex = (int)HexCell.FeatureType.FOREST;
                                break;
                            }
                        }
                    }
                }
                else if(val < 0.65f)
                    cell.FeatureIndex = (int)HexCell.FeatureType.HORSE;
                else
                    cell.FeatureIndex = (int)HexCell.FeatureType.FOOD;
            }
            else
                resourcesBudget++;
            guard++;
        }
    }

    HexCell GetRandomCell(MapRegion region)
    {
        return grid.GetCell(Random.Range(region.xMin, region.xMax), 
                            Random.Range(region.zMin, region.zMax));
    }

    HexCell GetRandomValidCell()
    {
        HexCell cell;
        do
        {
            cell = grid.GetCell(Random.Range(0, cellCount));
        } while (cell.IsUnderWater || cell.HasFeature ||
                 cell.TerrainBiomeIndex == (int)HexCell.BiomeType.SNOW ||
                 cell.CountNeighborsFeatures >= 2);
        return cell;
    }
}