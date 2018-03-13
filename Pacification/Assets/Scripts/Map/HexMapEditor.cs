using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    enum OptionalToggle { Ignore, No, Yes }

    public HexGrid hexGrid;
    public Client client;

    string data = "";
    string previousData = "";
    int activeTerrainBiomeIndex;
    int activeElevation;
    int activeFeature;
    bool applyElevation;
    int brushSize;

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

    HexCell GetCellUnderCursor()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(inputRay, out hit))
            return hexGrid.GetCell(hit.point);
        return null;
    }

    void HandleInput()
    {
        HexCell currentCell = GetCellUnderCursor();
        if(currentCell)
        {
            if(previousCell && previousCell != currentCell)
                ValidateDrag(currentCell);
            else
                isDrag = false;

            if(editMode)
                EditCells(currentCell);
            // Pathfinding (searching + showing the path between cells)
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

        EditCell(center);

        for(int r = 0, z = centerZ - brushSize; z <= centerZ; ++z, ++r)
            for(int x = centerX - r; x <= centerX + brushSize; ++x)
                EditCellWithBrush(hexGrid.GetCell(new HexCoordinates(x, z)));

        for(int r = 0, z = centerZ + brushSize; z > centerZ; --z, ++r)
            for(int x = centerX - brushSize; x <= centerX + r; ++x)
                EditCellWithBrush(hexGrid.GetCell(new HexCoordinates(x, z)));
    }


    void EditCellWithBrush(HexCell cell)
    {
        if (cell)
        {
            if (activeTerrainBiomeIndex >= 0)
                cell.TerrainBiomeIndex = activeTerrainBiomeIndex;

            if (applyElevation)
                cell.Elevation = activeElevation;

            if (activeFeature > 0)
                cell.FeatureIndex = activeFeature;

            if (roadMode == OptionalToggle.No)
                cell.RemoveRoads();

            if (isDrag)
            {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (otherCell && roadMode == OptionalToggle.Yes)
                    otherCell.AddRoad(dragDirection);
            }
        }
    }

    void EditCell(HexCell cell)
    {
        if (cell)
        {
            data = "CEDI|" + cell.coordinates.X + "." + cell.coordinates.Z + "." + brushSize + "#";

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
            }
            else
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

            if (isDrag)
            {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (otherCell && roadMode == OptionalToggle.Yes)
                {
                    otherCell.AddRoad(dragDirection);
                    data += (int)dragDirection + "." + otherCell.coordinates.X + "." + otherCell.coordinates.Z + "#";
                }
            }
            else
                data += "-1#";

            if (data != previousData)
            {
                previousData = data;
                if (FindObjectOfType<Client>())
                    client.Send(data);
            }
        }
    }

    public void NetworkEditedCells(string data, int brush)
    {
        string[] receivedData = data.Split('#');
        string[] position = receivedData[0].Split('.');
        string newData = "#" + receivedData[1] + "#" + receivedData[2] + "#" + receivedData[3] + "#" + receivedData[4];

        int centerX = int.Parse(position[0]);
        int centerZ = int.Parse(position[1]);

        for (int r = 0, z = centerZ - brush; z <= centerZ; ++z, ++r)
            for (int x = centerX - r; x <= centerX + brush; ++x)
            {
                NetworkEditedCell(x + "." + z + newData);
            }
        for (int r = 0, z = centerZ + brush; z > centerZ; --z, ++r)
            for (int x = centerX - brush; x <= centerX + r; ++x)
            {
                NetworkEditedCell(x + "." + z + newData);
            }
    }

    public void NetworkEditedCell(string data)
    {
        string[] receivedData = data.Split('#');
        string[] position = receivedData[0].Split('.');

        int X = int.Parse(position[0]);
        int Z = int.Parse(position[1]);
        HexCell cell = hexGrid.GetCell(new HexCoordinates(X, Z));

        if(cell)
        {
            string[] road = receivedData[4].Split('.');
            int newBiomeIndex = int.Parse(receivedData[1]);
            int newElevation = int.Parse(receivedData[2]);
            int newFeature = int.Parse(receivedData[3]);

            if(newBiomeIndex != -1)
                cell.TerrainBiomeIndex = newBiomeIndex;
            if(newElevation != -1)
                cell.Elevation = newElevation;
            if(newFeature != -1)
                cell.FeatureIndex = newFeature;
            if(road[0] == "0")
                cell.RemoveRoads();
            else if(road[1] != "-1")
            {
                int neighborX = int.Parse(road[2]);
                int neighborZ = int.Parse(road[3]);
                HexCell otherCell = hexGrid.GetCell(new HexCoordinates(neighborX, neighborZ));

                otherCell.AddRoad(((HexDirection)int.Parse(road[1])));
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
