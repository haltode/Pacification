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
    private int viewRange;
    private float speedMult;
    private List<RessourceTypes> ressources; //Types de ressources POSSIBLE pour le biome, d'où la présence du NONE à chaque fois

    /*
    private System.Object[] values = new System.Object[]
    {
        // [type, viewRange, speedMult, ressources]
        new System.Object[] {Biomes.GRASSY_PLAIN, 3, 1f, RessourceTypes.NONE, RessourceTypes.ANIMALS, RessourceTypes.FOOD},
        new System.Object[] {Biomes.GRASSY_HILL, 5, 1f, RessourceTypes.NONE, RessourceTypes.STONE, RessourceTypes.STEEL, RessourceTypes.GOLD},
        new System.Object[] {Biomes.SNOWY_PLAIN, 3, 1f, RessourceTypes.NONE, RessourceTypes.ANIMALS},
        new System.Object[] {Biomes.SNOWY_HILL, 5, 1f, RessourceTypes.NONE, RessourceTypes.STONE, RessourceTypes.STEEL, RessourceTypes.GOLD},
        new System.Object[] {Biomes.DESERT_PLAIN, 3, 1f, RessourceTypes.NONE},
        new System.Object[] {Biomes.DESERT_HILL, 5, 1f, RessourceTypes.NONE, RessourceTypes.STONE, RessourceTypes.STEEL, RessourceTypes.GOLD},
        new System.Object[] {Biomes.FOREST, 1, 0.5f, RessourceTypes.NONE, RessourceTypes.ANIMALS, RessourceTypes.WOOD},
        new System.Object[] {Biomes.ROCKY_MOUNTAIN, 4, 0.5f, RessourceTypes.NONE, RessourceTypes.STONE, RessourceTypes.GOLD, RessourceTypes.DIAMOND},
        new System.Object[] {Biomes.SNOWY_MOUNTAIN, 0, 0f, RessourceTypes.NONE},
        new System.Object[] {Biomes.OCEAN, 0, 0f, RessourceTypes.NONE},
    };
    */

    public Biome (Biomes type)
    {
        this.type = type;
        ressources = new List<RessourceTypes>();

        if (type == Biomes.GRA_PL)
        {
            viewRange = 3;
            speedMult = 1f;
            ressources.Add(RessourceTypes.NONE);
            ressources.Add(RessourceTypes.ANIMALS);
            ressources.Add(RessourceTypes.FOOD);
        }
        else if (type == Biomes.GRA_HI)
        {
            viewRange = 5;
            speedMult = 1f;
            ressources.Add(RessourceTypes.NONE);
            ressources.Add(RessourceTypes.STONE);
            ressources.Add(RessourceTypes.STEEL);
            ressources.Add(RessourceTypes.GOLD);
        }
        else if(type == Biomes.SNO_PL)
        {
            viewRange = 3;
            speedMult = 1f;
            ressources.Add(RessourceTypes.NONE);
            ressources.Add(RessourceTypes.ANIMALS);
        }
        else if(type == Biomes.SNO_HI)
        {
            viewRange = 5;
            speedMult = 1f;
            ressources.Add(RessourceTypes.NONE);
            ressources.Add(RessourceTypes.STONE);
            ressources.Add(RessourceTypes.STEEL);
            ressources.Add(RessourceTypes.GOLD);
        }
        else if(type == Biomes.DES_PL)
        {
            viewRange = 3;
            speedMult = 1f;
            ressources.Add(RessourceTypes.NONE);
        }
        else if(type == Biomes.DES_HI)
        {
            viewRange = 5;
            speedMult = 1f;
            ressources.Add(RessourceTypes.NONE);
            ressources.Add(RessourceTypes.STONE);
            ressources.Add(RessourceTypes.STEEL);
            ressources.Add(RessourceTypes.GOLD);
        }
        else if(type == Biomes.FOREST)
        {
            viewRange = 1;
            speedMult = 0.5f;
            ressources.Add(RessourceTypes.NONE);
            ressources.Add(RessourceTypes.ANIMALS);
            ressources.Add(RessourceTypes.WOOD);
        }
        else if(type == Biomes.ROC_MO)
        {
            viewRange = 4;
            speedMult = 0.5f;
            ressources.Add(RessourceTypes.NONE);
            ressources.Add(RessourceTypes.STONE);
            ressources.Add(RessourceTypes.GOLD);
            ressources.Add(RessourceTypes.DIAMOND);
        }
        else if(type == Biomes.SNO_MO)
        {
            viewRange = 0;
            speedMult = 0f;
            ressources.Add(RessourceTypes.NONE);
        }
        else if(type == Biomes.OCEAN)
        {
            viewRange = 0;
            speedMult = 0f;
            ressources.Add(RessourceTypes.NONE);
        }
    }

    public Biomes Type
    {
        get { return type; }
    }

    public int ViewRange
    {
        get { return viewRange; }
    }

    public float SpeedMult
    {
        get { return speedMult; }
    }

    public List<RessourceTypes> Ressources
    {
        get { return ressources; }
    }

    public bool CanAccess()
    {
        return !(type == Biomes.OCEAN || type == Biomes.SNO_MO);
    }

    public bool CanBuildCity()
    {
        return CanAccess() && !(type == Biomes.FOREST || type == Biomes.ROC_MO);
    }
}
