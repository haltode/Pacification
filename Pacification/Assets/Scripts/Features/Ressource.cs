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

    private ResourceType resourceType;
    private int hp;
    private int featureIndexOffset;
    private int[] resourceProd = new int[] {75, 50, 25, 1, 250, 100};

    public Resource (Player owner, HexCell location, ResourceType resourceType)
    {
        this.owner = owner;
        this.location = location;
        this.resourceType = resourceType;
        type = FeatureType.RESOURCE;
        hp = 700;
        featureIndexOffset = 4 + (int)resourceType;
    }

    public void Update()
    {
        owner.resources[(int)resourceType] += resourceProd[(int)resourceType];
    }
}