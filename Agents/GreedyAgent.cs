

namespace SmartSnake.Agents
{
    public class GreedyAgent : IAgent
    {
        public GameState gameState { get; }

        private Direction[] directions;

        public GreedyAgent(int rows, int cols)
        {
            this.gameState = new GameState(rows, cols);
            directions = [Direction.Up, Direction.Right, Direction.Down, Direction.Left];

        }

        public Direction GetPrediction()
        {
            Position foodPosition = gameState.FoodPosition;
            Position headPosition = gameState.HeadPosition();
            double minDistance = double.MaxValue;
            Direction futureDirection = Direction.Left;

            for(int i = 0; i< directions.Length;i++) { 
                Position newPosition = headPosition.Translate(directions[i]);
                double distance = newPosition.DistanceTo(foodPosition);
                if (distance < minDistance && IsValidMove(newPosition, directions[i]) )
                {
                    minDistance = distance;
                    futureDirection = directions[i];
                }
            }
            return futureDirection;
        }

        private bool IsValidMove(Position newPos, Direction dir)
        {
            GridValue gridValue = gameState.GetGridValue(newPos);
            return dir != gameState.Dir.Opposite() && gridValue != GridValue.Outside && gridValue != GridValue.Snake;
        }

        public async Task MakeMove()
        {
            gameState.ChangeDirection(GetPrediction());
            await Task.Delay(100);
            gameState.Move();
        }
    }
}
