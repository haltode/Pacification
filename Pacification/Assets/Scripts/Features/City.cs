using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : Feature
{
    // TODO : add on-map representation (conflicts with ressources to be expected considering their current implementation)

    public enum CitySize
    {
        SETTLEMENT,
        CITY,
        MEGALOPOLIS
    }

    private CitySize size;
    private int hp;
    public int maxHP; //La ville est réparée lorsqu'elle évolue.
    public int pop;

    //economy
    public float perTurnMoney;
    public float perTurnScience;
    public float prodRate;
    public float happiness;
    public float happinessMalus; //Malus temporaire si la ville est endommagée, il y a aussi un malus permanent lors d'une attaque (cf attacker.cf : attack(City))

    //buildings level
    public int scienceLevel;
    public int moneyLevel;
    public int prodLevel;
    public int happinessLevel;

    //for info panels
    public int[,] buildings = new int[4,3]; // [(int)BuildingType, level] - amount of buildings per type per level

    public City (Player owner, HexCell location)
    {
        this.owner = owner;
        this.location = location;
        type = FeatureType.CITY;
        size = CitySize.SETTLEMENT;
        hp = 600;
        maxHP = hp;
        pop = 100;

        perTurnScience = 0.5f;
        perTurnMoney = 0.1f;
        prodRate = 1f;
        happiness = 1f;
        happinessMalus = 1f;

        // TODO : couleur du joueur
    }

    public void Update()
    {
        pop = (int)(pop * happiness * happinessMalus * 1.1f);

        owner.money += (int)(pop * perTurnMoney * ((happiness * happinessMalus < 1) ? (happiness * happinessMalus) : 1f));
        owner.science += (int)(pop * perTurnScience * ((happiness * happinessMalus < 1) ? (happiness * happinessMalus) : 1f));

        if (size == CitySize.SETTLEMENT && pop >= 1000)
        {
            size = CitySize.CITY;
            hp = 900;
            maxHP = hp;
        }
        else if (size == CitySize.CITY && pop >= 5000)
        {
            size = CitySize.MEGALOPOLIS;
            hp = 1300;
            maxHP = hp;
        }

        happinessMalus = (float)(hp / maxHP); //damaged cities get a happiness malus, for obvious reasons people are not happy to be on fire
    }

    public void LevelupBuilding(int type)
    {
        switch (type)
        {
            case 0:
                if (scienceLevel < 3)
                    scienceLevel++;
                break;

            case 1:
                if (moneyLevel < 3)
                    moneyLevel++;
                break;

            case 2:
                if (prodLevel < 3)
                    prodLevel++;
                break;

            case 3:
                if (happinessLevel < 3)
                    happinessLevel++;
                break;

            default:
                Debug.Log("REEEEEEEEEEEE");
                break;
        }
    }

    public void Build (string buildingType)
    {
        if (buildingType == "Science")
            CityBuilding.Build(CityBuilding.BuildingType.SCIENCE, this);
        if (buildingType == "Money")
            CityBuilding.Build(CityBuilding.BuildingType.MONEY, this);
        if (buildingType == "Prod")
            CityBuilding.Build(CityBuilding.BuildingType.PROD, this);
        if (buildingType == "Happiness")
            CityBuilding.Build(CityBuilding.BuildingType.HAPPINESS, this);
    }

    public int Hp
    {
        get { return hp; }
        set { hp = value; }
    }

    public CitySize Size
    {
        get { return size; }
        set { size = value; }
    }
}