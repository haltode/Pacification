using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome
{
    public enum Biomes
    {
        GRA_PL,
        GRA_HI,
        SNO_PL,
        SNO_HI,
        DES_PL,
        DES_HI,
        FOREST,
        ROC_MO,
        SNO_MO,
        OCEAN
    }

    public enum RessourceTypes
    {
        NONE,
        WOOD,
        ANIMALS,
        FOOD,
        STONE,
        STEEL,
        GOLD,
        DIAMOND
    }

    private Biomes type;
    private bool canAccess;
    private int viewRange;
    private float speedMult;
    private bool canBuildCity;
    private List<RessourceTypes> ressources;

    public Biome (Biomes type)
    {
        this.type = type;
        ressources = new List<RessourceTypes>();

        if (type == Biomes.GRA_PL)
        {
            viewRange = 3;
            speedMult = 1f;
            canAccess = true;
            canBuildCity = true;
            ressources.Add(RessourceTypes.ANIMALS);
            ressources.Add(RessourceTypes.FOOD);
        }
        else if (type == Biomes.GRA_HI)
        {
            viewRange = 5;
            speedMult = 1f;
            canAccess = true;
            canBuildCity = true;
            ressources.Add(RessourceTypes.STONE);
            ressources.Add(RessourceTypes.STEEL);
            ressources.Add(RessourceTypes.GOLD);
        }
        else if(type == Biomes.SNO_PL)
        {
            viewRange = 3;
            speedMult = 1f;
            canAccess = true;
            canBuildCity = true;
            ressources.Add(RessourceTypes.ANIMALS);
        }
        else if(type == Biomes.SNO_HI)
        {
            viewRange = 5;
            speedMult = 1f;
            canAccess = true;
            canBuildCity = true;
            ressources.Add(RessourceTypes.STONE);
            ressources.Add(RessourceTypes.STEEL);
            ressources.Add(RessourceTypes.GOLD);
        }
        else if(type == Biomes.DES_PL)
        {
            viewRange = 3;
            speedMult = 1f;
            canAccess = true;
            canBuildCity = true;
            ressources.Add(RessourceTypes.NONE);
        }
        else if(type == Biomes.DES_HI)
        {
            viewRange = 5;
            speedMult = 1f;
            canAccess = true;
            canBuildCity = true;
            ressources.Add(RessourceTypes.STONE);
            ressources.Add(RessourceTypes.STEEL);
            ressources.Add(RessourceTypes.GOLD);
        }
        else if(type == Biomes.FOREST)
        {
            viewRange = 1;
            speedMult = 0.5f;
            canAccess = true;
            canBuildCity = false;
            ressources.Add(RessourceTypes.ANIMALS);
            ressources.Add(RessourceTypes.WOOD);
        }
        else if(type == Biomes.ROC_MO)
        {
            viewRange = 4;
            speedMult = 0.5f;
            canAccess = true;
            canBuildCity = false;
            ressources.Add(RessourceTypes.STONE);
            ressources.Add(RessourceTypes.GOLD);
            ressources.Add(RessourceTypes.DIAMOND);
        }
        else if(type == Biomes.SNO_MO)
        {
            viewRange = 0;
            speedMult = 0f;
            canAccess = false;
            canBuildCity = false;
            ressources.Add(RessourceTypes.NONE);
        }
        else if(type == Biomes.OCEAN)
        {
            viewRange = 0;
            speedMult = 0f;
            canAccess = false;
            canBuildCity = false;
            ressources.Add(RessourceTypes.NONE);
        }
    }

    public Biomes Type
    {
        get { return type; }
    }

    public bool CanAccess
    {
        get { return canAccess; }
    }

    public int ViewRange
    {
        get { return viewRange; }
    }

    public float SpeedMult
    {
        get { return speedMult; }
    }

    public bool CanBuildCity
    {
        get { return canBuildCity; }
    }

    public List<RessourceTypes> Ressources
    {
        get { return ressources; }
    }
}
