using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PathFindVisualizer
{
    public class Square :IComparable<Square>
    {
        public int x { get; set; }
        public int y { get; set; }
        public bool isWall = false;
        public int weight = 1;
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
            //Action a = () => uiRef.Fill = System.Windows.Media.Brushes.DarkGray;
            //Dispatcher.Invoke(a);
            if(weight == 1)
            {
                uiRef.Fill = System.Windows.Media.Brushes.DarkGray;
            }
            else
            {
                uiRef.Fill = System.Windows.Media.Brushes.LightSkyBlue;
            }    
        }

        public void ColorPath()
        {
            uiRef.Fill = System.Windows.Media.Brushes.Lavender;
        }

        public void ColorWall()
        {
            uiRef.Fill = System.Windows.Media.Brushes.Black;
        }

        public override string ToString()
        {
            return "Square: (" + x + ", " + y + ")";
        }

        public int CompareTo(Square other)
        {
            if (this.weight < other.weight)
            {
                return -1;
            }
            else if (this.weight > other.weight)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
