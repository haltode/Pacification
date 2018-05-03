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
            else if(Input.GetKeyDown(controls.unitAction))
                DoAction();
            else if(selectedUnit.Type == Unit.UnitType.WORKER && Input.GetKeyDown(controls.workerAddRoad))
                WorkerAddRoad();
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

    void DoAction()
    {
        if(selectedUnit.Type == Unit.UnitType.SETTLER)
        {
            ((Settler)selectedUnit).Settle();
            selectedUnit = null;
            currentCell = null;
        }
        else if(selectedUnit.Type == Unit.UnitType.WORKER)
            ((Worker)selectedUnit).Exploit();
        else
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

        MoveUnit(selectedUnit.HexUnit.location, currentCell);
    }

    void MoveUnit(HexCell start, HexCell end)
    {
        int xStart = start.coordinates.X;
        int zStart = start.coordinates.Z;
        int xEnd = end.coordinates.X;
        int zEnd = end.coordinates.Z;
        client.Send("CMOV|" + xStart + "#" + zStart + "#" + xEnd + "#" + zEnd);
    }

    public void NetworkMoveUnit(string data)
    {
        string[] receiverdData = data.Split('#');

        int xStart = int.Parse(receiverdData[0]);
        int zStart = int.Parse(receiverdData[1]);

        int xEnd = int.Parse(receiverdData[2]);
        int zEnd = int.Parse(receiverdData[3]);

        HexCell cellStart = hexGrid.GetCell(new HexCoordinates(xStart, zStart));
        HexCell cellEnd = hexGrid.GetCell(new HexCoordinates(xEnd, zEnd));

        hexGrid.ClearPath();
        hexGrid.FindPath(cellStart, cellEnd, cellStart.Unit);

        cellStart.Unit.Travel(hexGrid.GetPath());
        hexGrid.ClearPath();
    }

    void WorkerAddRoad()
    {
        HexCell roadCell = GetCellUnderCursor();
        if(!roadCell || roadCell.IsUnderWater || !roadCell.IsExplored || roadCell.Unit)
            return;
        bool isNeighbor = false;
        HexDirection roadDir = HexDirection.NE;
        for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW && !isNeighbor; ++dir)
        {
            HexCell neighbor = currentCell.GetNeighbor(dir);
            if(neighbor == roadCell)
            {
                isNeighbor = true;
                roadDir = dir;
            }
        }
        if(!isNeighbor || !currentCell.IsReachable(roadDir))
            return;

        currentCell.AddRoad(roadDir);
        MoveUnit(currentCell, roadCell);
        currentCell = roadCell;
    }
}
