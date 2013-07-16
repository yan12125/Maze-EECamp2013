using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using PointXY = System.Tuple<int, int>;

namespace Maze
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        private List<Border> _wall = new List<Border>();
        protected Border lastGrid = new Border();
        protected Dictionary<Key, bool> keyPressed = new Dictionary<Key,bool>();
        protected List<Key> pressedSeq = new List<Key>();
        public EventHandler PlayerArrived;
        protected bool cheating = false;

        public Map()
        {
            InitializeComponent();
            AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);
            AddHandler(Keyboard.KeyUpEvent, (KeyEventHandler)HandleKeyUpEvent);
            keyPressed.Add(Key.Up, false);
            keyPressed.Add(Key.Down, false);
            keyPressed.Add(Key.Left, false);
            keyPressed.Add(Key.Right, false);
            // last grid for checking arrival
            Grid.SetRow(lastGrid, Maze.RowDefinitions.Count - 1);
            Grid.SetColumn(lastGrid, Maze.ColumnDefinitions.Count - 1);
            lastGrid.Background = Brushes.Transparent;
            lastGrid.Width = lastGrid.Height = Double.NaN;
            Maze.Children.Add(lastGrid);
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            MovePlayer();
            updateClip();
        }

        /*
         * Key/Move algorithm by concise
         */
        private void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F9) // for debugging
            {
                toggleCheating();
            }
            if (e.IsRepeat)
            {
                return;
            }
            Key tKey = e.Key;

            if (!keyPressed.ContainsKey(tKey))
            {
                return;
            }

            int ti = pressedSeq.FindIndex(delegate (Key k){
                return k == tKey;
            });
            if (ti == -1)
            {
                if (pressedSeq.Count != 0)
                {
                    keyPressed[pressedSeq[pressedSeq.Count - 1]] = false;
                }
                pressedSeq.Add(tKey);
                keyPressed[tKey] = true;
            }
        }

        private void HandleKeyUpEvent(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat)
            {
                return;
            }
            Key tKey = e.Key;
            if (!keyPressed.ContainsKey(tKey))
            {
                return;
            }
            int ti = pressedSeq.FindIndex(delegate(Key k)
            {
                return k == tKey;
            });
            if (ti != -1)
            {
                pressedSeq.RemoveAt(ti);
            }
            keyPressed[tKey] = false;
            if (pressedSeq.Count != 0)
            {
                keyPressed[pressedSeq[pressedSeq.Count - 1]] = true;
            }
        }

        protected void MovePlayer()
        {
            var temp_Player_X = PlayerMove.X;
            var temp_Player_Y = PlayerMove.Y;
            
            if(keyPressed[Key.Up])
            {
                if (PlayerMove.Y > 0 && PlayerMove.Y <= 600 - Player.Height)
                {
                    PlayerMove.Y -= Player.Speed;
                }
            }
            else if(keyPressed[Key.Down])
            {
                if (PlayerMove.Y >= 0 && PlayerMove.Y < 600 - Player.Height)
                {
                    PlayerMove.Y += Player.Speed;
                }
            }
            else if(keyPressed[Key.Left])
            {
                if (PlayerMove.X > 0 && PlayerMove.X <= 800 - Player.Width)
                {
                    PlayerMove.X -= Player.Speed;
                }
            }
            else if(keyPressed[Key.Right])
            {
                if (PlayerMove.X >= 0 && PlayerMove.X < 800 - Player.Width)
                {
                    PlayerMove.X += Player.Speed;
                }
            }
            foreach (var item in _wall)
            {
                if (InArea(PlayerBorder, item))
                {
                    PlayerMove.X = temp_Player_X;
                    PlayerMove.Y = temp_Player_Y;
                    return;
                }
            }
            checkArrive();
        }

        protected void checkArrive()
        {
            if (InArea(PlayerBorder, lastGrid))
            {
                PlayerArrived.Invoke(this, new EventArgs());
            }
        }

        public void SetMap(int[,] maze)
        {
            int w = Maze.ColumnDefinitions.Count;
            int h = Maze.RowDefinitions.Count;
            PlayerMove.X = PlayerMove.Y = 0;
            // first two children are PlayerBorder and lastGrid
            Maze.Children.RemoveRange(2, Maze.Children.Count - 2);
            _wall.Clear();
            var colors = new Dictionary<int, Brush>()
            {
                { 1, Brushes.Blue }, 
                { 2, Brushes.Yellow }, 
                { 3, Brushes.Red }, 
                { 4, Brushes.Aquamarine }, 
                { 5, Brushes.Purple }, 
                { 6, Brushes.Blue }
            };
            var pics = new Dictionary<int, string>()
            {
                { 0, "floor.png" }, 
                { 7, "exit.jpg" }
            };
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    int propId = maze[i, j];
                    Border rect = new Border();
                    if(colors.ContainsKey(propId))
                    {
                        rect.Background = colors[propId];
                    }
                    else if (pics.ContainsKey(propId))
                    {
                        rect.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Image/" + pics[propId])));
                    }
                    else
                    {
                        rect.Background = Brushes.Transparent;
                    }
                    Grid.SetRow(rect, j);
                    Grid.SetColumn(rect, i);
                    rect.Height = Double.NaN;
                    rect.Width = Double.NaN;
                    Panel.SetZIndex(rect, 2);
                    Maze.Children.Add(rect);
                    if (propId == 1)
                    {
                        _wall.Add(rect);
                    }
                }
            }
            updateClip();
        }

        private bool InArea(Border player, Border wall)
        {
            Rect h = new Rect();
            Rect b = new Rect();
            h.Location = player.PointToScreen(new Point(0, 0));
            h.Height = player.ActualHeight;
            h.Width = player.ActualWidth;
            b.Location = wall.PointToScreen(new Point(0, 0));
            b.Height = wall.ActualHeight;
            b.Width = wall.ActualWidth;
            if (h.IntersectsWith(b))
                return true;
            return false;
        }
        protected void updateClip()
        {
            Point center = new Point(PlayerMove.X + Player.Width / 2, PlayerMove.Y + Player.Height / 2);
            double r = Player.View;
            if (Maze.Clip == null)
            {
                Maze.Clip = new EllipseGeometry()
                {
                    Center = center,
                    RadiusX = r,
                    RadiusY = r
                };
            }
            else
            {
                EllipseGeometry e = Maze.Clip as EllipseGeometry;
                e.Center = center;
                e.RadiusX = e.RadiusY = r;
            }
        }

        public void toggleCheating()
        {
            cheating = !cheating;
            Player.toggleCheating(cheating);
        }
    }
}
