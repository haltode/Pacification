using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int unitID;
    private int cityID;

    private Dictionary<int, Unit> playerUnits;
    //private Dictionary<int, City> playerCities;
    // TODO : couleur du joueur
    // TODO : tech tree

    public GameObject ActivePlayerCanvas;

    public Player()
    {
        unitID = 0;
        cityID = 0;

        playerUnits = new Dictionary<int, Unit>();
        //playerCities = new Dictionary<int, City>();
    }

    public void Start()
    {
        //NEED FIXING
        ActivePlayerCanvas = GameObject.FindGameObjectWithTag("Player");

        if(ActivePlayerCanvas)
            Debug.Log("OK");
        else
            Debug.Log("Unfound");
        ////////
    }

    public void TakeTurn()
    {
        //Set player ability to play to true
        ActivePlayerCanvas.SetActive(true);
    }

    public void EndTurn()
    {
        //Set player ability to play to true
        ActivePlayerCanvas.SetActive(false);
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
    }
    /*
    public void AddCity(City city)
    {
        cityID++;
        playerCities.Add(cityID, city);

        return cityID;
    }
    public void RemoveCity(City city)
    {
        playerCities.Remove(city.Id);
    }
    */
}
