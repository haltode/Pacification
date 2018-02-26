using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class HexGrid : MonoBehaviour
{
    public int cellCountX = 20;
    public int cellCountZ = 15;

    private int chunkCountX;
    private int chunkCountZ;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;
    public HexGridChunk chunkPrefab;

    HexGridChunk[] chunks;
    HexCell[] cells;

    void Awake()
    {
        CreateMap(cellCountX, cellCountZ);
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
        CreateChunks();
        CreateCells();
        ClearPath();

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

    bool CompareCells(HexCell a, HexCell b)
    {
        if(a == null || b == null)
            return true;
        else
            return a.SearchPriority <= b.SearchPriority;
    }

    public void FindPath(HexCell start, HexCell end, int speed)
    {
        ClearPath();
        bool found = SearchPath(start, end, speed);
        ShowPath(start, end, speed, found);
    }

    bool SearchPath(HexCell start, HexCell end, int speed)
    {
        PriorityQueue<HexCell> searchQueue = new PriorityQueue<HexCell>(CompareCells);
        start.Distance = 0;
        searchQueue.Enqueue(start);
        while(!searchQueue.IsEmpty())
        {
            HexCell current = searchQueue.Dequeue();
            if(current == end)
                return true;

            int currentTurn = current.Distance / speed;
            for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
            {
                HexCell neighbor = current.GetNeighbor(dir);
                if(neighbor == null || !current.IsReachable(dir) || neighbor.Distance != int.MaxValue)
                    continue;

                // Road and flat terrains are faster than cliffs
                int moveCost;
                if(current.HasRoadThroughEdge(dir))
                    moveCost = 1;
                else if(current.GetElevationDifference(dir) == 0)
                    moveCost = 5;
                else
                    moveCost = 10;

                int newDist = current.Distance + moveCost;
                int turn = newDist / speed;
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

    void ShowPath(HexCell start, HexCell end, int speed, bool found)
    {
        start.EnableHighlight(Color.blue);
        if(found)
        {
            HexCell current = end;
            while(current != start)
            {
                int turn = current.Distance / speed;
                current.SetLabel(turn.ToString());
                current.EnableHighlight(Color.white);
                current = current.PathFrom;
            }
            end.EnableHighlight(Color.green);
        }
        else
            end.EnableHighlight(Color.red);
    }

    void ClearPath()
    {
        for(int i = 0; i < cells.Length; ++i)
        {
            cells[i].Distance = int.MaxValue;
            cells[i].SetLabel(null);
            cells[i].DisableHighlight();
        }
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
        for(int i = 0; i < chunks.Length; ++i)
            chunks[i].Refresh();
        ClearPath();
    }
}