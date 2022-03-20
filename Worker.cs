using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TheGameOfDice.BusinessLogic.Interfaces;

namespace TheGameOfDice
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDiceGame _diceGameBL;

        public Worker(ILogger<Worker> logger, IDiceGame diceGameBL)
        {
            _logger = logger;
            _diceGameBL = diceGameBL;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine("\n\n\t\t\t################### Welcome to The Game of Dice ###################");
                    _diceGameBL.StartGame();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString(), args: ex);
                    // Console.WriteLine(ex.Message);
                }
                finally
                {
                    // wait for 1 sec to start new game.
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}
