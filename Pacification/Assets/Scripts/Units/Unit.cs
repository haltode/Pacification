using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public enum UnitType
    {
        WORKER,
        SETTLER,
        REGULAR,
        RANGED,
        HEAVY
    }

    // TODO : couleur du joueur
    protected Player owner;
    protected HexCell position;
    protected UnitType type;
    protected int mvtSPD;
    protected int hp;
    protected int id;

    public Player Owner
    {
        get { return owner; }
        set { owner = value; }
    }

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

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public HexCell Position
    {
        get { return position; }
        set { position = value; }
    }

    public bool CanAttack(Unit unit)
    {
        return (unit.Type == UnitType.REGULAR || unit.Type == UnitType.RANGED || unit.Type == UnitType.HEAVY);
    }

    public void MoveTo (HexCell target)
    {
        position = target;
    }
}