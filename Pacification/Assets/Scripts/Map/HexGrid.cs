using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;

public class HexGrid : MonoBehaviour
{
    public int seed;

    const int MaxIterationGen = 1000;
    const int MaxIterationNearCell = 100;
    const int MaxUnitInitSpawnRadius = 10;

    public int cellCountX = 20;
    public int cellCountZ = 15;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;
    public HexGridChunk chunkPrefab;

    public GameObject mainUnitPrefab;
    public GameObject[] unitPrefab;

    public GameObject[] cityPrefab;

    public System.Random rnd;

    HexGridChunk[] chunks;
    int chunkCountX, chunkCountZ;
    public HexCell[] cells;
    List<HexUnit> units = new List<HexUnit>();

    HexCellShaderData cellShaderData;

    public HexCell currentPathStart, currentPathEnd;
    public bool currentPathExists;

    public bool HasPath
    {
        get { return currentPathExists; }
    }

    void Awake()
    {
        HexMetrics.InitializeHashGrid(seed);
        rnd = new System.Random();
        cellShaderData = gameObject.AddComponent<HexCellShaderData>();
        cellShaderData.Grid = this;
        CreateMap(cellCountX, cellCountZ);
    }

    void OnEnable()
    {
        rnd = new System.Random();
        HexMetrics.InitializeHashGrid(seed);
        ResetVisibility();
    }

    public HexCell GetCell(Ray ray)
    {
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
            return GetCell(hit.point);
        return null;
    }

    public HexCell GetCell(int xOffset, int zOffset)
    {
        return cells[xOffset + zOffset * cellCountX];
    }

    public HexCell GetCell(int cellIndex)
    {
        return cells[cellIndex];
    }

    public HexCell GetNearFreeCell(HexCell start)
    {
        List<HexCell> possibleLocation = new List<HexCell>();
        ResetDistances();

        PriorityQueue<HexCell> searchQueue = new PriorityQueue<HexCell>(HexCell.CompareCells);
        start.Distance = 0;
        searchQueue.Enqueue(start);
        int guard = 0;
        int lastDist = 0;
        while(!searchQueue.IsEmpty())
        {
            guard++;
            if(guard > MaxIterationNearCell)
                break;
            HexCell current = searchQueue.Dequeue();
            if(current.Distance != lastDist)
            {
                if(possibleLocation.Count >= 1)
                    break;
                lastDist = current.Distance;
            }

            for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
            {
                HexCell neighbor = current.GetNeighbor(dir);
                if(neighbor == null || neighbor.Distance != int.MaxValue)
                    continue;
                if(neighbor.IsUnderWater || neighbor.HasUnit || IsBorder(neighbor) || neighbor.Elevation >= 4)
                    continue;
                neighbor.Distance = current.Distance + 1;
                neighbor.SearchHeuristic = 0;
                searchQueue.Enqueue(neighbor);
                possibleLocation.Add(neighbor);
            }
        }

        int randomCell = rnd.Next(possibleLocation.Count);
        return possibleLocation[randomCell];
    }

    public bool IsBorder(HexCell location)
    {
        for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
        {
            HexCell neighbor = location.GetNeighbor(dir);
            if(!neighbor)
                return true;
        }
        return false;
    }

