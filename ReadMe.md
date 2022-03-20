# The Game Of Dice

The "Game of Dice" is a multiplayer game where N players roll a 6 faced dice in a round-robin
fashion. Each time a player rolls the dice their points increase by the number (1 to 6) achieved
by the roll.

As soon as a player accumulates M points they complete the game and are assigned a rank.
Remaining players continue to play the game till they accumulate at least M points. The game
ends when all players have accumulated at least M points.

## Rules of the game

- The order in which the users roll the dice is decided randomly at the start of the game.
- If a player rolls the value "6" then they immediately get another chance to roll again and move ahead in the game.
- If a player rolls the value "1" two consecutive times then they are forced to skip their next turn as a penalty.

----------

## How to Run the Project

### Run game using docker

1. Create a docker image using DockerFile included in project. Run the command - `docker build -t <insert-img-tag> -f Dockerfile .`
2. Run the contaoner using command - `docker run -it <insert-img-tag>`

### If you don't have Docker installed, follow this

1. Install .Net 5.0 sdk in your pc from [dotnet-5.0](https://dotnet.microsoft.com/en-us/download/dotnet/5.0)
2. Open the project root in terminal
3. Run the command `dotnet run`
