using UnityEngine;
using System;
using System.Collections.Generic;

public class AI : MonoBehaviour
{
    public enum Difficulty { EASY, NORMAL, HARD };

    const int SpawnRadiusMin = 10;
    const int SpawnRadiusMax = 15;

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

    public void PlayTurn(int currentPlayerLvl)
    {
        ++currentRound;
        if(!IsSpawningTime())
            return;

        lastSpawnRound = currentRound;
        if(difficultyLevel == Difficulty.EASY)
            aiPlayer.IncreaseUnitLevel(currentPlayerLvl - 2);
        else if(difficultyLevel == Difficulty.NORMAL)
            aiPlayer.IncreaseUnitLevel(currentPlayerLvl + 1);
        else if(difficultyLevel == Difficulty.HARD)
            aiPlayer.IncreaseUnitLevel(currentPlayerLvl + 3);

        HexCell location = GetSpawningLocation();
        if(difficultyLevel == Difficulty.EASY)
            aiPlayer.AddUnit(Unit.UnitType.REGULAR, location);
        else
        {
            HexCell location2 = aiPlayer.hexGrid.GetNearFreeCell(location);
            aiPlayer.AddUnit(Unit.UnitType.REGULAR, location,Unit.UnitType.HEAVY, location2);
        }
    }

    HexCell GetSpawningLocation()
    {
        List<HexCell> possibleLocation = new List<HexCell>();
        int nbCities = ennemy.playerCities.Count;
        System.Random rnd = new System.Random();
        int choosenCity = rnd.Next(nbCities);
        HexCell location = ennemy.playerCities[choosenCity].Position;

        for(int i = 0; i < aiPlayer.hexGrid.cells.Length; ++i)
        {
            HexCell cell = aiPlayer.hexGrid.cells[i];
            int dist = cell.coordinates.DistanceTo(location.coordinates);
            if(dist <= SpawnRadiusMax && dist >= SpawnRadiusMin)
                possibleLocation.Add(cell);
        }

        int randomCell = rnd.Next(possibleLocation.Count);
        return possibleLocation[randomCell];
    }

    bool IsSpawningTime()
    {
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
}