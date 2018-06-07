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

    public static int[] price = new int[] {/*level 1:*/ 700, 400, 500, 500, /*level 2:*/ 2500, 2000, 1500, 1500, /*level 3:*/ 7000, 5000, 3000, 5000}; //= gold price. Wood price = gold price * 0.1
    public static float[] multiplier = new float[] {/*level 1:*/ 0.00001f, 0.00005f, 0.01f, 0.0001f, /*level 2:*/ 0.00005f, 0.0001f, 0.05f, 0.0005f, /*level 3:*/ 0.0001f, 0.0005f, 0.1f, 0.001f};

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
        if (city.Owner.resources[4] >= price[(level * ((int)type + 1)) - 1] / 10)
        {
            if (city.Owner.money >= price[(level * ((int)type + 1)) - 1])
            {
                city.Owner.money -= price[(level * ((int)type + 1)) - 1];
                city.Owner.resources[4] -= price[(level * ((int)type + 1)) - 1] / 10;
                return true;
            }
        }

        return false;
    }
}