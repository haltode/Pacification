using UnityEngine;
using System;
using System.Collections.Generic;

public class AI
{
    public enum Difficulty { EASY, NORMAL, HARD };

    const int SpawnRadiusMin = 10;
    const int SpawnRadiusMax = 15;
    const int MaxBarbarianUnits = 10;
    const int MovePointPerUnit = 7;

    Player aiPlayer;
    Player ennemy;

    Difficulty difficultyLevel;
    int currentRound;
    int lastSpawnRound;

    public AI(Player ennemy, Difficulty difficultyLevel)
    {
        aiPlayer = new Player("Google");
        this.ennemy = ennemy;
        this.difficultyLevel = difficultyLevel;
        currentRound = 0;
        lastSpawnRound = 0;
    }

    public void PlayTurn()
    {
        ++currentRound;
        if(IsSpawningTime())
            SpawnBarbarianUnits();
        else
            foreach(Unit unit in aiPlayer.playerUnits)
                DoActionBarbarianUnit(unit);
    }

    void SpawnBarbarianUnits()
    {
        UnityEngine.Object.FindObjectOfType<SoundManager>().PlayBarbarianSpawn();
        lastSpawnRound = currentRound;
        // TODO: need level up
        /*int currentPlayerLvl = ennemy.UnitLevel;
        if(difficultyLevel == Difficulty.EASY)
            aiPlayer.IncreaseUnitLevel(currentPlayerLvl - 2);
        else if(difficultyLevel == Difficulty.NORMAL)
            aiPlayer.IncreaseUnitLevel(currentPlayerLvl + 1);
        else if(difficultyLevel == Difficulty.HARD)
            aiPlayer.IncreaseUnitLevel(currentPlayerLvl + 3);*/

        HexCell location = GetSpawningLocation();
        string cmd = (int)Unit.UnitType.REGULAR + "#" + location.coordinates.X + "#" + location.coordinates.Z;
        aiPlayer.NetworkAddUnit(cmd);

        if(difficultyLevel == Difficulty.NORMAL || difficultyLevel == Difficulty.HARD)
        {
            HexCell location2 = aiPlayer.hexGrid.GetNearFreeCell(location);
            string cmd2 = (int)Unit.UnitType.HEAVY + "#" + location2.coordinates.X + "#" + location2.coordinates.Z;
            aiPlayer.NetworkAddUnit(cmd2);
        }
    }

    HexCell GetSpawningLocation()
    {
        List<HexCell> possibleLocation = new List<HexCell>();
        int nbCities = ennemy.playerCities.Count;
        System.Random rnd = ennemy.hexGrid.rnd;
        int choosenCity = rnd.Next(nbCities);
        HexCell location = ennemy.playerCities[choosenCity].Position;

        for(int i = 0; i < aiPlayer.hexGrid.cells.Length; ++i)
        {
            HexCell cell = aiPlayer.hexGrid.cells[i];
            int dist = cell.coordinates.DistanceTo(location.coordinates);
            if(dist <= SpawnRadiusMax && dist >= SpawnRadiusMin && 
                !aiPlayer.hexGrid.IsBorder(cell) && !cell.Unit && !cell.IsUnderWater)
                possibleLocation.Add(cell);
        }

        int randomCell = rnd.Next(possibleLocation.Count);
        return possibleLocation[randomCell];
    }

    bool IsSpawningTime()
    {
        if(ennemy.playerCities.Count == 0)
            return false;
        if(aiPlayer.playerUnits.Count >= MaxBarbarianUnits)
            return false;

        int diff = currentRound - lastSpawnRound;
        int randomRoundSpan = 0;
        int diffNbRound = 0;
        System.Random rnd = new System.Random();
        if(difficultyLevel == Difficulty.EASY)
        {
            randomRoundSpan = rnd.Next(2);
            diffNbRound = 15;
        }
        else if(difficultyLevel == Difficulty.NORMAL)
        {
            randomRoundSpan = rnd.Next(3);
            diffNbRound = 13;
        }
        else if(difficultyLevel == Difficulty.HARD)
        {
            randomRoundSpan = rnd.Next(3);
            diffNbRound = 10;
        }
        int sign = (UnityEngine.Random.value < 0.5) ? 1 : -1;
        return diff > (diffNbRound + randomRoundSpan * sign);
    }

    void MovePathfinding(Unit unit, HexCell end)
    {
        HexCell start = unit.HexUnit.location;
        aiPlayer.hexGrid.FindPath(start, end, unit.HexUnit, isAI:true);
        if(!aiPlayer.hexGrid.currentPathExists)
        {
            aiPlayer.hexGrid.ClearPath();
            return;
        }
        List<HexCell> pathToCity = aiPlayer.hexGrid.GetPath();
        HexCell targetCell = start;
        int movePoints = 0;
        int index = 0;
        while(movePoints < MovePointPerUnit && index < pathToCity.Count)
        {
            movePoints += unit.HexUnit.GetMoveCost(targetCell, pathToCity[index]);
            targetCell = pathToCity[index];
            ++index;
        }
        aiPlayer.hexGrid.ClearPath();
        aiPlayer.MoveUnit(unit, targetCell, isAI:true);
    }

    void MoveBarbarianUnit(Unit unit)
    {
        City target = null;
        int bestDist = Int32.MaxValue;
        foreach(City city in ennemy.playerCities)
        {
            int dist = unit.HexUnit.location.coordinates.DistanceTo(city.Position.coordinates);
            if(dist < bestDist)
            {
                bestDist = dist;
                target = city;
            }
        }
        if(target == null)
            return;

        MovePathfinding(unit, target.Position);
    }

    bool TryAttackCity(Unit unit)
    {
        return false;
    }

    void DoActionBarbarianUnit(Unit unit)
    {
        bool canAttack = TryAttackCity(unit);
        if(!canAttack)
            MoveBarbarianUnit(unit);
    }
}