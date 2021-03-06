﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PathFindVisualizer
{
    public static class GreedyBFS
    {
        public static async Task<List<Square>> GetPath(Field field)
        {
            PriorityQueue<Square> ToCheck = new PriorityQueue<Square>();
            ToCheck.Enqueue(field.start, 0);
            Dictionary<Square, Square> PreviousMoves = new Dictionary<Square, Square>();
            PreviousMoves.Add(field.start, null);
            Square current;

            while (ToCheck.Count() > 0)
            {
                current = ToCheck.Dequeue();

                if (current.Equals(field.goal))
                {
                    break;
                }

                for (int i = 0; i < current.neighbors.Count; i++)
                {
                    Square next = current.neighbors[i];
                    if (next.isWall)
                    {
                        continue;
                    }
                    else if (!PreviousMoves.ContainsKey(next))
                    {
                        int distance = Field.MeasureDistance(field.goal, next);
                        ToCheck.Enqueue(next, distance);
                        PreviousMoves.Add(next, current);
                    }
                }
                if (current != field.start) //color visited squares
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => current.ColorChecked()));
                    int.TryParse(App.Current.Properties["Speed"].ToString(), out int Speed);
                    await Task.Delay(50 * Speed + 20);
                }
            }

            List<Square> Path = new List<Square>();
            current = field.goal;
            while (current != field.start)
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
