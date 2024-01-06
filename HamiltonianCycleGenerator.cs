using System;

namespace SmartSnake.Agents
{
    public class HamiltonianCycleGenerator
    {
        private Direction[,] cycle;
        private int rows;
        private int cols;

        public HamiltonianCycleGenerator(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            cycle = new Direction[rows, cols];
        }

        public Direction[,] GenerateHamiltonianCycle()
        {
            if (FindHamiltonianCycle(new Position(0, 0), 1))
            {
                //Print();
                return cycle;
            }
            else
            {
                Console.WriteLine("No Hamiltonian Cycle possible for the given matrix size.");
                return null;
            }
        }

        private bool FindHamiltonianCycle(Position pos, int index)
        {
            //Print();
            if (index == rows * cols + 1)
            {
                return true;
            }

            if (IsValidMove(pos.Row, pos.Col))
            {

                for (int i = 0; i < 4; i++)
                {

                    cycle[pos.Row, pos.Col] = Direction.ClockWiseDirection[i];
                    if (FindHamiltonianCycle(
                        pos.Translate(Direction.ClockWiseDirection[i]),
                        index + 1))
                    {
                        return true;
                    }
                }

                cycle[pos.Row, pos.Col] = null;
            }

            return false;
        }

        private bool IsValidMove(int row, int col)
        {
            return (row >= 0 && row < rows && col >= 0 && col < cols && cycle[row, col] == null);
        }

        private void Print()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (cycle[i, j] == null) Console.Write("#");
                    if (cycle[i, j] == Direction.Left) Console.Write("<");
                    if (cycle[i, j] == Direction.Right) Console.Write(">");
                    if (cycle[i, j] == Direction.Up) Console.Write("^");
                    if (cycle[i, j] == Direction.Down) Console.Write("v");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }
    }
}
