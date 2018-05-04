using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settler : Unit
{
    public Settler(Player owner, int id)
    {
        this.owner = owner;
        this.id = id;
        type = UnitType.SETTLER;
        mvtSPD = 2;
        hp = 100;

        // TODO : couleur du joueur
    }

    public void Settle()
    {
        owner.client.Send("CUNM|CIT|" + (int)type + "#" + hexUnit.location.coordinates.X + "#" + hexUnit.location.coordinates.Z + "|" + this.HexUnit.location.coordinates.X + "#" + this.HexUnit.location.coordinates.Z);
    }
}