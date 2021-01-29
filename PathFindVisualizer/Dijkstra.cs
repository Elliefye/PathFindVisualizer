using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PathFindVisualizer
{
    public static class Dijkstra
    {
        public static async Task<List<Square>> GetPath(Field field)
        {
            PriorityQueue<Square> ToCheck = new PriorityQueue<Square>();
            ToCheck.Enqueue(field.start, 0);
            Dictionary<Square, Square> PreviousMoves = new Dictionary<Square, Square>();
            Dictionary<Square, int> CostSoFar = new Dictionary<Square, int>();
            PreviousMoves.Add(field.start, null);
            CostSoFar[field.start] = 0;
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
                    int newCost = CostSoFar[current] + next.weight;
                    if (next.isWall)
                    {
                        continue;
                    }
                    else if (next == field.goal)
                    {
                        ToCheck.Enqueue(next, 0); //high priority for goal
                        if (!PreviousMoves.ContainsKey(next))
                            PreviousMoves.Add(next, current);
                    }
                    else if(!CostSoFar.ContainsKey(next) || newCost < CostSoFar[next])
                    {
                        CostSoFar[next] = newCost;
                        ToCheck.Enqueue(next, newCost);
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
