﻿using Microsoft.VisualBasic;
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
        private double Eps = 0.1;
        private readonly Direction[] directions;
        private static Random random = new Random();

        public GameState gameState { get; private set; }


        public QAgent(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            gameState = new GameState(rows, cols);
            directions = [Direction.Up, Direction.Right, Direction.Down, Direction.Left];

            // Initialize Q-table with default values (e.g., zeros)
            qTable = new();
           
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

            qTable[currentState, actionIndex] = (1 - learningRate) 
                    * qTable[currentState, actionIndex] + learningRate
                    *(reward + discountFactor * maxNextQValue);
        }

        public int GetReward(GameState gameState, Direction action)
        {
            GridValue gridValue = gameState.GetGridValue(gameState.HeadPosition().Translate(action));
            return gridValue switch
            {
                GridValue.Snake => -10,
                GridValue.Outside => -10,
                GridValue.Food => 100,
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

        private SnakeState GetState(GameState gameState)
        {
            #region old state calculation
            //Position headPosition = gameState.HeadPosition();
            //Position foodPosition = gameState.FoodPosition;
            //int headState = headPosition.Row * cols + headPosition.Col;
            //int foodState = foodPosition.Row * cols + foodPosition.Col;
            //int dirState = GetIndexForDir(gameState.Dir);
            //double distHeadFood = headPosition.DistanceTo(foodPosition);
            //// booleans if foodPosition is up, right, down or left to headPosition.
            //bool foodIsUp = foodPosition.Row < headPosition.Row;
            //bool foodIsRight = foodPosition.Col > headPosition.Col;
            //bool foodIsDown = foodPosition.Row > headPosition.Row;
            //bool foodIsLeft = foodPosition.Col < headPosition.Col;

            //int hashCodeEnvironment = 5;
            //HashSet<Position> neighbours = GetNeighbours(headPosition, 5);
            //foreach (Position position in neighbours)
            //{
            //    hashCodeEnvironment = HashCode.Combine(hashCodeEnvironment, position.GetHashCode());
            //}

            //int resultHashCode = 12;
            //resultHashCode = HashCode.Combine(resultHashCode, headState);
            //resultHashCode = HashCode.Combine(resultHashCode, foodState);
            //resultHashCode = HashCode.Combine(resultHashCode, dirState);
            //resultHashCode = HashCode.Combine(resultHashCode, distHeadFood);
            //resultHashCode = HashCode.Combine(resultHashCode, foodIsUp);
            //resultHashCode = HashCode.Combine(resultHashCode, foodIsRight);
            //resultHashCode = HashCode.Combine(resultHashCode, foodIsDown);
            //resultHashCode = HashCode.Combine(resultHashCode, foodIsLeft);
            //resultHashCode = HashCode.Combine(resultHashCode, hashCodeEnvironment);
            #endregion

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
            return state;
        }

        private bool IsDanger(GridValue gridValue)
        {
            return gridValue == GridValue.Snake || gridValue == GridValue.Outside;
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

        internal void Save()
        {
            throw new NotImplementedException();
        }

        public Direction GetPrediction()
        {
            throw new NotImplementedException();
        }

    }
}
