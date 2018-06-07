using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{

    protected HexUnit hexUnit;
    public GameObject hexGameObject;

    public enum UnitType
    {
        SETTLER,
        WORKER,
        REGULAR,
        RANGED,
        HEAVY
    }

    // TODO : couleur du joueur
    public Player owner;
    protected UnitType type;
    protected int mvtSPD;
    protected int hp;
    public int maxHP;
    public bool embark;
    public bool hasMadeAction;
    public int currMVT;

    public UnitType Type
    {
        get { return type; }
    }

    public int MvtSPD
    {
        get { return mvtSPD; }
        set { mvtSPD = value; }
    }

    public int Hp
    {
        get { return hp; }
        set { hp = value; }
    }

    public HexUnit HexUnit
    {
        get { return hexUnit; }
        set { hexUnit = value; }
    }

    public static bool CanAttack(UnitType type)
    {
        return (type == UnitType.REGULAR || type == UnitType.RANGED || type == UnitType.HEAVY);
    }

    public static UnitType StrToType(string type)
    {
        if(type == "settler")
            return UnitType.SETTLER;
        else if(type == "worker")
            return UnitType.WORKER;
        else if(type == "regular")
            return UnitType.REGULAR;
        else if(type == "ranged")
            return UnitType.RANGED;
        else
            return UnitType.HEAVY;
    }

    public string TypeToStr()
    {
        if(type == UnitType.SETTLER)
            return "Settler";
        else if(type == UnitType.WORKER)
            return "Worker";
        else if(type == UnitType.REGULAR)
            return "Regular";
        else if(type == UnitType.RANGED)
            return "Ranged";
        else
            return "Heavy";
    }

    public void SetGraphics(GameObject prefab)
    {
        float prevOrientation = 0f;
        if(hexGameObject != null)
            prevOrientation = hexUnit.Orientation;

        GameObject graphicsObject = null;

        for(int i = 0; i < hexGameObject.transform.childCount; ++i)
            if(hexGameObject.transform.GetChild(i).name == "Graphics")
                graphicsObject = hexGameObject.transform.GetChild(i).gameObject;

        if(graphicsObject != null)
            Object.Destroy(graphicsObject);

        Vector3 spawnPos = hexGameObject.transform.position + new Vector3(0, 1, 0);
        Quaternion orientation = Quaternion.Euler(0f, prevOrientation, 0f);

        GameObject instantiated = Object.Instantiate(prefab, spawnPos, orientation);
        instantiated.name = "Graphics";
        instantiated.transform.parent = hexGameObject.transform;
    }

    public void Update()
    {
        hasMadeAction = false;
        currMVT = 0;
    }
}