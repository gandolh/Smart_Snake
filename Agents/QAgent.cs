using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.RightsManagement;
using System.Threading.Tasks;

namespace SmartSnake.Agents
{
    internal class QAgent : IAgent
    {

        public int SnakeSpeed = 0;
        private readonly int cols;
        private readonly int rows;

        // 4 actions: up, right, down, left
        private readonly QTable qTable;
        private readonly double learningRate = 0.01; 
        private readonly double discountFactor = 0.95; 
        private double Eps = 1.0;
        private double EpsDiscount = 0.9992;
        private double MinEps = 0.001;
        private readonly Direction[] directions;
        private static Random random = new Random();

        private int movesWithoutFood = 0;

        public GameState gameState { get; private set; }


        public QAgent(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            gameState = new GameState(rows, cols);
            directions = [Direction.Up, Direction.Right, Direction.Down, Direction.Left];

            // Initialize Q-table 
            qTable = new();
            //UpdateEps();
        }

   


        private Direction EpsilonGreedyAction(SnakeState state)
        {
            if (random.NextDouble() < Eps)
            {
                return directions[random.Next(directions.Length)];
            }
            return GetBestAction(state);
        }

        private Direction GetBestAction(SnakeState state)
        {
            double maxQValue = double.MinValue;
            int bestAction = 0;

            // Find the action with the highest Q-value for the given state
            for (int action = 0; action < 4; action++)
            {
                if (qTable[state,action] > maxQValue)
                {
                    maxQValue = qTable[state,action];
                    bestAction = action;
                }
            }

            return directions[bestAction];
        }

        public async Task MakeMove()
        {
            await Task.Delay(SnakeSpeed);
            UpdateQValues();
        }

        private void UpdateQValues()
        {
            movesWithoutFood++;
            UpdateEps();
            if (movesWithoutFood > 1000)
            {
                gameState.GameOver = true;
                return;
            }
            var currentState = GetState(gameState);
            var action = EpsilonGreedyAction(currentState);
            var actionIndex = GetIndexForDir(action);
            var reward = GetReward(gameState, action);

            gameState.ChangeDirection(action);
            gameState.Move();
            var newState = GetState(gameState);

            double maxNextQValue = double.MinValue;


            for (int nextAction = 0; nextAction < 4; nextAction++)
            {
                if (qTable[newState,nextAction] > maxNextQValue)
                {
                    maxNextQValue = qTable[newState,nextAction];   
                }
            }

            // bellman equation
            qTable[currentState, actionIndex] = (1 - learningRate) 
                    * qTable[currentState, actionIndex] + learningRate
                    *(reward + discountFactor * maxNextQValue);
        }

        public double GetReward(GameState gameState, Direction action)
        {
            double reward = 0;
            GridValue gridValue = gameState.GetGridValue(gameState.HeadPosition().Translate(action));
            if (gridValue == GridValue.Food)
                movesWithoutFood = 0;


            reward += gridValue switch
            {
                GridValue.Snake => -10,
                GridValue.Outside => -10,
                GridValue.Food => 100,
                GridValue.Empty => -1,
                _ => -1
            };

            return reward;

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

        private SnakeState GetState(GameState gameState)
        {
            #region new state calculation
            Position headPosition = gameState.HeadPosition();
            Position foodPosition = gameState.FoodPosition;

            SnakeState state = new SnakeState()
            {
                dirIsLeft = gameState.Dir == Direction.Left,
                dirIsRight = gameState.Dir == Direction.Right,
                dirIsUp = gameState.Dir == Direction.Up,
                dirIsDown = gameState.Dir == Direction.Down,
                foodIsUp = foodPosition.Row < headPosition.Row,
                foodIsRight = foodPosition.Col > headPosition.Col,
                foodIsDown = foodPosition.Row > headPosition.Row,
                foodIsLeft = foodPosition.Col < headPosition.Col,
                dangerIsLeft = IsDanger(gameState.GetGridValue(headPosition.Translate(Direction.Left))),
                dangerIsRight = IsDanger(gameState.GetGridValue(headPosition.Translate(Direction.Right))),
                dangerIsUp = IsDanger(gameState.GetGridValue(headPosition.Translate(Direction.Up))),
                dangerIsDown = IsDanger(gameState.GetGridValue(headPosition.Translate(Direction.Down)))
            };
            #endregion
            return state;
        }

        private bool IsDanger(GridValue gridValue)
        {
            return gridValue == GridValue.Snake || gridValue == GridValue.Outside;
        }

        internal void ResetGameState()
        {
            gameState = new GameState(rows, cols);
            movesWithoutFood = 0;
            //UpdateEps();
        }

        internal void Save()
        {
            SaveModule.WriteQTable($"{DateTime.UtcNow.ToString("dd-MM-yyyy_hh-mm")}.txt", qTable);
        }

        internal void Load()
        {
            string modelName = "05-01-2024_08-50.txt";
            SaveModule.ReadQTable(modelName, qTable);
        }

        public Direction GetPrediction()
        {
            throw new NotImplementedException();
        }

        private void UpdateEps()
        {
            Eps = Math.Max(Eps * EpsDiscount, MinEps);
        }
    }
}
