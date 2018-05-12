using UnityEngine;

public class NewMapMenu : MonoBehaviour
{
    public HexGrid hexGrid;
    public HexMapGenerator mapGenerator;

    bool generateMaps = true;

    public void Open()
    {
        gameObject.SetActive(true);
        HexMapCamera.Locked = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HexMapCamera.Locked = false;
    }

    public void ToggleMapGeneration(bool toggle)
    {
        generateMaps = toggle;
    }

    void CreateMap(int sizeX, int sizeZ)
    {
        if(generateMaps)
        {
            mapGenerator = FindObjectOfType<HexMapGenerator>();
            mapGenerator.GenerateMap(sizeX, sizeZ, 0);
        }
        else
        {
            hexGrid = FindObjectOfType<HexGrid>();
            hexGrid.CreateMap(sizeX, sizeZ);
        }
        HexMapCamera.ValidatePosition();
        Close();
    }

    public void CreateSmallMap()
    {
        CreateMap(20, 20);
    }

    public void CreateMediumMap()
    {
        CreateMap(40, 40);
    }

    public void CreateLargeMap()
    {
        CreateMap(80, 80);
    }
}