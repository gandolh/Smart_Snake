namespace SmartSnake.Agents
{
    internal class HamiltonianAgent : IAgent
    {
        private int snakeSpeed = 1;

        public GameState gameState { get; }
        public Direction[,] directions { get; }

        public int rows = 0;
        public int cols = 0;

        public HamiltonianAgent(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            this.gameState = new GameState(rows, cols);
            HamiltonianCycleGenerator hamiltonianGenerator = new HamiltonianCycleGenerator(16,16);
            directions = hamiltonianGenerator.GenerateHamiltonianCycle();
        }

        public Direction GetPrediction()
        {
            Position headPos = gameState.HeadPosition();
            return directions[headPos.Row, headPos.Col];

        }

        public async Task MakeMove()
        {
            if(gameState.SnakePositions().Count() == rows * cols - 10 ) { snakeSpeed = 100; }
            gameState.ChangeDirection(GetPrediction());
            await Task.Delay(snakeSpeed);
            gameState.Move();
        }
    }
}
