using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour
{
    public HexGrid hexGrid;
    ControlsManager controls;

    HexCell currentCell;
    HexCell attackTargetCell;
    public Unit selectedUnit;
    public City selectedCity;
    public Barrack barrack;
    bool didPathfinding;

    Client client;

    void Start()
    {
        // Editor should not have the game UI enabled
        if(GameManager.Instance.gamemode == GameManager.Gamemode.EDITOR)
        {
            gameObject.SetActive(false);
            return;
        }

        hexGrid = FindObjectOfType<HexGrid>();
        client = FindObjectOfType<Client>();
        controls = FindObjectOfType<ControlsManager>();
        barrack = FindObjectOfType<Barrack>();
        barrack.GetBarrackObject.SetActive(false);
    }

    void Update()
    {
        if(!client.player.canPlay || EventSystem.current.IsPointerOverGameObject())
            return;

        if(attackTargetCell)
        {
            attackTargetCell.DisableHighlight();
            attackTargetCell = null;
        }

        if(Input.GetMouseButtonDown(0))
            DoSelection();
        
        if(selectedUnit != null)
        {
            if(Input.GetMouseButton(1))
                DoPathfinding();
            else if(Input.GetMouseButtonUp(1))
                DoMove();
            else
            {
                bool unitAction = Input.GetKeyDown(controls.unitAction);
                bool workerRoad = Input.GetKeyDown(controls.workerAddRoad);
                // Worker has two actions (exploit resources or add road)
                if(unitAction || workerRoad)
                    DoAction(unitAction);
            }
            // After pathfinding clearing
            if(selectedUnit != null && !selectedUnit.HexUnit.location.IsHighlighted())
                selectedUnit.HexUnit.location.EnableHighlight(Color.blue);
        }
    }

    HexCell GetCellUnderCursor()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        return hexGrid.GetCell(inputRay);
    }

    City GetSelectCity(HexCell location)
    {
        City city = client.player.GetCity(location);
        if(city != null)
            barrack.GetBarrackObject.SetActive(true);
        return city;
    }

    bool UpdateCurrentCell()
    {
        HexCell cell = GetCellUnderCursor();
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
        if(selectedUnit != null)
            selectedUnit.HexUnit.location.DisableHighlight();
        selectedUnit = null;
        selectedCity = null;
        barrack.GetBarrackObject.SetActive(false);
        if(currentCell)
        {
            if(currentCell.Unit)
            {
                selectedUnit = client.player.GetUnit(currentCell);
                if(selectedUnit != null)
                    StartCoroutine(HexMapCamera.FocusSmoothTransition(currentCell.Position));
            }
            else if(currentCell.HasCity)
            {
                selectedCity = GetSelectCity(currentCell);
                if(selectedCity != null)
                    StartCoroutine(HexMapCamera.FocusSmoothTransition(currentCell.Position));
            }
        }
    }

    void DoAction(bool unitAction)
    {
        if(selectedUnit.Type == Unit.UnitType.SETTLER && unitAction)
        {
            ((Settler)selectedUnit).Settle();
            selectedUnit = null;
            currentCell = null;
        }
        else if(selectedUnit.Type == Unit.UnitType.WORKER)
        {
            if(unitAction)
                ((Worker)selectedUnit).Exploit();
            else
            {
                HexCell roadCell = GetCellUnderCursor();
                if(!roadCell || roadCell.IsUnderWater || !roadCell.IsExplored || roadCell.Unit)
                    return;
                bool roadOk = ((Worker)selectedUnit).AddRoad(roadCell);
                if(roadOk)
                    currentCell = roadCell;
            }
        }
        else if(unitAction)
        {
            attackTargetCell = GetCellUnderCursor();
            if(attackTargetCell && attackTargetCell.Unit && selectedUnit.Owner != attackTargetCell.Unit.Unit.Owner &&
                selectedUnit.HexUnit.location.coordinates.DistanceTo(attackTargetCell.coordinates) <= ((Attacker)selectedUnit).Range)
            {
                ((Attacker)selectedUnit).Attack(attackTargetCell.Unit.Unit);
                attackTargetCell.EnableHighlight(Color.red);
            }
        }
    }

    void DoPathfinding()
    {
        if(UpdateCurrentCell())
        {
            didPathfinding = true;
            if(currentCell)
                hexGrid.FindPath(selectedUnit.HexUnit.location, currentCell, selectedUnit.HexUnit);
            else
                hexGrid.ClearPath();
        }
    }

    void DoMove()
    {
        if(!didPathfinding)
            return;
        if(didPathfinding && !hexGrid.HasPath)
        {
            currentCell = null;
            didPathfinding = false;
            hexGrid.ClearPath();
            return;
        }

        client.player.MoveUnit(selectedUnit.HexUnit.location, currentCell);
    }

    public void NetworkRoad(string data)
    {
        string[] receivedData = data.Split('#');

        int x = int.Parse(receivedData[0]);
        int z = int.Parse(receivedData[1]);

        HexCell cell = hexGrid.GetCell(new HexCoordinates(x, z));

        if(receivedData[2] == "1")
            cell.SetRoad(int.Parse(receivedData[3]), true);
        else
            cell.NetworkRemoveRoad();
    }
}