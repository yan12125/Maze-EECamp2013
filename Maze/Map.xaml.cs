using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Maze
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        private List<Border> _wall = new List<Border>();

        public Map()
        {
            InitializeComponent();
            AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);
            _wall.Clear();
            SetMap();
        }

        private void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            var temp_Player_X = PlayerMove.X;
            var temp_Player_Y = PlayerMove.Y;
            switch (e.Key)
            {
                case Key.Up:
                    {
                        if (PlayerMove.Y > 0 && PlayerMove.Y <= 600 - Player.Height)
                        {
                            PlayerMove.Y -= Player.Speed;
                        }
                        break;
                    }
                case Key.Down:
                    {
                        if (PlayerMove.Y >= 0 && PlayerMove.Y < 600 - Player.Height)
                        {
                            PlayerMove.Y += Player.Speed;
                        }
                        break;
                    }
                case Key.Left:
                    {
                        if (PlayerMove.X > 0 && PlayerMove.X <= 800 - Player.Width)
                        {
                            PlayerMove.X -= Player.Speed;
                        }
                        break;
                    }
                case Key.Right:
                    {
                        if (PlayerMove.X >= 0 && PlayerMove.X < 800 - Player.Width)
                        {
                            PlayerMove.X += Player.Speed;
                        }
                        break;
                    }
                default:
                    break;
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
            Maze.Clip = new EllipseGeometry()
            {
                Center = new Point(PlayerMove.X + Player.Width / 2, PlayerMove.Y + Player.Height / 2),
                RadiusX = Player.View,
                RadiusY = Player.View
            };
        }

        public void SetMap()
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < 16; ++i)
            {
                for (int j = 0; j < 12; ++j)
                {
                    if (i == 0 && j == 0)
                        continue;
                    var x = r.Next(0, 4);
                    if (x > 2)
                    {
                        Border rect = new Border();
                        rect.Background = new SolidColorBrush(Colors.Blue);
                        Grid.SetRow(rect, j);
                        Grid.SetColumn(rect, i);
                        rect.Height = Double.NaN;
                        rect.Width = Double.NaN;
                        Maze.Children.Add(rect);
                        _wall.Add(rect);
                    }
                }
            }
            Maze.Clip = new EllipseGeometry()
            {
                Center = new Point(PlayerMove.X + Player.Width / 2, PlayerMove.Y + Player.Height / 2),
                RadiusX = Player.View,
                RadiusY = Player.View
            };
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
    }
}
