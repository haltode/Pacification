using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public HexGridChunk chunk;
    public RectTransform uiRect;

    [SerializeField] HexCell[] neighbors;
    [SerializeField] bool[] roads;

    int terrainBiomeIndex;
    int elevation = int.MinValue;
    int featureIndex;

    int distance;

    public Vector3 Position
    {
        get { return transform.localPosition; }
    }

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
        if(!roads[(int) direction] && IsReachable(direction))
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
                if(roads[i] && !IsReachable((HexDirection) i))
                    SetRoad(i, false);

            Refresh();
        }
    }

    public int GetElevationDifference(HexDirection direction)
    {
        int difference = Math.Abs(elevation - GetNeighbor(direction).elevation);
        return difference;
    }

    public bool IsReachable(HexDirection direction)
    {
        return GetElevationDifference(direction) <= HexMetrics.MaxRoadElevation;
    }

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

    public int Distance
    {
        get { return distance; }
        set { distance = value; }
    }

    public HexCell PathFrom { get; set; }

    public int SearchHeuristic { get; set; }

    public int SearchPriority
    {
        get { return distance + SearchHeuristic; }
    }

    public HexCell NextWithSamePriority { get; set; }


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

    public void EnableHighlight(Color color)
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.color = color;
        highlight.enabled = true;
    }

    public void DisableHighlight()
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.enabled = false;        
    }

    public void SetLabel(string text)
    {
        Text label = uiRect.GetComponent<Text>();
        label.text = text;
    }
}