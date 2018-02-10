using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    
    public Color color;

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

    int elevation;
    public int Elevation
    {
        get { return elevation; }
        set
        {
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.ElevationStep;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.ElevationStep;
            uiRect.localPosition = uiPosition;
        }
    }
}