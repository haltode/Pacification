using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settler : Unit
{
    public Settler(Player owner)
    {
        this.owner = owner;
        type = UnitType.WORKER;
        mvtSPD = 2;
        hp = 100;

        id = owner.AddUnit(this);

        // TODO : couleur du joueur
    }

    public void Settle()
    {
        new City(ref owner, hexUnit.location);
        owner.RemoveUnit(this);
    }
}