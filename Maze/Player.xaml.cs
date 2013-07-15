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

namespace Maze
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class Player : UserControl
    {
        public int Speed { get; set; }
        protected int _View;
        public int View {
            get { return cheating?5000:_View; }
            set { this._View = value; }
        }
        protected bool cheating = false;

        public Player()
        {
            InitializeComponent();
            Speed = 10;
            View = 50;
        }

        public void toggleCheating(bool _cheating)
        {
            cheating = _cheating;
        }
    }
}
