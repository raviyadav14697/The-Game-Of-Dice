using System;
using System.Collections.Generic;
using System.Linq;
using TheGameOfDice.BusinessLogic.Interfaces;

namespace TheGameOfDice.BusinessLogic
{
    public class DiceGame : IDiceGame
    {
        #region Private Members - Game State Variables

        private HashSet<int> _PlayersAccumulatedTotalPoints;
        private int[] _PlayerScoresArr;

        #endregion

        public void StartGame()
        {
            (int numOfPlayers, int pointsToAccumulate) = ReadAndValidateInput();
            InitiateGame(numOfPlayers);

            int round = 1, curPlayer = GetRandomInRange(1, numOfPlayers), prevDiceVal = 0;

            while (_PlayersAccumulatedTotalPoints.Count != numOfPlayers)
            {
                Console.WriteLine($"\n\t\t\t****  Round := {round}, curPlayer := {curPlayer}  ****");

                int diceVal = RollDice(curPlayer);
                _PlayerScoresArr[curPlayer] += diceVal;

                LogCurrentRoundDetails(round, curPlayer, diceVal);

                if (_PlayerScoresArr[curPlayer] >= pointsToAccumulate)
                {
                    _PlayersAccumulatedTotalPoints.Add(curPlayer);
                    Console.WriteLine($"****  [Player-{curPlayer}] accumulated total points  ****");
                }

                int nextPlayer = SelectNextPlayer(curPlayer, diceVal, numOfPlayers, _PlayersAccumulatedTotalPoints);
                curPlayer = nextPlayer;
                prevDiceVal = diceVal;
                round++;
            }

            Console.WriteLine("\t\t\t****  All the players accumulated total points!  ****");
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
            _PlayersAccumulatedTotalPoints = new();
            _PlayerScoresArr = new int[numOfPlayers + 1];
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
        private int SelectNextPlayer(int curPlayer, int curDiceVal, int numOfPlayers, HashSet<int> playersAccumulatedTotalPoints)
        {
            if (playersAccumulatedTotalPoints.Count == numOfPlayers)
                return int.MaxValue;

            if (curDiceVal == 6 && !playersAccumulatedTotalPoints.Contains(curPlayer))
            {
                Console.WriteLine($"\n\t\t\t****  [Player-{curPlayer}] got extra chance since they got 6 in previous move   ****");
                return curPlayer;
            }

            int nextPlayer = curPlayer == numOfPlayers ? 1 : curPlayer + 1;
            while (playersAccumulatedTotalPoints.Contains(nextPlayer))
                nextPlayer = nextPlayer == numOfPlayers ? 1 : nextPlayer + 1;
            return nextPlayer;
        }

        private void LogCurrentRoundDetails(int round, int curPlayer, int diceVal)
        {
            Console.WriteLine();
            Console.WriteLine($"Turn := Player-{curPlayer}");
            Console.WriteLine($"Value on Dice := {diceVal}");
            Console.WriteLine($"Player {curPlayer} Score] := {_PlayerScoresArr[curPlayer]} ");
            Console.WriteLine();
        }

        #endregion
    }
}