using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Unit
{
    public Worker(Player owner)
    {
        this.owner = owner;
        type = UnitType.WORKER;
        hasMadeAction = false;
        mvtSPD = 2;
        hp = 300;
        maxHP = hp;

        // TODO : couleur du joueur
    }

    public void Exploit()
    {
        if (hasMadeAction)
            return;

        HexCell cell = hexUnit.location;
        if(cell.FeatureIndex <= 3)
            return;

        const int amelioration = 6;
        if(cell.FeatureIndex + amelioration < 16)
            owner.client.Send("CUNI|WEX|" + cell.coordinates.X + "#" + cell.coordinates.Z + "#" + amelioration);
        hasMadeAction = true;
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