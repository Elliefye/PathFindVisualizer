using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    Grid.SetRow(square, j);
                    Grid.SetColumn(square, k);
                    square.Fill = System.Windows.Media.Brushes.Gray;
                    square.MouseLeftButtonUp += new MouseButtonEventHandler((s, e) => OnClickSquare(s, e));
                    PathGrid.Children.Add(square);
                    Field.current.AddSquare(new Square(square, j, k));
                }
            }
            Field.current.AddNeighbors();
        }

        void OnClickSquare(object sender, EventArgs e)
        {
            Rectangle uiSquare = (Rectangle)sender;

            if(m_choosingStart)
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

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Field.current.start == null || Field.current.goal == null)
            {
                MessageBox.Show("Select start and goal nodes!");
                return;
            }
            List<Square> path = BFS.GetPath(Field.current);

            foreach(Square node in path)
            {
                node.ColorPath();
            }
            
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach(object child in PathGrid.Children)
            {
                Rectangle rect = (Rectangle)child;
                rect.Fill = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}
