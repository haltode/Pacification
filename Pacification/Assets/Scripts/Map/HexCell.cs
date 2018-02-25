using UnityEngine;
using System.IO;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public HexGridChunk chunk;
    public RectTransform uiRect;

    public Vector3 Position
    {
        get { return transform.localPosition; }
    }

    [SerializeField]
    HexCell[] neighbors;

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int) direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int) direction] = cell;
        // Relationships between neighbors are bidirectional
        cell.neighbors[(int) direction.Opposite()] = this;
    }

    [SerializeField]
    bool[] roads;

    public bool HasRoadThroughEdge(HexDirection direction)
    {
        return roads[(int) direction];    
    }

    public bool HasRoads
    {
        get
        {
            for(int i = 0; i < roads.Length; ++i)
                if(roads[i])
                    return true;
            return false;
        }
    }

    public void AddRoad(HexDirection direction)
    {
        if(!roads[(int) direction] && GetElevationDifference(direction) <= HexMetrics.MaxRoadElevation)
            SetRoad((int) direction, true);
    }

    public void RemoveRoads()
    {
        for(int i =  0; i < neighbors.Length; ++i)
            if(roads[i])
                SetRoad(i, false);
    }

    void SetRoad(int index, bool state)
    {
        roads[index] = state;
        neighbors[index].roads[(int) ((HexDirection) index).Opposite()] = state;

        neighbors[index].RefreshSelfOnly();
        RefreshSelfOnly();
    }

    int terrainBiomeIndex;

    public int TerrainBiomeIndex
    {
        get { return terrainBiomeIndex; }
        set
        {
            if(terrainBiomeIndex != value)
            {
                terrainBiomeIndex = value;
                Refresh();
            }
        }
    }

    int elevation = int.MinValue;

    public int Elevation
    {
        get { return elevation; }
        set
        {
            if(elevation == value)
                return;

            elevation = value;
            RefreshPosition();

            for(int i = 0; i < roads.Length; ++i)
                if(roads[i] && GetElevationDifference((HexDirection) i) > HexMetrics.MaxRoadElevation)
                    SetRoad(i, false);

            Refresh();
        }
    }

    public int GetElevationDifference(HexDirection direction)
    {
        int difference = elevation - GetNeighbor(direction).elevation;
        if(difference < 0)
            difference = -difference;
        return difference;
    }

    int featureIndex = 0;

    public int FeatureIndex
    {
        get { return featureIndex; }
        set
        {
            if(featureIndex != value)
            {
                featureIndex = value;
                RefreshSelfOnly();
            }
        }
    }

    public bool HasFeature
    {
        get { return featureIndex > 0; }
    }

    void Refresh()
    {
        if(chunk)
        {
            chunk.Refresh();
            for(int i = 0; i < neighbors.Length; ++i)
            {
                HexCell neighbor = neighbors[i];
                if(neighbor != null && neighbor.chunk != chunk)
                    neighbor.chunk.Refresh();
            }
        }
    }

    void RefreshSelfOnly()
    {
        chunk.Refresh();
    }

    void RefreshPosition()
    {
        Vector3 position = transform.localPosition;
        position.y = elevation * HexMetrics.ElevationStep;
        transform.localPosition = position;

        Vector3 uiPosition = uiRect.localPosition;
        uiPosition.z = elevation * -HexMetrics.ElevationStep;
        uiRect.localPosition = uiPosition;
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte) terrainBiomeIndex);
        writer.Write((byte) elevation);
        writer.Write((byte) featureIndex);

        int roadFlags = 0;
        for(int i = 0; i < roads.Length; ++i)
            if(roads[i])
                roadFlags |= (1 << i);
        writer.Write((byte) roadFlags);
    }

    public void Load(BinaryReader reader)
    {
        terrainBiomeIndex = reader.ReadByte();
        elevation = reader.ReadByte();
        RefreshPosition();
        featureIndex = reader.ReadByte();

        int roadFlags = reader.ReadByte();
        for(int i = 0; i < roads.Length; ++i)
            roads[i] = (roadFlags & (1 << i)) != 0;
    }
}