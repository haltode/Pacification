using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settler : Unit
{
    public Settler(Player owner)
    {
        this.owner = owner;
        type = UnitType.SETTLER;
        hasMadeAction = false;
        mvtSPD = 2;
        hp = 100;
        maxHP = hp;
        // TODO : couleur du joueur
    }

    public void Settle()
    {
        if (hasMadeAction)
            return;

        owner.client.Send("CUNM|CIT|" + (int)type + "#" + hexUnit.location.coordinates.X + "#" + hexUnit.location.coordinates.Z + "|" + this.HexUnit.location.coordinates.X + "#" + this.HexUnit.location.coordinates.Z);
        hasMadeAction = true;
    }
}