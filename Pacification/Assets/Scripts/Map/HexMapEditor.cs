using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    enum OptionalToggle { Ignore, No, Yes }

    public HexGrid hexGrid;
    public Client client;

    private string data = "";
    private string previousData = "";
    private int activeTerrainBiomeIndex;
    private int activeElevation;
    private int activeFeature;
    private bool applyElevation;
    private int brushSize;

    bool editMode;
    bool isDrag;
    HexDirection dragDirection;
    HexCell previousCell;
    HexCell searchFromCell, searchToCell;

    OptionalToggle roadMode;

    void Start()
    {
        client = FindObjectOfType<Client>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
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

            if(editMode)
                EditCells(currentCell);
            else if(Input.GetKey(KeyCode.LeftShift) && searchToCell != currentCell)
            {
                if(searchFromCell != currentCell)
                {
                    if(searchFromCell)
                        searchFromCell.DisableHighlight();
                    searchFromCell = currentCell;
                    searchFromCell.EnableHighlight(Color.blue);
                    if(searchToCell)
                        hexGrid.FindPath(searchFromCell, searchToCell, 24);
                }
            }
            else if(searchFromCell && searchFromCell != currentCell)
            {
                if(searchToCell != currentCell)
                {
                    searchToCell = currentCell;
                    hexGrid.FindPath(searchFromCell, searchToCell, 24);
                }
            }
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
            data = "CEDI|" + cell.coordinates.X + "."  + cell.coordinates.Z + "#";

            if (activeTerrainBiomeIndex >= 0)
            {
                cell.TerrainBiomeIndex = activeTerrainBiomeIndex;
                data += activeTerrainBiomeIndex + "#";
            }
            else
                data += "-1#";

            if (applyElevation)
            {
                cell.Elevation = activeElevation;
                data += activeElevation + "#";
            }
            else
                data += "-1#";

            if (activeFeature > 0)
            {
                cell.FeatureIndex = activeFeature;
                data += activeFeature + "#";
            }else 
                data += "-1#";

            if (roadMode == OptionalToggle.No)
            {
                cell.RemoveRoads();
                data += "0.-1#";
            }
            else if (roadMode == OptionalToggle.Yes)
                data += "1.";
            else
                data += "-1.-1#";


            if(isDrag)
            {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (otherCell && roadMode == OptionalToggle.Yes)
                {
                    otherCell.AddRoad(dragDirection);

                    data += (int)dragDirection +"." + otherCell.coordinates.X + "." + otherCell.coordinates.Z + "#";
                }
            }
            else
                data += "-1#";

            if (data != previousData)
            {
                previousData = data;
                client.Send(data);
            }
        }
    }

    public void NetworkEditedCell(string data)
    {
        /* data :
            0: position[]
               -> 0 : X
               -> 1 : Z
            1: newBiomeIndex   // int
            2: newElevation    // int
            3: newFeature      // int 
            4: road[]          
               -> 0 : hasHoad
               -> 1 : dragDirec
               -> 2 : neighborCell.X
               -> 3 : neighborCell.Z
       */

        string[] receivedData = data.Split('#');
        string[] position = receivedData[0].Split('.');

        HexCell cell = hexGrid.GetCell(new HexCoordinates(int.Parse(position[0]), int.Parse(position[1])));

        if(cell)
        {
            string[] road = receivedData[4].Split('.');
            int newBiomeIndex = int.Parse(receivedData[1]);
            int newElevation = int.Parse(receivedData[2]);
            int newFeature = int.Parse(receivedData[3]);

            if (newBiomeIndex != -1)
                cell.TerrainBiomeIndex = newBiomeIndex;

            if(newElevation != -1)
                cell.Elevation = newElevation;

            if(newFeature != -1)
                cell.FeatureIndex = newFeature;

            if(road[0] == "0")
                cell.RemoveRoads();
            else if (road[1] != "-1")
            {
                HexCell otherCell = hexGrid.GetCell(new HexCoordinates(int.Parse(road[2]), int.Parse(road[3])));

                switch(road[1])
                {
                    case "0":
                        otherCell.AddRoad(HexDirection.NE);
                        break;

                    case "1":
                        otherCell.AddRoad(HexDirection.E);
                        break;

                    case "2":
                        otherCell.AddRoad(HexDirection.SE);
                        break;

                    case "3":
                        otherCell.AddRoad(HexDirection.SW);
                        break;

                    case "4":
                        otherCell.AddRoad(HexDirection.W);
                        break;

                    case "5":
                        otherCell.AddRoad(HexDirection.NW);
                        break;
                }
            }
        }
    }

    public void SetEditMode(bool toggle)
    {
        editMode = toggle;
        hexGrid.ShowUI(!toggle);
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

    public void SetBrushSize(float size)
    {
        brushSize = (int) size;
    }

    public void SetRoadMode(int mode)
    {
        roadMode = (OptionalToggle) mode;
    }
}
