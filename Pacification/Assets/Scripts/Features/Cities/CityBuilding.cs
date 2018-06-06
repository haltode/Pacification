using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CityBuilding
{
    public enum BuildingType
    {
        SCIENCE,
        MONEY,
        PROD,
        HAPPINESS
    }

    public static int[] price = new int[] {500, 500, 500, 500, 2000, 3000, 2000, 1500, 5000, 8000, 4000, 4000};
    public static float[] multiplier = new float[] {0.01f, 0.01f, 0.01f, 0.01f, 0.05f, 0.05f, 0.05f, 0.05f, 0.08f, 0.08f, 0.08f, 0.08f};

    public static void Build(BuildingType type, City city)
    {
        int level = city.scienceLevel;

        if (!RessourceCheck((int)type, level, city))
            return;

        switch ((int)type)
        {
            case 0:
                city.perTurnScience += multiplier[(level * ((int)type + 1)) - 1];
                break;

            case 1:
                level = city.moneyLevel;
                city.perTurnMoney += multiplier[(level * ((int)type + 1)) - 1];
                break;

            case 2:
                level = city.prodLevel;
                city.prodRate += multiplier[(level * ((int)type + 1)) - 1];
                break;

            case 3:
                level = city.happinessLevel;
                city.happiness += multiplier[(level * ((int)type + 1)) - 1];
                break;

            default:
                Debug.Log("Unknown building type");
                break;
        }

        city.buildings[(int)type, level]++;
    }

    public static bool RessourceCheck(int type, int level, City city)
    {
        if (city.owner.resources[4] >= price[(level * ((int)type + 1)) - 1] / 10)
        {
            if (city.owner.money >= price[(level * ((int)type + 1)) - 1])
            {
                city.owner.money -= price[(level * ((int)type + 1)) - 1];
                city.owner.resources[4] -= price[(level * ((int)type + 1)) - 1] / 10;
                return true;
            }
        }

        return false;
    }
}