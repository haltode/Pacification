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

    List<Unit> playerUnits;
    List<City> playerCities;
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

        playerUnits = new List<Unit>();
        playerCities = new List<City>();

        canPlay = false;

        hexGrid = Object.FindObjectOfType<HexGrid>();
    }

    public void AddUnit(Unit.UnitType type, HexCell location)
    {
        Unit unit = null;
        int unitID = playerUnits.Count - 1;
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
        playerUnits.Remove(unit);
        unit = null;
    }
    
    public void AddCity(HexCell location)
    {
        int cityID = playerCities.Count - 1;
        City city = new City(this, cityID, location);
        playerCities.Add(city);
    }

    public void RemoveCity(City city)
    {
        playerCities.Remove(city);
        city = null;
    }
    
    public void LevelUp()
    {
        unitLevel++;

        for(int i = 0; i < playerUnits.Count; ++i)
        {
            Unit u = playerUnits[i];

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