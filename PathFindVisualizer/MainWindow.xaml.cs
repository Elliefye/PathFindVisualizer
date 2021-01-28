using System;
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

        public MainWindow()
        {
            InitializeComponent();

            PopulateGrid();
        }

        private void DisplayChoices()
        {
            
        }

        private void PopulateGrid()
        {
            int width = 10;
            Field.current = new Field(width);

            for (int i = 0; i <= width; i++)
            {
                PathGrid.ColumnDefinitions.Add(new ColumnDefinition());
                PathGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int j = 0; j < width; j++) //rows
            {
                for (int k = 0; k < width; k++) //columns
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
                    square.Fill = System.Windows.Media.Brushes.Gray;
                    square.MouseLeftButtonUp += new MouseButtonEventHandler((s, e) => Square_OnMouseLeftButtonUp(s, e));
                    square.MouseLeftButtonDown += new MouseButtonEventHandler((s, e) => Square_OnMouseLeftButtonDown(s, e));
                    square.MouseMove += new MouseEventHandler((s, e) => Square_OnMouseMove(s, e));
                    square.DragEnter += new DragEventHandler((s, e) => Square_DragEnter(s, e));
                    PathGrid.Children.Add(square);
                    Field.current.AddSquare(new Square(square, j, k));
                }
            }
            Field.current.AddNeighbors();
        }

        private void Square_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle uiSquare = (Rectangle)sender;

            if(m_drawingWalls)
            {
                //goal or start can't be a wall
                if(Field.current.goal != Field.current.field[Field.GetCoordinates(uiSquare.Name).Item1,
                    Field.GetCoordinates(uiSquare.Name).Item2] &&
                    Field.current.start != Field.current.field[Field.GetCoordinates(uiSquare.Name).Item1,
                    Field.GetCoordinates(uiSquare.Name).Item2])
                {
                    Field.current.SetWall(Field.GetCoordinates(uiSquare.Name).Item1,
                    Field.GetCoordinates(uiSquare.Name).Item2);
                    uiSquare.Fill = System.Windows.Media.Brushes.Black;
                }
            }
            else if(m_choosingStart)
            {
                Field.current.SetStart(Field.GetCoordinates(uiSquare.Name).Item1, 
                    Field.GetCoordinates(uiSquare.Name).Item2);
                m_choosingStart = false;
                m_choosingGoal = true;
            }
            else if(m_choosingGoal)
            {
                Field.current.SetGoal(Field.GetCoordinates(uiSquare.Name).Item1, 
                    Field.GetCoordinates(uiSquare.Name).Item2);
                m_choosingGoal = false;
                m_choosingStart = true;
            }
        }

        private void Square_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle uiSquare = (Rectangle)sender;

            if (m_drawingWalls)
            {
                //goal or start can't be a wall
                if (Field.current.goal != Field.current.field[Field.GetCoordinates(uiSquare.Name).Item1,
                    Field.GetCoordinates(uiSquare.Name).Item2] &&
                    Field.current.start != Field.current.field[Field.GetCoordinates(uiSquare.Name).Item1,
                    Field.GetCoordinates(uiSquare.Name).Item2])
                {
                    Field.current.SetWall(Field.GetCoordinates(uiSquare.Name).Item1,
                    Field.GetCoordinates(uiSquare.Name).Item2);
                    uiSquare.Fill = System.Windows.Media.Brushes.Black;
                }
            }
        }

        private void Square_DragEnter(object sender, DragEventArgs e)
        {
            Rectangle uiSquare = (Rectangle)sender;

            if (m_drawingWalls && e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                //goal or start can't be a wall
                if (Field.current.goal != Field.current.field[Field.GetCoordinates(uiSquare.Name).Item1,
                    Field.GetCoordinates(uiSquare.Name).Item2] &&
                    Field.current.start != Field.current.field[Field.GetCoordinates(uiSquare.Name).Item1,
                    Field.GetCoordinates(uiSquare.Name).Item2])
                {
                    string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

                    // If the string can be converted into a Brush, convert it.
                    BrushConverter converter = new BrushConverter();
                    if (converter.IsValid(dataString))
                    {
                        Brush wallColor = (Brush)converter.ConvertFromString(dataString);
                        uiSquare.Fill = wallColor;
                    }
                    Field.current.SetWall(Field.GetCoordinates(uiSquare.Name).Item1,
                    Field.GetCoordinates(uiSquare.Name).Item2);
                }
            }
        }

        private void Square_OnMouseMove(object sender, MouseEventArgs e)
        {
            if(m_drawingWalls)
            {
                Rectangle square = sender as Rectangle;
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragDrop.DoDragDrop(square, square.Fill.ToString(), DragDropEffects.Copy);
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
            List<Square> path;
            try
            {
                switch(AlgSelect.SelectedIndex)
                {
                    case 0:
                        path = await BFS.GetPath(Field.current);
                        break;
                    case 1:
                        path = await Dijkstra.GetPath(Field.current);
                        break;
                    case 2:
                        path = await GreedyBFS.GetPath(Field.current);
                        break;
                    default:
                        MessageBox.Show("Error selecting algorithm.");
                        return;
                }
            }
            catch
            {
                MessageBox.Show("Path could not be found.");
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
                Thread.Sleep(100);
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
                rect.Fill = System.Windows.Media.Brushes.Gray;
            }
        }

        private void WallsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            m_drawingWalls = !m_drawingWalls;
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
    }
}
