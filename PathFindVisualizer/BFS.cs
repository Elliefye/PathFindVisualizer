using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PathFindVisualizer
{
    public static class BFS
    {
        public static List<Square> GetPath(Field field)
        {
            Queue<Square> ToCheck = new Queue<Square>();
            ToCheck.Enqueue(field.start);
            Dictionary<Square, Square> PreviousMoves = new Dictionary<Square, Square>();
            PreviousMoves.Add(field.start, null);
            Square current;

            while (ToCheck.Count > 0)
            {
                current = ToCheck.Dequeue();

                if (current.Equals(field.goal))
                {
                    break;
                }

                for(int i = 0; i < current.neighbors.Count; i++)
                {
                    if(current.neighbors[i].isWall || PreviousMoves.ContainsKey(current.neighbors[i]))
                    {
                        continue;
                    }
                    else if (!ToCheck.Contains(current.neighbors[i]))
                    {
                        ToCheck.Enqueue(current.neighbors[i]);
                        PreviousMoves.Add(current.neighbors[i], current);
                    }
                }
                current.ColorChecked();
            }

            List<Square> Path = new List<Square>();
            current = field.goal;
            while(current != field.start)
            {
                Path.Add(current);
                current = PreviousMoves[current];
            }
            Path.Add(field.start);
            Path.Reverse();
            return Path;
        }
    }
}
