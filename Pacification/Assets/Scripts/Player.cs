using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player
{
    public HexGrid hexGrid;

    private int unitID;
    private int cityID;
    private int unitLevel;

    public int money;
    public int science;
    public int production;
    public int happiness;

    private Dictionary<int, Unit> playerUnits;
    private Dictionary<int, City> playerCities;
    // TODO : couleur du joueur
    // TODO : tech tree

    public bool canPlay;

    public DisplayInformationManager displayer;

    public Player()
    {
        unitID = 0;
        cityID = 0;
        unitLevel = 1;

        money = 1000;
        science = 0;
        production = 0;
        happiness = 5;

        playerUnits = new Dictionary<int, Unit>();
        playerCities = new Dictionary<int, City>();

        canPlay = false;

        hexGrid = Object.FindObjectOfType<HexGrid>();
    }

    public int AddUnit(Unit unit)
    {
        unitID++;
        playerUnits.Add(unitID, unit);

        return unitID;
    }

    public void RemoveUnit(Unit unit)
    {
        playerUnits.Remove(unit.Id);
        unit = null;
    }

    public void CreateUnit(Unit.UnitType type, HexCell location)
    {
        Unit unit = null;
        if(type == Unit.UnitType.SETTLER)
            unit = new Settler(this);
        else if(type == Unit.UnitType.WORKER)
            unit = new Worker(this);
        else if(type == Unit.UnitType.REGULAR)
            unit = new Regular(this);
        else if(type == Unit.UnitType.RANGED)
            unit = new Ranged(this);
        else if(type == Unit.UnitType.HEAVY)
            unit = new Heavy(this);
        else
            Debug.Log("Unknown unit type");

        unit.hexUnitGameObect = GameObject.Instantiate(hexGrid.mainUnitPrefab);
        unit.HexUnit = unit.hexUnitGameObect.GetComponent<HexUnit>();
        unit.HexUnit.Unit = unit;
        UnitGraphics.SetGraphics(unit.hexUnitGameObect, hexGrid.unitPrefab[(int)type]);

        float orientation = UnityEngine.Random.Range(0f, 360f);
        hexGrid.AddUnit(unit.HexUnit, location, orientation);
    }
    
    public int AddCity(City city)
    {
        cityID++;
        playerCities.Add(cityID, city);

        return cityID;
    }

    public void RemoveCity(City city)
    {
        playerCities.Remove(city.Id);
        city = null;
    }
    
    public void LevelUp()
    {
        unitLevel++;

        foreach (KeyValuePair<int, Unit> entry in playerUnits)
        {
            Unit u = entry.Value;

            if (u.Type == Unit.UnitType.REGULAR)
                ((Regular)u).LevelUp();
            if (u.Type == Unit.UnitType.RANGED)
                ((Ranged)u).LevelUp();
            if (u.Type == Unit.UnitType.HEAVY)
                ((Heavy)u).LevelUp();
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
        Debug.Log(displayer != null); //Probleme a résoudre
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