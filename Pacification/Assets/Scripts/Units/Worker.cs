using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Unit
{
    public Worker(Player owner, int id)
    {
        this.owner = owner;
        this.id = id;
        type = UnitType.WORKER;
        mvtSPD = 2;
        hp = 300;

        // TODO : couleur du joueur
    }

    public void Exploit()
    {
        // TODO quand les ressources seront gérées
    }
}