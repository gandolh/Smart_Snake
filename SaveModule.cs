using System.IO;
using System.Text.Json;

namespace SmartSnake
{
    internal static class SaveModule
    {
        private static readonly JsonSerializerOptions options;

        static SaveModule()
        {
            options = new JsonSerializerOptions();
        }

        private static string GetPathToModel(string modelName)
        {
            string inOutFolder = "Models";
            string startupPath = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
            return Path.Join(startupPath, inOutFolder, modelName);
        }

        internal static void WriteQTable(string modelName, QTable qTable)
        {
            string path = GetPathToModel(modelName);
            string[] lines = qTable.ToCustomFormat();
            File.WriteAllLines(path, lines);
        }

        internal static void ReadQTable(string modelName, QTable qTable)
        {
            string path = GetPathToModel(modelName);
            string[] lines = File.ReadAllLines(path);
            qTable.FromCustomFormat(lines);

        }
    }
}
