using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
        private readonly Rectangle uiRef;
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
            uiRef.Fill = App.Sdefault;
        }

        public void ColorStart()
        {
            uiRef.Fill = App.Sstart;
        }

        public void ColorGoal()
        {
            uiRef.Fill = App.Sgoal;
        }

        public void ColorChecked()
        {
            if(weight == 1)
            {
                uiRef.Fill = App.SdefaultVisited;
            }
            else
            {
                uiRef.Fill = App.SweightVisited;
            }    
        }

        public void ColorPath()
        {
            if (weight == 1)
            {
                uiRef.Fill = App.SdefaultPath;
            }
            else
            {
                uiRef.Fill = App.SweightPath;
            }
        }

        public void ColorWall()
        {
            uiRef.Fill = App.Swall;
        }

        public void ColorWeight()
        {
            uiRef.Fill = App.Sweight;
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
