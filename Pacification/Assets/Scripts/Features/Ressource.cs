using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : Feature
{
    public enum ResourceType
    {
        IRON,
        GOLD,
        DIAMOND,
        HORSES,
        WOOD,
        FOOD
    }

    readonly ResourceType RessourceType;
    public int Hp;
    int featureIndexOffset;
    int[] resourceProd = new int[] {75, 50, 25, 1, 250, 100};

    public Resource(Player owner, HexCell location, ResourceType RessourceType)
    {
        this.owner = owner;
        this.location = location;
        this.RessourceType = RessourceType;
        type = FeatureType.RESOURCE;
        Hp = 700;
        featureIndexOffset = 4 + (int)RessourceType;
    }

    public void Update()
    {
        owner.resources[(int)RessourceType] += resourceProd[(int)RessourceType];
    }

    public bool Exploited
    {
        get { return location.FeatureIndex > 10; }
    }
}