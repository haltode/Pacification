using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Unit
{
    public Worker(Player owner)
    {
        this.owner = owner;
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