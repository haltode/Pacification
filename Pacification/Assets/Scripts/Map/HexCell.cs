using UnityEngine;

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

    int elevation = int.MinValue;

    public int Elevation
    {
        get { return elevation; }
        set
        {
            if(elevation == value)
                return;

            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.ElevationStep;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.ElevationStep;
            uiRect.localPosition = uiPosition;

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

    Color color;

    public Color Color
    {
        get { return color; }
        set
        {
            if(color == value)
                return;

            color = value;
            Refresh();
        }
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
}