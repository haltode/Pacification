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
    bool isUnderWater;
    int featureIndex;

    int visibility;
    bool explored;

    int distance;

    public HexUnit Unit { get; set; }

    public int Index { get; set; }

    public Vector3 Position
    {
        get { return transform.localPosition; }
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int) direction];
    }

    public HexDirection GetNeighborDir(HexCell neighbor)
    {
        for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; ++dir)
            if(GetNeighbor(dir) == neighbor)
                return dir;
        return default(HexDirection);
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
        if(!roads[(int)direction] && IsReachable(direction))
            FindObjectOfType<Client>().Send("CUNI|ROD|" + coordinates.X + "#" + coordinates.Z + "#" + 1 + "#" + (int)direction);
    }

    public void RemoveRoads()
    {
        FindObjectOfType<Client>().Send("CUNI|ROD|" + coordinates.X + "#" + coordinates.Z + "#" + 0);
    }

    public void NetworkRemoveRoad()
    {
        for(int i = 0; i < neighbors.Length; ++i)
            if(roads[i])
                SetRoad(i, false);
    }

    public void SetRoad(int index, bool state)
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
                ShaderData.RefreshTerrain(this);
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
            ShaderData.ViewElevationChanged();
            RefreshPosition();
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
        return GetElevationDifference(direction) <= HexMetrics.MaxElevationReach;
    }

    public bool IsUnderWater
    {
        get { return isUnderWater; }
        set
        {
            if(isUnderWater == value)
                return;

            isUnderWater = value;
            Refresh();
        }
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

    public bool HasCity
    {
        get { return featureIndex == 1 || featureIndex == 2 || featureIndex == 3; }
    }

    public bool Explorable { get; set; }

    public bool IsVisible
    {
        get { return visibility > 0 && Explorable; }
    }

    public bool IsExplored
    {
        get { return explored && Explorable; }
        private set { explored = value; }
    }

    public int ViewElevation
    {
        get { return elevation; }
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

    public static bool CompareCells(HexCell a, HexCell b)
    {
        if(a == null || b == null)
            return true;
        else
            return a.SearchPriority <= b.SearchPriority;
    }

    public HexCellShaderData ShaderData { get; set; }

    public void Save(BinaryWriter writer)
    {
        // Use byte to save space since we stay inside range [0; 255]
        writer.Write((byte) terrainBiomeIndex);
        writer.Write((byte) elevation);
        writer.Write(isUnderWater);
        writer.Write((byte) featureIndex);

        // Again we save space by encoding the boolean array inside a single int
        int roadFlags = 0;
        for(int i = 0; i < roads.Length; ++i)
            if(roads[i])
                roadFlags |= (1 << i);
        writer.Write((byte) roadFlags);

        writer.Write(IsExplored);
    }

    public void Load(BinaryReader reader)
    {
        terrainBiomeIndex = reader.ReadByte();
        ShaderData.RefreshTerrain(this);
        elevation = reader.ReadByte();
        isUnderWater = reader.ReadBoolean();
        RefreshPosition();
        featureIndex = reader.ReadByte();

        int roadFlags = reader.ReadByte();
        for(int i = 0; i < roads.Length; ++i)
            roads[i] = (roadFlags & (1 << i)) != 0;
    
        IsExplored = reader.ReadBoolean();
        ShaderData.RefreshVisibility(this);
    }

    void Refresh()
    {
        if(!chunk)
            return;

        if(Unit)
            Unit.ValidateLocation();

        chunk.Refresh();
        for(int i = 0; i < neighbors.Length; ++i)
        {
            HexCell neighbor = neighbors[i];
            if(neighbor != null && neighbor.chunk != chunk)
                neighbor.chunk.Refresh();
        }
    }

    void RefreshSelfOnly()
    {
        chunk.Refresh();
        if(Unit)
            Unit.ValidateLocation();
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

    public bool IsHighlighted()
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        return highlight.isActiveAndEnabled;
    }

    public void SetLabel(string text)
    {
        Text label = uiRect.GetComponent<Text>();
        label.text = text;
    }

    public void IncreaseVisibility()
    {
        ++visibility;
        if(visibility == 1)
        {
            IsExplored = true;
            ShaderData.RefreshVisibility(this);
        }
    }

    public void DecreaseVisibility()
    {
        --visibility;
        if(visibility == 0)
            ShaderData.RefreshVisibility(this);
    }

    public void ResetVisibility()
    {
        if(visibility > 0)
        {
            visibility = 0;
            ShaderData.RefreshVisibility(this);
        }
    }
}