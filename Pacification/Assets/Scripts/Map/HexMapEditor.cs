using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;

    void Awake()
    {
        SelectColor(0);
    }

    void Update()
    {
        if(Input.GetMouseButton(0) && 
            !EventSystem.current.IsPointerOverGameObject())
            HandleInput();
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(inputRay, out hit))
            hexGrid.ColorCell(hit.point, activeColor);
    }

    public void SelectColor(int colorIndex)
    {
        activeColor = colors[colorIndex];
    }
}
