using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{

    protected HexUnit hexUnit;
    public GameObject hexUnitGameObect;

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

    public static bool CanAttack(Unit unit)
    {
        return (unit.Type == UnitType.REGULAR || unit.Type == UnitType.RANGED || unit.Type == UnitType.HEAVY);
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
}