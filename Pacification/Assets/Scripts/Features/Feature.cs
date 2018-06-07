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

    public Player Owner { get; set; }

    public FeatureType Type { get; set; }

    public HexCell Location { get; set; }

    public int Hp { get; set; }

    public int MaxHp { get; set; }
}
