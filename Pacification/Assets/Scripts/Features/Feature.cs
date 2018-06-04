using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feature
{
    public enum FeatureType
    {
        CITY,
        RESOURCE
    }

    public Player owner;
    public HexCell location;
    protected FeatureType type;

    public Player Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    public FeatureType Type
    {
        get { return type; }
    }

    public HexCell Location
    {
        get { return location; }
        set { location = value; }
    }
}
