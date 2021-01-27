using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace PathFindVisualizer
{
    public class Square
    {
        public int x { get; set; }
        public int y { get; set; }
        public bool isWall = false;
        private Rectangle uiRef;
        public List<Square> neighbors = new List<Square>();

        public Square(Rectangle uiRef)
        {
            this.uiRef = uiRef;
        }

        public Square(Rectangle uiRef, int x, int y)
        {
            this.uiRef = uiRef;
            this.x = x;
            this.y = y;
        }

        public void ResetColor()
        {
            uiRef.Fill = System.Windows.Media.Brushes.Gray;
        }

        public void ColorStart()
        {
            uiRef.Fill = System.Windows.Media.Brushes.Red;
        }

        public void ColorGoal()
        {
            uiRef.Fill = System.Windows.Media.Brushes.Blue;
        }

        public void ColorChecked()
        {
            uiRef.Fill = System.Windows.Media.Brushes.DarkGray;
        }

        public void ColorPath()
        {
            uiRef.Fill = System.Windows.Media.Brushes.Lavender;
        }
    }
}
