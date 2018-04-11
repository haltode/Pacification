using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Unit
{
    public Worker(ref Player owner, ref HexCell position)
    {
        this.owner = owner;
        this.position = position;
        type = UnitType.WORKER;
        mvtSPD = 2;
        hp = 300;

        id = owner.AddUnit(this);

        // TODO : couleur du joueur
    }

    public void Exploit()
    {
        // TODO quand les ressources seront gérées
    }
}