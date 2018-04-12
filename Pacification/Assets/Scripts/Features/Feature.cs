using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feature
{
    public enum FeatureType
    {
        CITY,
        RESSOURCE
    }

    protected Player owner;
    protected HexCell position;
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

    public HexCell Position
    {
        get { return position; }
        set { position = value; }
    }
}
