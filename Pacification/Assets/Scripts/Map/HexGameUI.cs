using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour
{
    public HexGrid grid;
    ControlsManager controls;

    HexCell currentCell;
    HexCell targetCell;
    HexUnit selectedUnit;
    public City selectedCity;
    public Barrack barrack;

    Client client;

    void Start()
    {
        grid = FindObjectOfType<HexGrid>();
        client = FindObjectOfType<Client>();
        controls = FindObjectOfType<ControlsManager>();
        barrack = FindObjectOfType<Barrack>();
        barrack.GetBarrackObject.SetActive(false);
    }

    void Update()
    {
        if(GameManager.Instance.gamemode == GameManager.Gamemode.EDITOR)
            return;
        if(!(client.player.canPlay && !EventSystem.current.IsPointerOverGameObject()))
            return;

        if (targetCell)
        {
            targetCell.DisableHighlight();
            targetCell = null;
        }

        if(Input.GetMouseButtonDown(0))
        {
            DoSelection();
            if(currentCell)
            {
                if (currentCell.FeatureIndex == 1) // City
                {
                    selectedCity = client.player.GetCity(currentCell);
                    barrack.GetBarrackObject.SetActive(true);
                }
                else
                {
                    barrack.GetBarrackObject.SetActive(false);
                }
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
                    targetCell = grid.GetCell(inputRay);

                    if (selectedUnit.location.coordinates.DistanceTo(targetCell.coordinates) <= ((Attacker)selectedUnit.Unit).Range)
                    {
                        if (selectedUnit.Unit.Owner != targetCell.Unit.Unit.Owner)
                        {
                            ((Attacker)selectedUnit.Unit).Attack(targetCell.Unit.Unit);
                            targetCell.EnableHighlight(Color.red);
                        }
                    }
                }
            }
        }
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
        if (currentCell)
        {
            if (currentCell.Unit &&
                currentCell.Unit.Unit.Owner == client.player)
                selectedUnit = currentCell.Unit;
            else
                selectedUnit = null;
        }
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
            if(GameManager.Instance.gamemode == GameManager.Gamemode.EDITOR)
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
