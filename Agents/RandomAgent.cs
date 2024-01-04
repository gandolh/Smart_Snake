namespace SmartSnake.Agents
{
    internal class RandomAgent : IAgent
    {
        public GameState gameState { get; }

        private Direction[] directions;
        private Random random;

        public RandomAgent(int rows, int cols)
        {
            this.gameState = new GameState(rows, cols);
            directions = [Direction.Up, Direction.Right, Direction.Down, Direction.Left];
            random = new Random();
        }

        public Direction GetPrediction()
        {
            return directions[random.Next(directions.Length)]; ;
        }

        public async Task MakeMove()
        {
            gameState.ChangeDirection(GetPrediction());
            await Task.Delay(100);
            gameState.Move();
        }
    }
}
