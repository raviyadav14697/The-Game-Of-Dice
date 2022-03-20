namespace TheGameOfDice
{
    public class InvalidInputException : ExceptionBase
    {
        public InvalidInputException() : base("Provided invalid input. Please check!") { }
        public InvalidInputException(string message) : base(message) { }
    }

}