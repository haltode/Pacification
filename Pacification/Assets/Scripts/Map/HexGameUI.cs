using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour
{
    public HexGrid grid;

    HexCell currentCell;
    HexUnit selectedUnit;

    void Update()
    {
        if(!EventSystem.current.IsPointerOverGameObject())
        {
            if(Input.GetMouseButtonDown(0))
                DoSelection();
            else if(selectedUnit)
            {
                if(Input.GetMouseButtonDown(1))
                    DoMove();
                else
                    DoPathfinding();
            }
        }
    }

    public void SetEditMode(bool toggle)
    {
        enabled = !toggle;
        grid.ShowUI(!toggle);
        grid.ClearPath();
    }

    bool UpdateCurrentCell()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        HexCell cell = grid.GetCell(inputRay);
        if(cell != currentCell)
        {
            currentCell = cell;
            return true;
        }
        return false;
    }

    void DoSelection()
    {
        UpdateCurrentCell();
        if(currentCell)
            selectedUnit = currentCell.Unit;
    }

    void DoPathfinding()
    {
        if(UpdateCurrentCell())
        {
            if(currentCell && selectedUnit.IsValidDestination(currentCell))
                grid.FindPath(selectedUnit.Location, currentCell, 24);
            else
                grid.ClearPath();
        }
    }

    void DoMove()
    {
        if(grid.HasPath)
        {
            selectedUnit.Location = currentCell;
            grid.ClearPath();
        }
    }
}
