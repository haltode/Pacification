using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : Feature
{
    //Add different buildings supports

    public enum CitySize
    {
        SETTLEMENT,
        CITY,
        MEGALOPOLIS
    }

    private CitySize size;
    private int id;
    private int hp;

    public City (ref Player owner, HexCell position)
    {
        this.owner = owner;
        this.position = position;
        type = FeatureType.CITY;
        size = CitySize.SETTLEMENT;
        hp = 600;

        id = owner.AddCity(this);

        // TODO : couleur du joueur
    }

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public int Hp
    {
        get { return hp; }
        set { hp = value; }
    }

    public CitySize Size
    {
        get { return size; }
        set { size = value; }
    }
}