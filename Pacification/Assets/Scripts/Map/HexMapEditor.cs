using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public class HexMapEditor : MonoBehaviour
{
    enum OptionalToggle {
        Ignore, No, Yes
    }

    public HexGrid hexGrid;

    private int activeTerrainBiomeIndex;
    private int activeElevation;
    private int activeFeature;
    private bool applyElevation;
    private bool applyFeature;
    private int brushSize;

    bool isDrag;
    HexDirection dragDirection;
    HexCell previousCell;

    OptionalToggle roadMode;

    void Update()
    {
        if(Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            HandleInput();
        else
            previousCell = null;
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(inputRay, out hit))
        {
            HexCell currentCell = hexGrid.GetCell(hit.point);
            if(previousCell && previousCell != currentCell)
                ValidateDrag(currentCell);
            else
                isDrag = false;

            EditCells(currentCell);
            previousCell = currentCell;
            isDrag = true;
        }
        else
            previousCell = null;
    }

    void ValidateDrag(HexCell currentCell)
    {
        for(dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; ++dragDirection)
        {
            if(previousCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
    }

    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for(int r = 0, z = centerZ - brushSize; z <= centerZ; ++z, ++r)
            for(int x = centerX - r; x <= centerX + brushSize; ++x)
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));

        for(int r = 0, z = centerZ + brushSize; z > centerZ; --z, ++r)
            for(int x = centerX - brushSize; x <= centerX + r; ++x)
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
    }

    void EditCell(HexCell cell)
    {
        if(cell)
        {
            if(activeTerrainBiomeIndex >= 0)
                cell.TerrainBiomeIndex = activeTerrainBiomeIndex;
            if(applyElevation)
                cell.Elevation = activeElevation;
            if(applyFeature)
                cell.FeatureIndex = activeFeature;
            if(roadMode == OptionalToggle.No)
                cell.RemoveRoads();
            if(isDrag)
            {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if(otherCell && roadMode == OptionalToggle.Yes)
                    otherCell.AddRoad(dragDirection);
            }
        }
    }

    public void SetTerrainBiomeIndex(int index)
    {
        activeTerrainBiomeIndex = index;
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int) elevation;
    }

    public void SetFeatureIndex(int featureIndex)
    {
        activeFeature = featureIndex;
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    public void SetApplyFeature(bool toggle)
    {
        applyFeature = toggle;
    }

    public void SetBrushSize(float size)
    {
        brushSize = (int) size;
    }

    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }

    public void SetRoadMode(int mode)
    {
        roadMode = (OptionalToggle) mode;
    }
}
