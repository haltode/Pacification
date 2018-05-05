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
        maxHP = hp;

        // TODO : couleur du joueur
    }

    public void Exploit()
    {
        // TODO quand les ressources seront gérées
    }

    public bool AddRoad(HexCell roadCell)
    {
        HexCell currentCell = hexUnit.location;
        bool isNeighbor = false;
        HexDirection roadDir = HexDirection.NE;
        for(HexDirection dir = HexDirection.NE; dir <= HexDirection.NW && !isNeighbor; ++dir)
        {
            HexCell neighbor = currentCell.GetNeighbor(dir);
            if(neighbor == roadCell)
            {
                isNeighbor = true;
                roadDir = dir;
            }
        }
        if(!isNeighbor || !currentCell.IsReachable(roadDir))
            return false;

        currentCell.AddRoad(roadDir);
        return true;
    }
}