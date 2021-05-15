using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Input;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Text.RegularExpressions;

namespace tekenprogramma
{
    public sealed partial class MainPage : Page
    {
        string type = "Rectangle";
        bool selecting = false;
        bool grouping = false;
        Rectangle backuprectangle;
        Ellipse backupellipse;

        public Invoker invoker = new Invoker();
        public List<Shape> selectedShapesList = new List<Shape>();
        public FrameworkElement selectedElement;
        public List<FrameworkElement> selectElementsList = new List<FrameworkElement>();

        public Strategy strategy;

        //public Strategy rectangleStrategy = RectangleStrategy.GetInstance();
        //public Strategy ellipseStrategy = EllipseStrategy.GetInstance();

        public MainPage()
        {
            InitializeComponent();
        }

        private void Drawing_pressed(object sender, PointerRoutedEventArgs e)
        {

            FrameworkElement checkElement = e.OriginalSource as FrameworkElement;

            //canvas elements
            if (checkElement.Name == "Rectangle")
            {
                selectedElement = checkElement;
                selecting = true;
                Selecting(sender, e, selectedElement);
            }
            else if (checkElement.Name == "Ellipse")
            {
                selectedElement = checkElement;
                selecting = true;
                Selecting(sender, e, selectedElement);
            }
            //not canvas elements
            else
            {

                selecting = false;
                //move
                if (type == "Move")
                {
                    if (invoker.selectedGroups.Count() > 0)
                    {
                        MovingGroup(sender, e);
                    }
                    else
                    {
                        MovingShape(sender, e);
                    }

                }
                //resize
                else if (type == "Resize")
                {

                    if (invoker.selectedGroups.Count() > 0)
                    {
                        ResizingGroup(sender, e);
                    }
                    else
                    {
                        ResizingShape(sender, e);
                    }
                }
                //make shapes
                else if (type == "Rectangle")
                {
                    MakeRectangle(sender, e);
                }
                else if (type == "Elipse")
                {
                    MakeEllipse(sender, e);
                }
            }

        }

        //
        //shapes
        //

        //selecting shape
        private void Selecting(object sender, PointerRoutedEventArgs e, FrameworkElement element)
        {
            Shape shape = new Shape(e.GetCurrentPoint(paintSurface).Position.X, e.GetCurrentPoint(paintSurface).Position.Y, 50, 50);
            ICommand place = new Select(shape, e, this.invoker, paintSurface);
            this.invoker.Execute(place);
        }

        //make rectangle shape
        private void MakeRectangle(object sender, PointerRoutedEventArgs e)
        {
            Shape shape = new Shape(e.GetCurrentPoint(paintSurface).Position.X, e.GetCurrentPoint(paintSurface).Position.Y, 50, 50);
            ICommand place = new MakeRectangles(shape, this.invoker, paintSurface);
            this.invoker.Execute(place);
        }

        //make ellipse shape
        private void MakeEllipse(object sender, PointerRoutedEventArgs e)
        {
            Shape shape = new Shape(e.GetCurrentPoint(paintSurface).Position.X, e.GetCurrentPoint(paintSurface).Position.Y, 50, 50);
            ICommand place = new MakeEllipses(shape, this.invoker, paintSurface);
            this.invoker.Execute(place);
        }

        //moving shape
        private void MovingShape(object sender, PointerRoutedEventArgs e)
        {
            Location location = new Location();
            location.x = e.GetCurrentPoint(paintSurface).Position.X;
            location.y = e.GetCurrentPoint(paintSurface).Position.Y;
            location.width = selectedElement.Width;
            location.height = selectedElement.Height;
            Shape shape = new Shape(location.x, location.y, location.width, location.height);
            ICommand place = new Moving(shape, invoker, location, paintSurface, selectedElement);
            this.invoker.Execute(place);
        }

        //resizing shape
        private void ResizingShape(object sender, PointerRoutedEventArgs e)
        {
            //calculate size
            double ex = e.GetCurrentPoint(paintSurface).Position.X;
            double ey = e.GetCurrentPoint(paintSurface).Position.Y;
            double lw = Convert.ToDouble(selectedElement.ActualOffset.X); //set width
            double lh = Convert.ToDouble(selectedElement.ActualOffset.Y); //set height
            double w = ReturnSmallest(ex, lw);
            double h = ReturnSmallest(ey, lh);

            Location location = new Location();
            location.x = Convert.ToDouble(selectedElement.ActualOffset.X);
            location.y = Convert.ToDouble(selectedElement.ActualOffset.Y);
            location.width = w;
            location.height = h;

            Shape shape = new Shape(location.x, location.y, location.width, location.height);
            ICommand place = new Resize(shape, invoker, e, location, paintSurface, selectedElement);
            this.invoker.Execute(place);
        }

        //give smallest
        public double ReturnSmallest(double first, double last)
        {
            if (first < last)
            {
                return last - first;
            }
            else
            {
                return first - last;
            }
        }

        //
        //groups
        //

        //moving group
        private void MovingGroup(object sender, PointerRoutedEventArgs e)
        {
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            ICommand place = new MoveGroup(group, e, paintSurface, invoker, selectedElement);
            this.invoker.Execute(place);

        }

        //resizing group
        private void ResizingGroup(object sender, PointerRoutedEventArgs e)
        {
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            ICommand place = new ResizeGroup(group, e, paintSurface, invoker, selectedElement);
            this.invoker.Execute(place);
        }

        //
        //clicks
        //

        //move click
        private void Move_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            grouping = false;
            selecting = true;
        }

        //resize click
        private void Resize_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            grouping = false;
            selecting = true;
        }

        //elipse click
        private void Elipse_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            grouping = false;
            selecting = false;
        }

        //rectangle click
        private void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            grouping = false;
            selecting = false;
        }

        //ornament click
        private void Ornament_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            grouping = false;
        }

        //group click
        private void Group_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;

            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            ICommand place = new MakeGroup(group, paintSurface, invoker);
            this.invoker.Execute(place);
            grouping = true;
        }

        //undo click
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            invoker.Undo();
            grouping = false;
        }

        //redo click
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            invoker.Redo();
            grouping = false;
        }

        //save click
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            WriteClient writer = new WriteClient();
            IWriter visitor = new ConcreteVisitorWrite();
            writer.Client(this.paintSurface, this.invoker, visitor);
        }

        //load click
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            Shape command = new Shape(0, 0, 0, 0);
            ICommand place = new Loaded(command, paintSurface, invoker);
            invoker.Execute(place);
            grouping = false;
        }

        private void Front_canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {

        }

        private void Width_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Height_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}