    public bool CreateMap(int sizeX, int sizeZ)
    {
        if( sizeX <= 0 || sizeX % HexMetrics.ChunkSizeX != 0 ||
            sizeZ <= 0 || sizeZ % HexMetrics.ChunkSizeZ != 0)
        {
            Debug.LogError("Unsupported map size.");
            return false;
        }
        if(chunks != null)
            for(int i = 0; i < chunks.Length; ++i)
                Destroy(chunks[i].gameObject);

        cellCountX = sizeX;
        cellCountZ = sizeZ;
        chunkCountX = cellCountX / HexMetrics.ChunkSizeX;
        chunkCountZ = cellCountZ / HexMetrics.ChunkSizeZ;
        cellShaderData.Initialize(cellCountX, cellCountZ);
        CreateChunks();
        CreateCells();
        ClearPath();
        ClearUnits();

        return true;
    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];
        int chunkIndex = 0;
        for(int z = 0; z < chunkCountZ; ++z)
        {
            for(int x = 0; x < chunkCountX; ++x)
            {
                HexGridChunk chunk = chunks[chunkIndex] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
                ++chunkIndex;
            }
        }
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountX * cellCountZ];
        int cellIndex = 0;
        for(int z = 0; z < cellCountZ; ++z)
            for(int x = 0; x < cellCountX; ++x)
                CreateCell(x, z, cellIndex++);
    }

    void CreateCell(int x, int z, int cellIndex)
    {
        Vector3 position;
        // Hexagonal spacing
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.InnerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.OuterRadius * 1.5f);

        HexCell cell = cells[cellIndex] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Index = cellIndex;
        cell.ShaderData = cellShaderData;
        cell.Explorable = x > 0 && z > 0 && x < (cellCountX - 1) && z < (cellCountZ - 1);

        // E-W neighbor connection
        if(x > 0)
            cell.SetNeighbor(HexDirection.W, cells[cellIndex - 1]);
        if(z > 0)
        {
            // NW-SE and NE-SW connections (dealing with even/odd rows)
            if(z % 2 == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[cellIndex - cellCountX]);
                if(x > 0)
                    cell.SetNeighbor(HexDirection.SW, cells[cellIndex - cellCountX - 1]);
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[cellIndex - cellCountX]);
                if(x < cellCountX - 1)
                    cell.SetNeighbor(HexDirection.SE, cells[cellIndex - cellCountX + 1]);
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        cell.uiRect = label.rectTransform;

        cell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.ChunkSizeX;
        int chunkZ = z / HexMetrics.ChunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.ChunkSizeX;
        int localZ = z - chunkZ * HexMetrics.ChunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.ChunkSizeX, cell);
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        if(z < 0 || z >= cellCountZ)
            return null;
        int x = coordinates.X + z / 2;
        if(x < 0 || x >= cellCountX)
            return null;

        int index = x + z * cellCountX;
        return cells[index];
    }

    public void ResetDistances()
    {
        for(int i = 0; i < cells.Length; ++i)
            cells[i].Distance = int.MaxValue;    
    }

    public void FindPath(HexCell start, HexCell end, HexUnit unit, bool isAI=false)
    {
        ClearPath();
        currentPathStart = start;
        currentPathEnd = end;
        currentPathExists = SearchPath(start, end, unit, isAI);
        if(!isAI)
            ShowPath(unit.Speed);
    }

    public bool SearchPath(HexCell start, HexCell end, HexUnit unit, bool isAI=false)
    {
        PriorityQueue<HexCell> searchQueue = new PriorityQueue<HexCell>(HexCell.CompareCells);
        start.Distance = 0;
        searchQueue.Enqueue(start);
        while(!searchQueue.IsEmpty())
        {
            HexCell current = searchQueue.Dequeue();
            if(current == end)
                return true;

            for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
            {
                HexCell neighbor = current.GetNeighbor(dir);
                if(neighbor == null || neighbor.Distance != int.MaxValue)
                    continue;
                if(neighbor.IsUnderWater && !unit.Unit.CanEmbark || neighbor.Unit)
                    continue;
                // AI ignores fog
                if(!isAI && !neighbor.IsExplored)
                    continue;

                int moveCost = unit.GetMoveCost(current, neighbor);
                if(moveCost == -1)
                    continue;
                if(moveCost > unit.Unit.MvtSPD)
                    continue;
                int newDist = current.Distance + moveCost;
                if(!isAI && (newDist > unit.Unit.MvtSPD - unit.Unit.currMVT))
                    continue;

                neighbor.Distance = newDist;
                neighbor.PathFrom = current;
                neighbor.SearchHeuristic = neighbor.coordinates.DistanceTo(end.coordinates);
                searchQueue.Enqueue(neighbor);
            }
        }
        return false;
    }

    void ShowPath(int speed)
    {
        currentPathStart.EnableHighlight(Color.blue);
        if(currentPathExists)
        {
            HexCell current = currentPathEnd;
            while(current != currentPathStart)
            {
                current.SetLabel(current.Distance.ToString());
                current.EnableHighlight(Color.white);
                current = current.PathFrom;
            }
            currentPathEnd.EnableHighlight(Color.green);
        }
        else
            currentPathEnd.EnableHighlight(Color.red);
    }

    public List<HexCell> GetPath()
    {
        if(!currentPathExists)
            return null;
        List<HexCell> path = ListPool<HexCell>.Get();
        for(HexCell c = currentPathEnd; c != currentPathStart; c = c.PathFrom)
            path.Add(c);
        path.Add(currentPathStart);
        path.Reverse();
        return path;
    }

    public void ClearPath()
    {
        ResetDistances();
        for(int i = 0; i < cells.Length; ++i)
        {
            cells[i].SetLabel(null);
            cells[i].DisableHighlight();
        }
    }

    List<HexCell> GetVisibleCells(HexCell start, int range)
    {
        List<HexCell> visibleCells = ListPool<HexCell>.Get();
        ResetDistances();
        range += start.ViewElevation;
        HexCoordinates startCoordinates = start.coordinates;

        PriorityQueue<HexCell> searchQueue = new PriorityQueue<HexCell>(HexCell.CompareCells);
        start.Distance = 0;
        searchQueue.Enqueue(start);
        while(!searchQueue.IsEmpty())
        {
            HexCell current = searchQueue.Dequeue();
            visibleCells.Add(current);

            for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
            {
                HexCell neighbor = current.GetNeighbor(dir);
                if(neighbor == null || neighbor.Distance != int.MaxValue || !neighbor.Explorable)
                    continue;
                int dist = current.Distance + 1;
                if(dist + neighbor.ViewElevation > range ||
                    dist > startCoordinates.DistanceTo(neighbor.coordinates))
                    continue;
                neighbor.Distance = dist;
                neighbor.SearchHeuristic = 0;
                searchQueue.Enqueue(neighbor);
            }
        }
        return visibleCells;
    }

    public void IncreaseVisibility(HexCell start, int range)
    {
        List<HexCell> cells = GetVisibleCells(start, range);
        for(int i = 0; i < cells.Count; ++i)
            cells[i].IncreaseVisibility();
        ListPool<HexCell>.Add(cells);
    }

    public void DecreaseVisibility(HexCell start, int range)
    {
        List<HexCell> cells = GetVisibleCells(start, range);
        for(int i = 0; i < cells.Count; ++i)
            cells[i].DecreaseVisibility();
        ListPool<HexCell>.Add(cells);
    }

    public void ResetVisibility()
    {
        for(int i = 0; i < cells.Length; ++i)
            cells[i].ResetVisibility();
        for(int i = 0; i < units.Count; ++i)
        {
            HexUnit unit = units[i];
            IncreaseVisibility(unit.Location, unit.VisionRange);
        }
    }

    public void InitialSpawnUnit()
    {
        List<HexCell> possibleLocation = new List<HexCell>();
        for(int i = 0; i < cells.Length; ++i)
        {
            HexCell cell = cells[i];
            if(!cell.IsUnderWater && !cell.Unit && !IsBorder(cell) && cell.Elevation <= 3)
                possibleLocation.Add(cell);
        }

        Client client = FindObjectOfType<Client>();
        string str = "CUNI|INI|";
        for(int i = 0; i < client.players.Count; ++i)
        {
            if(client.players[i].name == "Google")
                continue;

            HexCell randomCell = null;
            int guard = 0;
            do
            {
                int idx = rnd.Next(possibleLocation.Count);
                randomCell = possibleLocation[idx];
                possibleLocation.RemoveAt(idx);
                guard++;
            } while(possibleLocation.Count > 0 && guard < MaxIterationGen &&
                    OtherUnitInRadius(randomCell, MaxUnitInitSpawnRadius));
            if(randomCell == null || possibleLocation.Count == 0 || guard == MaxIterationGen ||
                OtherUnitInRadius(randomCell, MaxUnitInitSpawnRadius))
                Debug.LogError("The current map is too small for this many players");
            
            HexCell spawnSettler = randomCell;
            HexCell spawnAttacker = GetNearFreeCell(randomCell);

            str += client.players[i].name + "|";

            string settler = (int)Unit.UnitType.SETTLER + "#" + spawnSettler.coordinates.X + "#" + spawnSettler.coordinates.Z + "#" + "1";
            client.players[i].NetworkAddUnit(settler);

            str += settler + "|";

            string attacker = (int)Unit.UnitType.REGULAR + "#" + spawnAttacker.coordinates.X + "#" + spawnAttacker.coordinates.Z + "#" + "1";
            client.players[i].NetworkAddUnit(attacker);

            str += attacker;
            if(i < client.players.Count -1)
                str += "|";
        }

        client.Send(str);
    }

    public void AddUnit(HexUnit unit, HexCell location, float orientation)
    {
        units.Add(unit);
        unit.Grid = this;
        unit.transform.SetParent(transform, false);
        unit.Location = location;
        unit.Orientation = orientation;
    }

    public void RemoveUnit(HexUnit unit)
    {
        units.Remove(unit);
        unit.Die();
    }

    public bool OtherUnitInRadius(HexCell start, int radius)
    {
        ResetDistances();

        PriorityQueue<HexCell> searchQueue = new PriorityQueue<HexCell>(HexCell.CompareCells);
        start.Distance = 0;
        searchQueue.Enqueue(start);
        while(!searchQueue.IsEmpty())
        {
            HexCell current = searchQueue.Dequeue();
            if(current.Distance > radius)
                continue;
            if(current.HasUnit)
                return true;

            for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
            {
                HexCell neighbor = current.GetNeighbor(dir);
                if(neighbor == null || neighbor.Distance != int.MaxValue)
                    continue;
                neighbor.Distance = current.Distance + 1;
                neighbor.SearchHeuristic = 0;
                searchQueue.Enqueue(neighbor);
            }
        }

        return false;
    }

    public void ClearUnits()
    {
        for(int i = 0; i < units.Count; i++)
            units[i].Die();
        units.Clear();
    }

    public void ShowUI(bool visible)
    {
        for(int i = 0; i<chunks.Length; ++i)
            chunks[i].ShowUI(visible);
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(cellCountX);
        writer.Write(cellCountZ);
        for(int i = 0; i < cells.Length; ++i)
            cells[i].Save(writer);
        writer.Write(units.Count);
        for(int i = 0; i < units.Count; ++i)
            units[i].Save(writer);
    }

    public void Load(BinaryReader reader)
    {
        int sizeX = reader.ReadInt32();
        int sizeZ = reader.ReadInt32();
        if(sizeX != cellCountX || sizeZ != cellCountZ)
            if(!CreateMap(sizeX, sizeZ))
                return;
        for(int i = 0; i < cells.Length; ++i)
            cells[i].Load(reader);
        int unitCount = reader.ReadInt32();
        for(int i = 0; i < unitCount; ++i)
            HexUnit.Load(reader, this);

        for(int i = 0; i < chunks.Length; ++i)
            chunks[i].Refresh();
        ClearPath();
        ClearUnits();
    }
}
