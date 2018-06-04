using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    enum OptionalToggle { Ignore, No, Yes }

    public HexGrid hexGrid;

    int activeTerrainBiomeIndex;
    int activeElevation;
    int activeFeature;
    bool applyElevation;

    OptionalToggle underWaterMode;

    void Start()
    {
        hexGrid = FindObjectOfType<HexGrid>();
        if(GameManager.Instance.gamemode == GameManager.Gamemode.EDITOR)
            FindObjectOfType<DisplayInformationManager>().KillLoading();
    }

    void Update()
    {
        if(GameManager.Instance.gamemode != GameManager.Gamemode.EDITOR)
            return;

        if(!EventSystem.current.IsPointerOverGameObject())
            if(Input.GetMouseButton(0))
                HandleInput();
    }

    HexCell GetCellUnderCursor()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        return hexGrid.GetCell(inputRay);
    }

    void HandleInput()
    {
        HexCell currentCell = GetCellUnderCursor();
        if(currentCell)
            EditCell(currentCell);
    }

    public void EditCell(HexCell cell)
    {
        if (activeTerrainBiomeIndex >= 0)
            cell.TerrainBiomeIndex = activeTerrainBiomeIndex;
        cell.FeatureIndex = activeFeature;
        if(applyElevation)
            cell.Elevation = activeElevation;
        if(underWaterMode == OptionalToggle.No && cell.IsUnderWater)
            cell.IsUnderWater = false;
        else if(underWaterMode == OptionalToggle.Yes && !cell.IsUnderWater)
            cell.IsUnderWater = true;
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

    public void SetUnderWaterMode(int mode)
    {
        underWaterMode = (OptionalToggle) mode;
    }
}
