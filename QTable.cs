using System.Text.Json;

namespace SmartSnake
{
    internal class QTable
    {
        private Dictionary<SnakeState, double[]> qTable;

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

        internal string[] ToCustomFormat()
        {
            string[] lines = new string[qTable.Count];
            var keys = qTable.Keys.ToArray();
            for(int i = 0; i < lines.Length; i++)
            {
                var value = qTable[keys[i]];
                lines[i] = $"{JsonSerializer.Serialize(keys[i])};{value[0]};" +
                    $"{value[1]};{value[2]};{value[3]}";
            }
            return lines;
        }

        internal void FromCustomFormat(string[] lines)
        {
            qTable.Clear();
            foreach(var line in lines)
            {
                string[] splitArray = line.Split(';');
                SnakeState key = JsonSerializer.Deserialize<SnakeState>(splitArray[0]) 
                    ?? throw new Exception("Deserialize exception");
                double value0 = double.Parse(splitArray[1]);
                double value1 = double.Parse(splitArray[2]);
                double value2 = double.Parse(splitArray[3]);
                double value3 = double.Parse(splitArray[4]);

                qTable.Add(key, [value0,value1,value2,value3]);
            }
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
