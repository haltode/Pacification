using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;
    private int activeElevation;

    void Awake()
    {
        SetColor(0);
    }

    void Update()
    {
        if(Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            HandleInput();
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(inputRay, out hit))
            EditCell(hexGrid.GetCell(hit.point));
    }

    void EditCell(HexCell cell)
    {
        cell.Color = activeColor;
        cell.Elevation = activeElevation;
    }

    public void SetColor(int colorIndex)
    {
        activeColor = colors[colorIndex];
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int) elevation;
    }
}
