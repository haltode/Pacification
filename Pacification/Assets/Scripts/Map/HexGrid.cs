using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;

public class HexGrid : MonoBehaviour
{
    public int seed;

    public int cellCountX = 20;
    public int cellCountZ = 15;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;
    public HexGridChunk chunkPrefab;

    public GameObject mainUnitPrefab;
    public GameObject[] unitPrefab;

    public GameObject[] cityPrefab;

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
        cellShaderData = gameObject.AddComponent<HexCellShaderData>();
        cellShaderData.Grid = this;
        CreateMap(cellCountX, cellCountZ);
    }

    void OnEnable()
    {
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

    public HexCell GetNearFreeCell(HexCell location)
    {
        List<HexCell> possibleLocation = new List<HexCell>();   
        for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
        {
            HexCell neighbor = location.GetNeighbor(dir);
            if(neighbor && !neighbor.IsUnderWater && !neighbor.Unit)
                possibleLocation.Add(neighbor);
        }

        System.Random rnd = new System.Random();
        int randomCell = rnd.Next(possibleLocation.Count);
        return possibleLocation[randomCell];
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

    public void FindPath(HexCell start, HexCell end, HexUnit unit)
    {
        ClearPath();
        currentPathStart = start;
        currentPathEnd = end;
        currentPathExists = SearchPath(start, end, unit);
        ShowPath(unit.Speed);
    }

    public bool SearchPath(HexCell start, HexCell end, HexUnit unit)
    {
        PriorityQueue<HexCell> searchQueue = new PriorityQueue<HexCell>(HexCell.CompareCells);
        start.Distance = 0;
        searchQueue.Enqueue(start);
        int speed = unit.Speed;
        while(!searchQueue.IsEmpty())
        {
            HexCell current = searchQueue.Dequeue();
            if(current == end)
                return true;

            int currentTurn = (current.Distance - 1) / speed;
            for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
            {
                HexCell neighbor = current.GetNeighbor(dir);
                if(neighbor == null || neighbor.Distance != int.MaxValue)
                    continue;
                if(!unit.IsValidDestination(neighbor))
                    continue;

                int moveCost = unit.GetMoveCost(current, neighbor, dir);
                if(moveCost == -1)
                    continue;
                int newDist = current.Distance + moveCost;
                int turn = (newDist - 1) / speed;
                if(turn > currentTurn)
                    newDist = turn * speed + moveCost;

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
                int turn = (current.Distance - 1) / speed;
                current.SetLabel(turn.ToString());
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
        currentPathExists = false;
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

    public void ClearUnits()
    {
        for(int i = 0; i < units.Count; i++)
            units[i].Die();
        units.Clear();
    }

    public void ShowUI(bool visible)
    {
        for(int i = 0; i < chunks.Length; ++i)
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
