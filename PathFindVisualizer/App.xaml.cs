using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PathFindVisualizer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly Brush Sdefault = System.Windows.Media.Brushes.Gray;
        public static readonly Brush Swall = System.Windows.Media.Brushes.Black;
        public static readonly Brush Sweight = System.Windows.Media.Brushes.DarkBlue;
        public static readonly Brush Sgoal = System.Windows.Media.Brushes.Green;
        public static readonly Brush Sstart = System.Windows.Media.Brushes.Red;
        public static readonly Brush SdefaultVisited = System.Windows.Media.Brushes.DarkGray;
        public static readonly Brush SweightVisited = System.Windows.Media.Brushes.LightSkyBlue;
        public static readonly Brush SdefaultPath = System.Windows.Media.Brushes.White;
        public static readonly Brush SweightPath = System.Windows.Media.Brushes.White;
    }
}
