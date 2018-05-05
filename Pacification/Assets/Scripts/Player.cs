using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player
{
    public HexGrid hexGrid;
    public Client client;

    private int unitLevel;

    public int money;
    public int science;
    public int production;
    public int happiness;

    public List<Unit> playerUnits;
    public List<City> playerCities;

    // TODO : couleur du joueur

    public bool canPlay;
    public string name;

    public DisplayInformationManager displayer;

    public Player(string name)
    {
        Debug.Log("Player spawned: " + name);
        this.name = name;
        unitLevel = 1;

        money = 1000;
        science = 0;
        production = 0;
        happiness = 5;

        playerUnits = new List<Unit>();
        playerCities = new List<City>();

        canPlay = false;
        client = Object.FindObjectOfType<Client>();
        hexGrid = Object.FindObjectOfType<HexGrid>();
    }

    public void InitialSpawnUnit()
    {

        displayer = Object.FindObjectOfType<DisplayInformationManager>();
        hexGrid = Object.FindObjectOfType<HexGrid>();

        List<HexCell> possibleLocation = new List<HexCell>();
        for(int i = 0; i < hexGrid.cells.Length; ++i)
        {
            HexCell cell = hexGrid.cells[i];
            if(!cell.IsUnderWater && !cell.Unit && !hexGrid.IsBorder(cell) && cell.Elevation <= 4)
                possibleLocation.Add(cell);
        }

        int randomCell = hexGrid.rnd.Next(possibleLocation.Count);
        HexCell spawnSettler = possibleLocation[randomCell];
        HexCell spawnAttacker = hexGrid.GetNearFreeCell(spawnSettler);

        AddUnits(Unit.UnitType.SETTLER, spawnSettler, Unit.UnitType.REGULAR, spawnAttacker);

        HexMapCamera.FocusOnPosition(spawnSettler.Position);
    }

    public void AddUnit(Unit.UnitType type, HexCell location)
    {
        client.Send("CUNI|UNC|" + (int)type + "#" + location.coordinates.X + "#" + location.coordinates.Z);
    }

    public void AddUnits(Unit.UnitType type, HexCell location, Unit.UnitType type2, HexCell location2)
    {
        client.Send("CUNM|UAA|" + (int)type + "#" + location.coordinates.X + "#" + location.coordinates.Z + "|" + (int)type2 + "#" + location2.coordinates.X + "#" + location2.coordinates.Z);
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

    public void NetworkTakeDamage(string data)
    {
        hexGrid = Object.FindObjectOfType<HexGrid>();

        string[] receivedData = data.Split('#');
        HexCell attackedCell = hexGrid.GetCell(new HexCoordinates(int.Parse(receivedData[0]), int.Parse(receivedData[1])));
        Unit unit = attackedCell.Unit.Unit;

        unit.Hp -= int.Parse(receivedData[2]);

        attackedCell.EnableHighlight(Color.red);

        if(unit.Hp <= 0)
            RemoveUnit(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        hexGrid = Object.FindObjectOfType<HexGrid>();

        unit.HexUnit.location.EnableHighlight(Color.red);
        playerUnits[unit.Id] = null;
        hexGrid.RemoveUnit(unit.HexUnit);
        //unit.HexUnit.location.Unit.Die();  // What purpose ?
    }

    public void MoveUnit(Unit unit, HexCell end)
    {
        int xStart = unit.HexUnit.location.coordinates.X;
        int zStart = unit.HexUnit.location.coordinates.Z;
        int xEnd = end.coordinates.X;
        int zEnd = end.coordinates.Z;

        client.Send("CMOV|" + xStart + "#" + zStart + "#" + xEnd + "#" + zEnd + "#");
    }

    public void NetworkMoveUnit(string data, bool isAI=false)
    {
        string[] receivedData = data.Split('#');

        int xStart = int.Parse(receivedData[0]);
        int zStart = int.Parse(receivedData[1]);

        int xEnd = int.Parse(receivedData[2]);
        int zEnd = int.Parse(receivedData[3]);

        HexCell cellStart = hexGrid.GetCell(new HexCoordinates(xStart, zStart));
        HexCell cellEnd = hexGrid.GetCell(new HexCoordinates(xEnd, zEnd));

        hexGrid.ClearPath();
        hexGrid.FindPath(cellStart, cellEnd, cellStart.Unit, isAI);

        cellStart.Unit.Travel(hexGrid.GetPath());
        hexGrid.ClearPath();
    }

    public Unit GetUnit(HexCell location)
    {
        for(int i = 0; i < playerUnits.Count; ++i)
            if(playerUnits[i] != null &&
                playerUnits[i].HexUnit.location == location)
                return playerUnits[i];
        return null;
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

        HexCell location = hexGrid.GetCell(new HexCoordinates(int.Parse(receivedData[1]), int.Parse(receivedData[2])));
        location.FeatureIndex = 0;
        City city = GetCity(location);
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
