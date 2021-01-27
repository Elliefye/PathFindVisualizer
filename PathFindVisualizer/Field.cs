﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFindVisualizer
{
    public class Field
    {
        public Square start;
        public Square goal;
        public Square[,] field;

        public static Field current = null;

        public Field(int size)
        {
            field = new Square[size, size];
        }

        public void AddSquare(Square s)
        {
            field[s.x, s.y] = s;
        }

        public void SetStart(int x, int y)
        {
            start?.ResetColor();
            start = field[x, y];
            start.ColorStart();
        }

        public void SetGoal(int x, int y)
        {
            goal?.ResetColor();
            goal = field[x, y];
            goal.ColorGoal();
        }

        public void AddNeighbors()
        {
            for(int i = 0; i < field.GetLength(0); i++)
            {
                for(int j = 0; j < field.GetLength(1); j++)
                {
                    field[i, j].neighbors = FindNeighbors(field[i, j]);
                }
            }
        }

        private List<Square> FindNeighbors(Square s)
        {
            List<Square> neighbors = new List<Square>();

            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) //self node
                    {
                        continue;
                    }

                    int neighborX = s.x + i;
                    int neighborY = s.y + j;

                    if(neighborX != s.x && neighborY != s.y) //diagonals aren't neighbors
                    {
                        continue;
                    }

                    if((neighborX >= 0 && neighborX < field.GetLength(0)) && (neighborY >= 0 && neighborY < field.GetLength(1)))
                    {
                        neighbors.Add(field[neighborX, neighborY]);
                    }
                }
            }

            return neighbors;
        }

        //only supports 2 digit length coordinates
        public static (int, int) GetCoordinates(string name)
        {
            int x = int.Parse(name.Substring(1, 2));
            int y = int.Parse(name.Substring(3, 2));
            return (x, y);
        }
    }
}
