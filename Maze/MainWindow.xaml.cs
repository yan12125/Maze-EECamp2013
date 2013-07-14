using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
// using alias
// Reference: http://blog.miniasp.com/post.aspx?id=27b3b6ab-f104-4386-b2a1-cc448d04d1c3
using Timer = System.Windows.Forms.Timer;

namespace Maze
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected Timer timer = new Timer();
        protected DateTime tStart;
        protected static int nRounds = 6;
        // jagged array
        // http://stackoverflow.com/questions/9590144
        protected int[][,] maps = new int[nRounds][,];
        protected int mapIndex = 0;
        // fading in and fading out 
        // Reference: http://stackoverflow.com/questions/4317875
        protected DoubleAnimation changeScene = new DoubleAnimation
        {
            FillBehavior = FillBehavior.Stop,
            BeginTime = TimeSpan.FromSeconds(0),
            Duration = new Duration(TimeSpan.FromSeconds(1))
        };
        protected Storyboard storyboard = new Storyboard();
        protected bool changingScene = false;

        public MainWindow()
        {
            InitializeComponent();
            generateMaps();
            // for changing scene animation
            storyboard.Children.Add(changeScene);
            Storyboard.SetTarget(changeScene, Map);
            Storyboard.SetTargetProperty(changeScene, new PropertyPath(OpacityProperty));
            storyboard.Completed += new EventHandler(Animation_Completed);
            timer.Interval = 50;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            nextMap();
            tStart = DateTime.Now;
            Map.PlayerArrived += new EventHandler(Map_PlayerArrived);
        }

        protected void generateMaps()
        {
            int w = Map.Maze.ColumnDefinitions.Count;
            int h = Map.Maze.RowDefinitions.Count;
            for (int i = 0; i < maps.Length; i++)
            {
                maps[i] = new int[w, h];
                MazeGen gen = new MazeGen(w, h);
                do
                {
                    maps[i] = gen.generate(0, 0);
                } while (maps[i][w - 1, h - 1] != 0);
            }
        }

        protected void Map_PlayerArrived(object sender, EventArgs e)
        {
            if (changingScene)
            {
                return;
            }
            if (mapIndex == nRounds)
            {
                timer.Stop();
                MessageBox.Show("Win!");
                return;
            }
            changingScene = true;
            changeScene.From = 1.0;
            changeScene.To = 0.0;
            storyboard.Begin();
        }

        protected void nextMap()
        {
            Map.SetMap(maps[mapIndex]);
            MazeName.Content = "Maze #" + (mapIndex + 1);
            mapIndex++;
            Map.Visibility = Visibility.Visible;
            changeScene.From = 0.0;
            changeScene.To = 1.0;
            Map.Focus();
            storyboard.Begin();
        }

        protected void Animation_Completed(object sender, EventArgs e)
        {
            if (changingScene)
            {
                Map.Visibility = Visibility.Hidden;
                changingScene = false;
                nextMap();
            }
        }

        protected void timer_Tick(object sender, EventArgs e)
        {
            Map.timer_Tick(sender, e);
            updateElapsedTime();
        }

        protected void updateElapsedTime()
        {
            TimeSpan elaplsedTime = DateTime.Now.Subtract(tStart);
            elapsed_time.Content = elaplsedTime.ToString(@"mm\:ss");
        }
    }
}
