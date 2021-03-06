﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PathFindVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool m_choosingStart = true;
        private bool m_choosingGoal = false;
        private bool m_drawingWalls = false;
        private bool m_drawingWeights = false;

        public MainWindow()
        {
            InitializeComponent();
            App.Current.Properties["Speed"] = 0;
        }

        private void PopulateGrid()
        {
            int RowNo = 10;
            int ColNo = 25;

            if (PathGrid.ActualWidth != 0 && PathGrid.Width != 0)
            {
                RowNo = (int)PathGrid.ActualHeight / 30;
                ColNo = (int)PathGrid.ActualWidth / 30;
                PathGrid.RowDefinitions.Clear();
                PathGrid.ColumnDefinitions.Clear();
                PathGrid.Children.Clear();
            }

            Field.current = new Field(RowNo, ColNo);

            for (int i = 0; i <= RowNo; i++)
            {
                PathGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int j = 0; j < ColNo; j++)
            {
                PathGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int j = 0; j < RowNo; j++) //rows (height)
            {
                for (int k = 0; k < ColNo; k++) //columns (width)
                {
                    Rectangle square = new Rectangle();

                    string name = "_";
                    if (j < 10)
                        name += "0";
                    name += j;
                    if (k < 10)
                        name += "0";
                    name += k;

                    square.Name = name;
                    square.RadiusX = 50;
                    square.RadiusY = 50;
                    square.AllowDrop = true;
                    Grid.SetRow(square, j);
                    Grid.SetColumn(square, k);
                    square.Fill = App.Sdefault;
                    square.MouseLeftButtonUp += new MouseButtonEventHandler((s, e) => Square_OnMouseLeftButtonUp(s, e));
                    square.MouseMove += new MouseEventHandler((s, e) => Square_OnMouseMove(s, e));
                    square.DragEnter += new DragEventHandler((s, e) => Square_DragEnter(s, e));
                    PathGrid.Children.Add(square);
                    Field.current.AddSquare(new Square(square, j, k));
                }
            }
            Field.current.AddNeighbors();
        }

        //click
        private void Square_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle uiSquare = (Rectangle)sender;
            var coord = Field.GetCoordinates(uiSquare.Name);

            if (m_drawingWalls)
            {
                if (Field.current.field[coord.Item1, coord.Item2].isWall)
                {
                    Field.current.SetWall(coord.Item1, coord.Item2, false);
                }
                //goal or start can't be a wall
                else if (Field.current.goal != Field.current.field[coord.Item1, coord.Item2] && 
                    Field.current.start != Field.current.field[coord.Item1, coord.Item2])
                {
                    Field.current.SetWall(coord.Item1, coord.Item2);
                }
            }
            else if(m_drawingWeights)
            {
                //if already a weight, reset
                if (Field.current.field[coord.Item1, coord.Item2].weight > 1)
                {
                    Field.current.SetWeight(coord.Item1, coord.Item2, 1);
                }
                else if (Field.current.goal != Field.current.field[coord.Item1, coord.Item2] &&
                    Field.current.start != Field.current.field[coord.Item1, coord.Item2] &&
                    !Field.current.field[coord.Item1, coord.Item2].isWall)
                {
                    Field.current.SetWeight(coord.Item1, coord.Item2);
                }
            }
            else if(m_choosingStart)
            {
                Field.current.SetStart(coord.Item1, coord.Item2);
                m_choosingStart = false;
                m_choosingGoal = true;
            }
            else if(m_choosingGoal)
            {
                Field.current.SetGoal(coord.Item1, coord.Item2);
                m_choosingGoal = false;
                m_choosingStart = true;
            }
        }

        //drag/drop enter
        private void Square_DragEnter(object sender, DragEventArgs e)
        {
            Rectangle uiSquare = (Rectangle)sender;
            var coord = Field.GetCoordinates(uiSquare.Name);

            if (Field.current.goal != Field.current.field[coord.Item1, coord.Item2] &&
                    Field.current.start != Field.current.field[coord.Item1, coord.Item2])
            {
                if (m_drawingWalls)
                {
                    if(!Field.current.field[coord.Item1, coord.Item2].isWall)
                    {
                        Field.current.SetWall(coord.Item1, coord.Item2);
                    }
                    else Field.current.SetWall(coord.Item1, coord.Item2, false);
                }
                //walls have higher priority than weights
                else if (!Field.current.field[coord.Item1, coord.Item2].isWall && m_drawingWeights)
                {
                    if(Field.current.field[coord.Item1, coord.Item2].weight == 1)
                    {
                        Field.current.SetWeight(coord.Item1, coord.Item2);
                    }
                    else Field.current.SetWeight(coord.Item1, coord.Item2, 1);
                }
            }
        }

        //begin drag/drop
        private void Square_OnMouseMove(object sender, MouseEventArgs e)
        {
            Rectangle uiSquare = sender as Rectangle;
            var coord = Field.GetCoordinates(uiSquare.Name);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Field.current.goal != Field.current.field[coord.Item1, coord.Item2] &&
                Field.current.start != Field.current.field[coord.Item1, coord.Item2])
                {
                    if (m_drawingWalls)
                    {
                        DragDrop.DoDragDrop(uiSquare, "", DragDropEffects.Copy);
                    }
                    else if (m_drawingWeights && !Field.current.field[coord.Item1, coord.Item2].isWall)
                    {
                        DragDrop.DoDragDrop(uiSquare, "", DragDropEffects.Copy);
                    }
                }
            }
        }

        private async void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Field.current.start == null || Field.current.goal == null)
            {
                MessageBox.Show("Select start and goal nodes!");
                return;
            }

            ResetBtn_Click(null, null);
            List<Square> path;

            try
            {
                switch(AlgSelect.SelectedIndex)
                {
                    case 0:
                        path = await AStar.GetPath(Field.current);
                        break;
                    case 1:
                        path = await Dijkstra.GetPath(Field.current);
                        break;
                    case 2:
                        path = await GreedyBFS.GetPath(Field.current);
                        break;
                    case 3:
                        path = await BFS.GetPath(Field.current);
                        break;
                    default:
                        MessageBox.Show("Error selecting algorithm.");
                        return;
                }
            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("Path could not be found.");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error finding path: " + ex.Message);
                return;
            }

            for(int i = 0; i < path.Count - 1; i++)
            {
                if(i ==0)
                {
                    path[i].ColorStart();
                }
                else path[i].ColorPath();
                UpdateUI();
                int.TryParse(App.Current.Properties["Speed"].ToString(), out int Speed);
                Thread.Sleep(50 * Speed + 20);
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            Field.current.start = null;
            Field.current.goal = null;
            Field.current.ClearWalls();
            foreach(object child in PathGrid.Children)
            {
                Rectangle rect = (Rectangle)child;
                rect.Fill = App.Sdefault;
            }
        }

        private void WallsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(!m_drawingWalls)
            {
                WeightsCheckBox.IsChecked = false;
            }
            m_drawingWalls = !m_drawingWalls;
        }

        private void WeightsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_drawingWeights)
            {
                WallsCheckBox.IsChecked = false;
            }
            m_drawingWeights = !m_drawingWeights;
        }

        private void UpdateUI()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object par)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            WallsCheckBox.IsChecked = false;
            foreach (object child in PathGrid.Children)
            {
                Rectangle rect = (Rectangle)child;
                Square current = Field.current.field[Field.GetCoordinates(rect.Name).Item1, Field.GetCoordinates(rect.Name).Item2];

                if (current != Field.current.start && current != Field.current.goal && !current.isWall && current.weight == 1)
                {
                    current.ResetColor();
                }
                else if (current.weight > 1)
                {
                    current.ColorWeight();
                }
                else if (current.isWall)
                {
                    current.ColorWall();
                }
            }
        }

        private void SpeedSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.Current.Properties["Speed"] = SpeedSelect.SelectedIndex;
        }

        private void PathGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PopulateGrid();
        }
    }
}
