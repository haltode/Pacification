using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexGameUI : MonoBehaviour
{
    public HexGrid hexGrid;
    HexMapCamera mapCamera;
    ControlsManager controls;

    HexCell currentCell;
    HexCell attackTargetCell;
    public Unit selectedUnit;
    public City selectedCity;

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
        mapCamera = FindObjectOfType<HexMapCamera>();
        controls = FindObjectOfType<ControlsManager>();
    }

    void Update()
    {
        if(EventSystem.current.IsPointerOverGameObject())
            return;

        if(Input.GetMouseButtonDown(0))
            DoSelection();

        if(!client.player.canPlay)
            return;

        if(attackTargetCell)
        {
            attackTargetCell.DisableHighlight();
            attackTargetCell = null;
        }

        if(client.chat == null || client.chat.input.isFocused)
            return;

        if(Input.GetKeyDown(controls.cycleCity))
        {
            int cityIdx = mapCamera.CycleBetweenCities();
            if(cityIdx != -1)
            {
                if(currentCell)
                    currentCell.DisableHighlight();
                currentCell = client.player.playerCities[cityIdx].Location;
                currentCell.EnableHighlight(Color.blue);
                DoSelection(updateCell: false);
            }
        }
        else if(Input.GetKeyDown(controls.cycleUnit))
        {
            int unitIdx = mapCamera.CycleBetweenUnits();
            if(unitIdx != -1)
            {
                if(currentCell)
                    currentCell.DisableHighlight();
                currentCell = client.player.playerUnits[unitIdx].HexUnit.Location;
                currentCell.EnableHighlight(Color.blue);
                DoSelection(updateCell: false);
            }
        }
        
        if(selectedUnit != null)
        {
            if(Input.GetMouseButton(1))
                DoPathfinding();
            else if(Input.GetMouseButtonUp(1))
                DoMove();
            else
                DoAction();
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

    bool UpdateCurrentCell()
    {
        HexCell cell = GetCellUnderCursor();
        if(cell != currentCell)
        {
            if(currentCell)
                currentCell.DisableHighlight();
            currentCell = cell;
            if(currentCell && currentCell.IsExplored)
                currentCell.EnableHighlight(Color.blue);
            return true;
        }
        return false;
    }

    void DoSelection(bool updateCell = true)
    {
        if(updateCell)
            UpdateCurrentCell();

        if(selectedUnit != null)
            selectedUnit.HexUnit.location.DisableHighlight();
        selectedUnit = null;
        selectedCity = null;
        
        if(currentCell)
        {
            client.player.displayer.UpdateInformationPannels(currentCell);

            if(currentCell.Unit)
            {
                selectedUnit = client.player.GetUnit(currentCell);
                if(selectedUnit != null)
                    StartCoroutine(mapCamera.FocusSmoothTransition(currentCell.Position));
            }

            if(currentCell.HasCity)
            {
                selectedCity = client.player.GetCity(currentCell);
                if(selectedCity != null)
                    StartCoroutine(mapCamera.FocusSmoothTransition(currentCell.Position));
            }
        }
    }

    void DoAction()
    {
        if(client.chat.input.isFocused)
            return;

        if(Input.GetKeyDown(controls.unitPrimaryAction))
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

                Attacker attacker = (Attacker)selectedUnit;
                if(!attackTargetCell || !(attacker.IsInRangeToAttack(attackTargetCell)))
                    return;

                if(attackTargetCell.Unit && selectedUnit.Owner != attackTargetCell.Unit.Unit.Owner)
                {
                    if (attacker.Attack(attackTargetCell.Unit.Unit))
                        attackTargetCell.EnableHighlight(Color.red);
                }
                else if(attackTargetCell.HasCity && selectedUnit.Owner != attackTargetCell.Feature.Owner)
                {
                    if (attacker.Attack((City)attackTargetCell.Feature))
                        attackTargetCell.EnableHighlight(Color.red);
                }
                else if(attackTargetCell.HasResource && attackTargetCell.FeatureIndex > 9 && selectedUnit.Owner != attackTargetCell.Feature.Owner)
                {
                    if (attacker.Attack((Resource)attackTargetCell.Feature))
                        attackTargetCell.EnableHighlight(Color.red);
                }
            }
        }
        else if(Input.GetKeyDown(controls.unitSecondaryAction))
        {
            if(selectedUnit.Type == Unit.UnitType.WORKER && selectedUnit.currMVT < selectedUnit.MvtSPD)
            {
                HexCell roadCell = GetCellUnderCursor();
                if(!roadCell || roadCell.IsUnderWater || !roadCell.IsExplored || roadCell.Unit)
                    return;
                bool roadOk = ((Worker)selectedUnit).AddRoad(roadCell);
                if(roadOk)
                    currentCell = roadCell;
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

        client.player.MoveUnit(selectedUnit, currentCell);
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