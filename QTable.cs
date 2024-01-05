using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSnake
{
    internal class QTable
    {
        private readonly Dictionary<SnakeState, double[]> qTable;

        public QTable()
        {
            qTable = new();
        }

        private double GetValue(SnakeState state, int action)
        {
            if(qTable.ContainsKey(state) == false) {
                qTable[state] = [0, 0, 0, 0];
            }
            return qTable[state][action];
        }

        public double this[SnakeState state, int action]
        {
            get {
                return GetValue(state,action);
            }
            set {
                qTable[state][action] = value;
            }
        }
    }
}
