using System;
using System.Collections.Generic;
using System.Linq;
using TheGameOfDice.BusinessLogic.Interfaces;
using TheGameOfDice.Entity;

namespace TheGameOfDice.BusinessLogic
{
    public class DiceGame : IDiceGame
    {
        #region Private Members - Game State Variables

        private Dictionary<int, int> _PlayersAccumulatedTotalPoints; // PlayerId : Rank
        private int[] _PlayerScoresArr;
        private int[] _PlayerConsecutiveOnesCount;

        private int _NumOfPlayers;
        #endregion

        public void StartGame()
        {
            (int numOfPlayers, int pointsToAccumulate) = ReadAndValidateInput();
            InitiateGame(numOfPlayers);

            int round = 1, rank = 1, curPlayer = GetRandomInRange(1, numOfPlayers);

            while (_PlayersAccumulatedTotalPoints.Count != numOfPlayers)
            {
                Console.WriteLine($"\n\t\t\t****  Round := {round}, curPlayer := {curPlayer}  ****");

                int diceVal = RollDice(curPlayer);
                _PlayerScoresArr[curPlayer] += diceVal;
                if (diceVal == 1)
                    _PlayerConsecutiveOnesCount[curPlayer] += 1;

                if (_PlayerScoresArr[curPlayer] >= pointsToAccumulate)
                {
                    _PlayersAccumulatedTotalPoints.Add(curPlayer, rank);
                    Console.WriteLine($"[Player-{curPlayer}] accumulated total points. Rank := {rank++}");
                }

                LogCurrentRoundDetails(round, curPlayer, diceVal, rank);

                int nextPlayer = SelectNextPlayer(curPlayer, diceVal, numOfPlayers);
                curPlayer = nextPlayer;
                round++;
            }

            Console.WriteLine("\t\t\t****  All the players accumulated total points!  ****");
            Console.WriteLine("\t\t\t****                Game Ended!!                 ****");
        }

        #region Private Methods

        private (int numOfPlayers, int target) ReadAndValidateInput()
        {
            int numOfPlayers = 0, pointsToAccumulate = 0;

            Console.WriteLine("\nPlease enter number of players := ");
            var nStr = Console.ReadLine();
            Console.WriteLine("\nPlease enter target M value := ");
            var mStr = Console.ReadLine();

            if (!int.TryParse(nStr, out numOfPlayers) || !int.TryParse(mStr, out pointsToAccumulate))
                throw new InvalidInputException("Invalid input, input should be numbers");

            if (numOfPlayers <= 0 || pointsToAccumulate <= 0)
                throw new InvalidInputException("Invalid input, n and m should be > 0");
            return (numOfPlayers, pointsToAccumulate);
        }

        private void InitiateGame(int numOfPlayers)
        {
            _NumOfPlayers = numOfPlayers;
            _PlayersAccumulatedTotalPoints = new();
            _PlayerScoresArr = new int[numOfPlayers + 1];
            _PlayerConsecutiveOnesCount = new int[numOfPlayers + 1];
        }

        private int GetRandomInRange(int start, int end) => new Random().Next(start, end + 1);

        private int RollDice(int curPlayer)
        {
            Console.WriteLine($"\n[Player-{curPlayer}], its your turn. please press R to roll the dice");
            ConsoleKeyInfo input = Console.ReadKey();
            do
            {
                if (input.KeyChar == 'r' || input.KeyChar == 'R')
                {
                    return GetRandomInRange(1, 6);
                }
                Console.WriteLine($"\n[Player-{curPlayer}], invalid key pressed. please try again");
                input = Console.ReadKey();
            } while (true);
        }

        /// <summary>
        /// Get Id of Player with next turn
        /// </summary>
        /// <param name="curPlayer"></param>
        /// <param name="numOfPlayers"></param>
        /// <param name="playersAccumulatedTotalPoints"></param>
        /// <returns>int.MaxValue - if all players have achieved target, otherwise next player id</returns>
        private int SelectNextPlayer(int curPlayer, int curDiceVal, int numOfPlayers)
        {
            if (_PlayersAccumulatedTotalPoints.Count == numOfPlayers)
                return int.MaxValue;

            if (curDiceVal == 6 && !_PlayersAccumulatedTotalPoints.ContainsKey(curPlayer))
            {
                Console.WriteLine($"\n\t\t\t****  [Player-{curPlayer}] got extra chance since they got 6 in previous move   ****");
                return curPlayer;
            }

            int nextPlayer = GetNextPlayerInRoundRobin(curPlayer, numOfPlayers);
            while (_PlayersAccumulatedTotalPoints.ContainsKey(nextPlayer) || _PlayerConsecutiveOnesCount[nextPlayer] >= 2)
            {
                nextPlayer = GetNextPlayerInRoundRobin(nextPlayer, numOfPlayers);
                if (_PlayerConsecutiveOnesCount[nextPlayer] >= 2 && (numOfPlayers - _PlayersAccumulatedTotalPoints.Count > 1)) // Penalty of 1 round for 2 consecutive ones only if there are more than 1 remaining players
                {
                    Console.WriteLine($"\n\t\t\t**** Skipped turn of [Player-{curPlayer}] as Penalty, because [Player-{curPlayer}] got 2 consecutive ones.   ****");
                    _PlayerConsecutiveOnesCount[nextPlayer] = 0;
                    nextPlayer = GetNextPlayerInRoundRobin(nextPlayer, numOfPlayers);
                }
            }
            return nextPlayer;
        }

        private int GetNextPlayerInRoundRobin(int player, int numOfPlayers) => player == numOfPlayers ? 1 : player + 1;

        private void LogCurrentRoundDetails(int round, int curPlayer, int diceVal, int startRank)
        {
            Console.WriteLine();
            // Console.WriteLine($"Turn := Player-{curPlayer}");
            Console.WriteLine($"Value on Dice := {diceVal}");
            Console.WriteLine($"[Player-{curPlayer} Score] := {_PlayerScoresArr[curPlayer]} ");
            Console.WriteLine();

            var RemainingPlayersRank = CalculateRankOfRemainingPlayers(startRank);

            Console.WriteLine("\t Player \t Score \t\t Rank");
            for (int i = 1; i <= _NumOfPlayers; i++)
            {
                var found = _PlayersAccumulatedTotalPoints.TryGetValue(i, out int rank);
                if (!found)
                {
                    rank = RemainingPlayersRank[i];
                }
                Console.WriteLine($"\t {i} \t\t {_PlayerScoresArr[i]} \t\t {rank}");
            }
        }

        private Dictionary<int, int> CalculateRankOfRemainingPlayers(int startRank)
        {
            var playerDict = new Dictionary<int, int>();
            if (_PlayersAccumulatedTotalPoints.Count == _NumOfPlayers)
                return playerDict;

            var RemPlayers = new List<int>();

            for (int i = 1; i <= _NumOfPlayers; i++)
            {
                var found = _PlayersAccumulatedTotalPoints.TryGetValue(i, out int rank);
                if (!found)
                {
                    RemPlayers.Add(i);
                }
            }
            var players = RemPlayers.Select(id => new Player { Id = id, Score = _PlayerScoresArr[id] })
                            .OrderByDescending(p => p.Score)
                            .ToList();

            int idx = 0, prevValue = -1, newRank = 0;
            foreach (var p in players)
            {
                newRank = startRank + (prevValue == -1 || prevValue == p.Score ? idx : idx++);
                playerDict.Add(p.Id, newRank);
                prevValue = p.Score;
            }

            return playerDict;
        }

        #endregion
    }
}