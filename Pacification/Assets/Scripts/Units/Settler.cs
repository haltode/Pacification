using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settler : Unit
{
    public Settler(ref Player owner, ref HexCell position)
    {
        this.owner = owner;
        this.position = position;
        type = UnitType.WORKER;
        mvtSPD = 2;
        hp = 100;

        id = owner.AddUnit(this);

        // TODO : couleur du joueur
    }

    public void Settle()
    {
        // TODO quand City.cs sera fait
    }
}
