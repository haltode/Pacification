using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour
{
    public HexGrid grid;
    ControlsManager controls;

    HexCell currentCell;
    HexUnit selectedUnit;
    public City selectedCity;

    Client client;

    void Start()
    {
        grid = FindObjectOfType<HexGrid>();
        client = FindObjectOfType<Client>();
        controls = FindObjectOfType<ControlsManager>();
    }

    void Update()
    {
        if(!(GameManager.Instance.editor || client.player.canPlay && !EventSystem.current.IsPointerOverGameObject()))
            return;

        if(Input.GetMouseButtonDown(0))
        {
            DoSelection();
            if(currentCell)
            {
                if(currentCell.FeatureIndex == 1) // City
                    selectedCity = client.player.GetCity(currentCell);
            }
        }
        
        if(selectedUnit)
        {
            if(Input.GetMouseButton(0))
                DoPathfinding();
            else if(Input.GetMouseButtonUp(0))
                DoMove();
            else if(Input.GetKeyDown(controls.unitAction))
            {
                if(selectedUnit.Unit.Type == Unit.UnitType.SETTLER)
                    ((Settler)selectedUnit.Unit).Settle();
                else if(selectedUnit.Unit.Type == Unit.UnitType.WORKER)
                    ((Worker)selectedUnit.Unit).Exploit();
                else
                {
                    Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    HexCell targetCell = grid.GetCell(inputRay);

                    //TODO : differenciate owners. Currently teamkill is allowed for testing purposes.
                    if (selectedUnit.location.coordinates.DistanceTo(targetCell.coordinates) <= ((Attacker)selectedUnit.Unit).Range)
                        ((Attacker)selectedUnit.Unit).Attack(targetCell.Unit.Unit);
                }
            }
        }
    }

    public void SetEditMode(bool toggle)
    {
        grid = FindObjectOfType<HexGrid>();
        enabled = !toggle;
        grid.ShowUI(!toggle);
        grid.ClearPath();
        if(toggle)
            Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
        else
            Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
    }

    bool UpdateCurrentCell()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        HexCell cell = grid.GetCell(inputRay);
        if(cell != currentCell)
        {
            if(currentCell)
                currentCell.DisableHighlight();
            currentCell = cell;
            currentCell.EnableHighlight(Color.blue);
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
        grid = FindObjectOfType<HexGrid>();
        if(UpdateCurrentCell())
        {
            if(currentCell && selectedUnit.IsValidDestination(currentCell))
                grid.FindPath(selectedUnit.Location, currentCell, selectedUnit);
            else
                grid.ClearPath();
        }
    }

    void DoMove()
    {
        if(grid.HasPath)
        {
            if(GameManager.Instance.editor)
                selectedUnit.Travel(grid.GetPath());
            else
            {
                int xStart = selectedUnit.Location.coordinates.X;
                int zStart = selectedUnit.Location.coordinates.Z;

                int xEnd = currentCell.coordinates.X;
                int zEnd = currentCell.coordinates.Z;

                client.Send("CMOV|" + xStart + "#" + zStart + "#" + xEnd + "#" + zEnd);
            }
        }
    }

    public void NetworkDoMove(string data)
    {
        string[] receiverdData = data.Split('#');

        int xStart = int.Parse(receiverdData[0]);
        int zStart = int.Parse(receiverdData[1]);

        int xEnd = int.Parse(receiverdData[2]);
        int zEnd = int.Parse(receiverdData[3]);

        HexCell cellStart = grid.GetCell(new HexCoordinates(xStart, zStart));
        HexCell cellEnd = grid.GetCell(new HexCoordinates(xEnd, zEnd));

        grid.ClearPath();
        grid.FindPath(cellStart, cellEnd, cellStart.Unit);

        cellStart.Unit.Travel(grid.GetPath());
        grid.ClearPath();
    }
}
