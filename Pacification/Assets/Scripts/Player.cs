using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int unitID;
    private int cityID;
    private int unitLevel;

    private int money;

    private Dictionary<int, Unit> playerUnits;
    private Dictionary<int, City> playerCities;
    // TODO : couleur du joueur
    // TODO : tech tree

    public bool canPlay;

    public Player()
    {
        unitID = 0;
        cityID = 0;
        unitLevel = 1;

        money = 1000;

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

    public int Money
    {
        get { return money; }
        set { money = value; }
    }

    public int UnitLevel
    {
        get { return unitLevel; }
    }
}