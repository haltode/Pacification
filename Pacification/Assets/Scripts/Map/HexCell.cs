using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public HexGridChunk chunk;
    public RectTransform uiRect;

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

            Refresh();
        }
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
}