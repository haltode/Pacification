using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player
{
    public HexGrid hexGrid;
    private Client client;

    private int unitID;
    private int cityID;
    private int unitLevel;

    public int money;
    public int science;
    public int production;
    public int happiness;

    public List<Unit> playerUnits;
    public List<City> playerCities;

    // TODO : couleur du joueur
    // TODO : tech tree

    public bool canPlay;
    public string name;

    public DisplayInformationManager displayer;

    public Player(string name)
    {
        this.name = name;
        unitID = 0;
        cityID = 0;
        unitLevel = 1;

        money = 1000;
        science = 0;
        production = 0;
        happiness = 5;

        playerUnits = new List<Unit>();
        playerCities = new List<City>();

        canPlay = false;

        client = Object.FindObjectOfType<Client>();
    }

    public void InitialSpawnUnit()
    {
        displayer = Object.FindObjectOfType<DisplayInformationManager>();
        hexGrid = Object.FindObjectOfType<HexGrid>();

        List<HexCell> possibleLocation = new List<HexCell>();
        for(int i = 0; i < hexGrid.cells.Length; ++i)
        {
            HexCell cell = hexGrid.cells[i];
            if(!cell.IsUnderWater && !hexGrid.IsBorder(cell))
                possibleLocation.Add(cell);
        }

        int randomCell = hexGrid.rnd.Next(possibleLocation.Count);
        HexCell spawnSettler = possibleLocation[randomCell];
        HexCell spawnAttacker = hexGrid.GetNearFreeCell(spawnSettler);

        AddUnit(Unit.UnitType.SETTLER, spawnSettler);
        AddUnit(Unit.UnitType.REGULAR, spawnAttacker);

        HexMapCamera.FocusOnPosition(spawnSettler.Position);
    }

    public void AddUnit(Unit.UnitType type, HexCell location)
    {
        client.Send("CUNI|UNC|" + (int)type + "#" + location.coordinates.X + "#" + location.coordinates.Z);
    }

    public void NetworkAddUnit(string data)
    {
        string[] receivedData = data.Split('#');
        hexGrid = Object.FindObjectOfType<HexGrid>();

        Unit.UnitType type = (Unit.UnitType)int.Parse(receivedData[0]);
        HexCell location = hexGrid.GetCell(new HexCoordinates(int.Parse(receivedData[1]), int.Parse(receivedData[2])));
        Unit unit = null;
        int unitID = playerUnits.Count;

        if(type == Unit.UnitType.SETTLER)
            unit = new Settler(this, unitID);
        else if(type == Unit.UnitType.WORKER)
            unit = new Worker(this, unitID);
        else if(type == Unit.UnitType.REGULAR)
            unit = new Regular(this, unitID);
        else if(type == Unit.UnitType.RANGED)
            unit = new Ranged(this, unitID);
        else if(type == Unit.UnitType.HEAVY)
            unit = new Heavy(this, unitID);
        else
            Debug.Log("Unknown unit type");

        /* for upgraded model
        if (Unit.CanAttack(unit) && unitLevel > 10)
            (int)type += 3;
        */

        unit.hexUnitGameObect = GameObject.Instantiate(hexGrid.mainUnitPrefab);
        unit.HexUnit = unit.hexUnitGameObect.GetComponent<HexUnit>();
        unit.HexUnit.Unit = unit;
        UnitGraphics.SetGraphics(unit.hexUnitGameObect, hexGrid.unitPrefab[(int)type]);

        float orientation = UnityEngine.Random.Range(0f, 360f);
        hexGrid.AddUnit(unit.HexUnit, location, orientation);

        playerUnits.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        client.Send("CUNI|UND|" + unit.HexUnit.location.coordinates.X + "#" + unit.HexUnit.location.coordinates.Z);
    }

    public void NetworkRemoveUnit(string data)
    {
        hexGrid = Object.FindObjectOfType<HexGrid>();

        string[] receivedData = data.Split('#');
        Unit unit = hexGrid.GetCell(new HexCoordinates(int.Parse(receivedData[0]), int.Parse(receivedData[1]))).Unit.Unit;
        hexGrid.RemoveUnit(unit.HexUnit);
        playerUnits[unit.Id] = null;
        unit = null;
    }

    public void AddCity(HexCell location, City.CitySize type)
    {
        client.Send("CUNI|CIC|" + (int)type + "#" + location.coordinates.X + "#" + location.coordinates.Z);
    }

    public void NetworkAddCity(string data)
    {
        string[] receivedData = data.Split('#');
        hexGrid = Object.FindObjectOfType<HexGrid>();

        City.CitySize type = (City.CitySize)int.Parse(receivedData[0]);
        HexCell location = hexGrid.GetCell(new HexCoordinates(int.Parse(receivedData[1]), int.Parse(receivedData[2])));
        location.FeatureIndex = 1;
        location.IncreaseVisibility();

        int cityID = playerCities.Count;
        City city = new City(this, cityID, location);

        Vector3 position = location.Position;
        float hash = HexMetrics.SampleHashGrid(position);
        city.instance = GameObject.Instantiate(
            hexGrid.cityPrefab[(int)type],
            position,
            Quaternion.Euler(0f, 360f * hash, 0f));

        playerCities.Add(city);
    }

    public void RemoveCity(City city)
    {
        client.Send("CUNI|CID|" + city.Position.Position.x  + "#" + city.Position.Position.z);
    }

    public void NetworkRemoveCity(string data)
    {
        string[] receivedData = data.Split('#');
        hexGrid = Object.FindObjectOfType<HexGrid>();

        City city = GetCity(hexGrid.GetCell(new HexCoordinates(int.Parse(receivedData[1]), int.Parse(receivedData[2]))));
        Object.Destroy(city.instance);
        playerUnits[city.Id] = null;
        city = null;
    }

    public City GetCity(HexCell location)
    {
        for(int i = 0; i < playerCities.Count; ++i)
            if(playerCities[i] != null &&
                playerCities[i].Position == location)
                return playerCities[i];
        return null;
    }
    
    public void LevelUp()
    {
        client.Send("CUNI|UNL|0");
    }

    public void NetworkLevelUp()
    {
        if (unitLevel < 20)
        {
            unitLevel++;

            foreach (Unit u in playerUnits)
            {
                if (u.Type == Unit.UnitType.REGULAR)
                    ((Regular)u).LevelUp();
                if (u.Type == Unit.UnitType.RANGED)
                    ((Ranged)u).LevelUp();
                if (u.Type == Unit.UnitType.HEAVY)
                    ((Heavy)u).LevelUp();
            }
        }
    }

    public void IncreaseUnitLevel(int target)
    {
        while(unitLevel < target)
            LevelUp();
    }

    public void SetDisplayer()
    {
        displayer = Object.FindObjectOfType<DisplayInformationManager>();
        //Debug.Log(displayer != null); //Probleme a résoudre
    }

    public void UpdateMoneyDisplay()
    {
        displayer.UpdateMoneyDisplay(money);
    }

    public void UpdateScienceDisplay()
    {
        displayer.UpdateScienceDisplay(science);
    }

    public void UpdateProductionDisplay()
    {
        displayer.UpdateProductionDisplay(production);
    }

    public void UpdateHappinessDisplay()
    {
        displayer.UpdateHappinessDisplay(happiness);
    }

    public int UnitLevel
    {
        get { return unitLevel; }
    }
}