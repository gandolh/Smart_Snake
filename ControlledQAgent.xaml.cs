using SmartSnake.Agents;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmartSnake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ControlledQAgent : Window
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

        private const int MillisecondsDelay1X = 100;
        private const int MillisecondsDelay10X = 1;


        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;

        private QAgent qAgent;
        private int maxScore;
        private long maxScoreEpoch;
        private bool DrawEnabled = true;

        public ControlledQAgent()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            qAgent = new QAgent(rows, cols);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await RunGame();
        }


        private async Task RunGame()
        {
            Overlay.Visibility = Visibility.Hidden;
            qAgent.SnakeSpeed = MillisecondsDelay10X;
            long epoch = 0;
            while (true)
            {
                EppochText.Text = $"Epoch: {epoch++}";
                await GameLoop();
                if(qAgent.gameState.Score > maxScore)
                {
                    maxScore = qAgent.gameState.Score;
                    maxScoreEpoch = epoch - 1;
                    MaxScoreText.Text = $"Max Score: {maxScore}";
                    MaxScoreEpochText.Text = $"Max Score Epoch: {maxScoreEpoch}";
                }
                
                qAgent.ResetGameState();
            }
        }

        private async Task GameLoop()
        {
            while (!qAgent.gameState.GameOver)
            {
                await qAgent.MakeMove();
                if (DrawEnabled)
                {
                    ScoreText.Text = $"Score: {qAgent.gameState.Score}";
                    Draw();
                }
            }
        }

        private Image[,] SetupGrid()
        {
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
                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            return images;
        }
        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
        }
        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridVal = qAgent.gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity;

                }
        }
        private void DrawSnakeHead()
        {
            Position headPos = qAgent.gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;

            int rotation = dirToRotation[qAgent.gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }
        private void SpeedButton_Click(object sender, RoutedEventArgs e)
        {
            if(qAgent.SnakeSpeed == MillisecondsDelay1X)
            {
                qAgent.SnakeSpeed = MillisecondsDelay10X;
                SpeedButton.Content = "10x Speed";
            }
            else
            {
                qAgent.SnakeSpeed = MillisecondsDelay1X;
                SpeedButton.Content = "1x Speed";
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            qAgent.Save();
        }
     
    }
}