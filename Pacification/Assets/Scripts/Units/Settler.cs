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
        currMVT = 0;
        hp = 100;
        MaxHP = hp;
        // TODO : couleur du joueur
    }

    public void Settle()
    {
        if (hasMadeAction)
            return;

        anim.animator.SetInteger("AnimPar", 2);

        owner.client.Send("CUNI|CIT|" + (int)type + "#" + hexUnit.location.coordinates.X + "#" + hexUnit.location.coordinates.Z);
        hasMadeAction = true;
    }
}