﻿using System.Collections;
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
    int[] resourceProd = {75, 50, 25, 1, 250, 100};

    public Resource(Player owner, HexCell location, ResourceType ressourceType)
    {
        Owner = owner;
        Location = location;
        RessourceType = ressourceType;
        Type = FeatureType.RESOURCE;
        Hp = 700;
        MaxHp = Hp;
    }

    public void Update()
    {
        Owner.resources[(int)RessourceType] += resourceProd[(int)RessourceType];
    }

    public bool Exploited
    {
        get { return Location.FeatureIndex > 10; }
    }
}