using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player
{
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