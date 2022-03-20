namespace TheGameOfDice.Entity
{
    public class Player
    {
        public int Id { get; set; }
        public string Name => $"Player-{Id}";
        public int Score { get; set; }
    }
}