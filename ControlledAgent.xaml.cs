using SmartSnake.Agents;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmartSnake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ControlledAgent : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            {Direction.Up, 0 },
            {Direction.Right, 90 },
            {Direction.Down, 180 },
            {Direction.Left, 270 }, 
        };

        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;
        private bool gameRunning;

        private GreedyAgent greedyAgent;
        private RandomAgent randomAgent;
        //private QAgent qAgent;
        private IAgent selectedAgent;
        private readonly GameState gameState;

        public ControlledAgent()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            greedyAgent = new GreedyAgent(rows, cols);
            randomAgent = new RandomAgent(rows, cols);
            //qAgent = new QAgent(rows, cols);
            selectedAgent = greedyAgent;
            gameState = selectedAgent.gameState;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await ShowCountDown();
            gameRunning = true;
            await RunGame();
            gameRunning = false;
        }


        private async Task RunGame()
        {
            Draw();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await selectedAgent.MakeMove();
                Draw();
            }
        }

        private Image[,] SetupGrid() {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };
                    images[r,c] = image;
                    GameGrid.Children.Add(image);
                }
            return images;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity;

                }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());

            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(50);
            }
        }

        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "PRESS ANY KEY TO START";
        }
    }
}