using UnityEngine;
using System;

public class AI : MonoBehaviour
{
    public enum Difficulty { EASY, NORMAL, HARD };

    Player aiPlayer;

    Difficulty difficultyLevel;
    int currentRound;
    int lastSpawnRound;

    public AI(Difficulty difficultyLevel)
    {
        aiPlayer = new Player("Google");
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
        aiPlayer.AddUnit(Unit.UnitType.REGULAR, location);
        if(difficultyLevel == Difficulty.NORMAL || difficultyLevel == Difficulty.HARD)
        {
            HexCell location2 = GetNearFreeCell(location);
            aiPlayer.AddUnit(Unit.UnitType.HEAVY, location2);
        }
    }

    HexCell GetSpawningLocation()
    {
        // TODO: get list of cities, choose random one, spawn near it
        return null;
    }

    HexCell GetNearFreeCell(HexCell location)
    {
        return null;
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