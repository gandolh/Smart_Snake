using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SmartSnake.Agents
{
    internal class QAgent : IAgent
    {
        private const int MaxPossibleDistinctStates = 1_000_000;
        private const int MillisecondsDelay = 10;
        private readonly int cols;
        private readonly int rows;
        private readonly double[,] QTable;
        private readonly double learningRate = 0.1; // Learning rate
        private readonly double discountFactor = 0.9; // Discount factor
        private readonly Direction[] directions;

        public GameState gameState { get; private set; }


        public QAgent(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            gameState = new GameState(rows, cols);
            directions = [Direction.Up, Direction.Right, Direction.Down, Direction.Left];

            // Initialize Q-table with default values (e.g., zeros)
            QTable = new double[MaxPossibleDistinctStates, 4]; // 4 actions: up, right, down, left
        }

        public Direction GetPrediction()
        {
            // Perform epsilon-greedy action selection
            Direction action = EpsilonGreedyAction(GetState(gameState));

            return action;
        }



        private Direction EpsilonGreedyAction(int state)
        {
            Random random = new Random();
            double epsilon = 0.1; // Exploration rate (probability of exploration)

            if (random.NextDouble() < epsilon)
            {
                // Exploration: Choose a random action
                return GetRandomAction();
            }
            else
            {
                // Exploitation: Choose the best action based on Q-values
                return GetBestAction(state);
            }
        }

        private Direction GetRandomAction()
        {
            Random random = new Random();
            return directions[random.Next(directions.Length)];
        }

        private Direction GetBestAction(int state)
        {
            double maxQValue = double.MinValue;
            int bestAction = 0;

            // Find the action with the highest Q-value for the given state
            for (int action = 0; action < 4; action++)
            {
                if (QTable[state, action] > maxQValue)
                {
                    maxQValue = QTable[state, action];
                    bestAction = action;
                }
            }

            return directions[bestAction];
        }

        public async Task MakeMove()
        {
            Direction predictedAction = GetPrediction();
            gameState.ChangeDirection(predictedAction);
            await Task.Delay(MillisecondsDelay);
            gameState.Move();

            // Perform Q-learning update after the move
            UpdateQValues();
        }

        private void UpdateQValues()
        {
            var tmpGameState = new GameState(gameState);
            tmpGameState.Move();

            int currentState = GetState(gameState);
            int action = GetIndexForDir(gameState.Dir);
            int newState = GetState(tmpGameState);
            double reward = GetReward(tmpGameState);
            bool done = tmpGameState.GameOver;

            double currentQValue = QTable[currentState, action];

            double maxNextQValue = double.MinValue;
            for (int nextAction = 0; nextAction < 4; nextAction++)
            {
                if (QTable[newState, nextAction] > maxNextQValue)
                {
                    maxNextQValue = QTable[newState, nextAction];
                }
            }

            // Bellman equation update for Q-value
            // or (1 - learningRate ) * currentQValue + learningRate* (reward + discountFactor*maxNextQValue);
            double updatedQValue = currentQValue + learningRate * (reward
                + (done ? 0 : discountFactor * maxNextQValue) - currentQValue);

            QTable[currentState, action] = updatedQValue;
        }

        public int GetReward(GameState gameState)
        {
            GridValue gridValue = gameState.GetWhatWillHit(gameState.HeadPosition());
            return gridValue switch
            {
                GridValue.Snake => -10,
                GridValue.Outside => -10,
                GridValue.Food => 20,
                GridValue.Empty => -1,
                _ => -1
            };
        }

        private int GetIndexForDir(Direction dir)
        {
            // could be a for but this is faster
            if (dir == directions[0]) return 0;
            if (dir == directions[1]) return 1;
            if (dir == directions[2]) return 2;
            if (dir == directions[3]) return 3;

            throw new NotImplementedException();
        }

        private int GetState(GameState gameState)
        {
            Position headPosition = gameState.HeadPosition();
            Position foodPosition = gameState.FoodPosition;
            int headState = headPosition.Row * cols + headPosition.Col;
            int foodState = foodPosition.Row * cols + foodPosition.Col;
            int dirState = GetIndexForDir(gameState.Dir);
            double distHeadFood = headPosition.DistanceTo(foodPosition);
            // booleans if foodPosition is up, right, down or left to headPosition.
            bool foodIsUp = foodPosition.Row < headPosition.Row;
            bool foodIsRight = foodPosition.Col > headPosition.Col;
            bool foodIsDown = foodPosition.Row > headPosition.Row;
            bool foodIsLeft = foodPosition.Col < headPosition.Col;

            int hashCodeEnvironment = 5;
            HashSet<Position> neighbours = GetNeighbours(headPosition, 3);
            foreach (Position position in neighbours)
            {
                hashCodeEnvironment = HashCode.Combine(hashCodeEnvironment, position.GetHashCode());
            }

            int resultHashCode = 12;
            resultHashCode = HashCode.Combine(resultHashCode, headState);
            resultHashCode = HashCode.Combine(resultHashCode, foodState);
            resultHashCode = HashCode.Combine(resultHashCode, dirState);
            resultHashCode = HashCode.Combine(resultHashCode, distHeadFood);
            resultHashCode = HashCode.Combine(resultHashCode, foodIsUp);
            resultHashCode = HashCode.Combine(resultHashCode, foodIsRight);
            resultHashCode = HashCode.Combine(resultHashCode, foodIsDown);
            resultHashCode = HashCode.Combine(resultHashCode, foodIsLeft);
            resultHashCode = HashCode.Combine(resultHashCode, hashCodeEnvironment);


            // map the value
            double scaledValue = (1D * resultHashCode - int.MinValue) / (1D * int.MaxValue - int.MinValue) * MaxPossibleDistinctStates;
            resultHashCode = (int)Math.Clamp(scaledValue, 0, MaxPossibleDistinctStates);
            return resultHashCode;
        }

        private HashSet<Position> GetNeighbours(Position headPosition, int depth)
        {
            HashSet<Position> neighborPositions = new HashSet<Position>();
            Queue<Position> queue = new Queue<Position>();
            queue.Enqueue(headPosition);
            for (int i = 0; i < depth; i++)
            {
                int count = queue.Count;
                for (int j = 0; j < count; j++)
                {
                    Position pos = queue.Dequeue();
                    neighborPositions.Add(pos);
                    for (int k = 0; k < directions.Length; k++)
                    {
                        Position newPos = pos.Translate(directions[k]);
                        if (!neighborPositions.Contains(newPos))
                            queue.Enqueue(newPos);
                    }
                }
            }
            return neighborPositions;
        }

        internal void ResetGameState()
        {
            gameState = new GameState(rows, cols);
        }
    }
}
