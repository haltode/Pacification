using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : Feature
{
    public enum CitySize
    {
        SETTLEMENT,
        CITY,
        MEGALOPOLIS
    }

    public int pop;
    public int spawncount;

    //economy
    public float perTurnMoney;
    public float perTurnScience;
    public float perTurnPop;
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
        Owner = owner;
        Location = location;
        Type = FeatureType.CITY;
        Size = CitySize.SETTLEMENT;
        Hp = 600;
        MaxHp = Hp;
        pop = 100;

        perTurnScience = 0.0075f;
        perTurnMoney = 0.025f;
        perTurnPop = 1.05f;
        prodRate = 1f;
        happiness = 1f;
        happinessMalus = 1f;

        spawncount = 1;

        // TODO : couleur du joueur
    }

    public string Update()
    {
        spawncount = 0;
        pop = (int)(pop * happiness * happinessMalus * perTurnPop);

        Owner.money += (int)(Owner.economyMalus * pop * perTurnMoney * ((happiness * happinessMalus < 1) ? (happiness * happinessMalus) : 1f));
        Owner.science += (int)(Owner.economyMalus * pop * perTurnScience * ((happiness * happinessMalus < 1) ? (happiness * happinessMalus) : 1f));

        happinessMalus = (float)(Hp / MaxHp); //damaged cities get a happiness malus, for obvious reasons people are not happy to be on fire

        if(Size == CitySize.SETTLEMENT && pop >= 1000)
            return "|" + Location.coordinates.X + "#" + Location.coordinates.Z + "#1";
        else if(Size == CitySize.CITY && pop >= 5000)
            return "|" + Location.coordinates.X + "#" + Location.coordinates.Z + "#2";
        else
            return "";
    }

    public void LevelUp(string upgrade)
    {
        if(upgrade == "1")
        {
            Size = CitySize.CITY;
            Hp = 900;
            MaxHp = Hp;
            Location.FeatureIndex = 2;
            perTurnPop = 0.015f;
        }
        else if(upgrade == "2")
        {
            Size = CitySize.MEGALOPOLIS;
            Hp = 1300;
            MaxHp = Hp;
            Location.FeatureIndex = 3;
            perTurnPop = 0.005f;
        }
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
                Debug.Log("Unknown Building Type");
                break;
        }
    }

    public void Build(string buildingType)
    {
        if (buildingType == "science")
            CityBuilding.Build(CityBuilding.BuildingType.SCIENCE, this);
        if (buildingType == "money")
            CityBuilding.Build(CityBuilding.BuildingType.MONEY, this);
        if (buildingType == "prod")
            CityBuilding.Build(CityBuilding.BuildingType.PROD, this);
        if (buildingType == "happiness")
            CityBuilding.Build(CityBuilding.BuildingType.HAPPINESS, this);
    }

    public CitySize Size { get; set; }
}