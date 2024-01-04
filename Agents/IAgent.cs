namespace SmartSnake.Agents
{
    public interface IAgent
    {
        GameState gameState { get; }

        Direction GetPrediction();
        Task MakeMove();
    }
}