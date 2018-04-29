using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour
{
    public HexGrid grid;

    HexCell currentCell;
    HexUnit selectedUnit;

    Client client;

    void Start()
    {
        grid = FindObjectOfType<HexGrid>();
        client = FindObjectOfType<Client>();
    }

    void Update()
    {
        if(GameManager.Instance.editor || client.player.canPlay && !EventSystem.current.IsPointerOverGameObject())
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
            {
                Debug.Log("test : " + grid.GetPath().Count);
                selectedUnit.Travel(grid.GetPath());
            }
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
        grid.FindPath(cellStart, cellEnd, selectedUnit);

        cellStart.Unit.Travel(grid.GetPath());
        grid.ClearPath();
    }
}
