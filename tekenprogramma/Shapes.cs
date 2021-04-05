using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using Windows.Storage;
using Windows.ApplicationModel.Activation;

namespace tekenprogramma
{
    public class Location
    {
        public double x;
        public double y;
        public double width;
        public double height;
    }


    public class Shape
    {
        public double x;
        public double y;
        public double width;
        public double height;
        private bool selected;
        private bool drawed;

        private List<ICommand> actionsList = new List<ICommand>();
        private List<ICommand> redoList = new List<ICommand>();

        private List<FrameworkElement> undoList = new List<FrameworkElement>();
        private List<FrameworkElement> reList = new List<FrameworkElement>();

        private List<Location> undoLocList = new List<Location>();
        private List<Location> redoLocList = new List<Location>();

        public Invoker invoker;
        public Canvas paintSurface;
        public PointerRoutedEventArgs pet;

        public Rectangle backuprectangle; //rectangle shape
        public Ellipse backupellipse; //ellipse shape
        string type = "Rectangle"; //default shape type
        //bool moved = false; //moving
        public FrameworkElement backelement; //backup element

        //file IO
        public string FileText { get; set; }

        //shape
        public Shape(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        // Selects the shape
        public void select(PointerRoutedEventArgs e)
        {
            this.selected = true;
        }

        // Deselects the shape
        public void deselect(PointerRoutedEventArgs e)
        {
            this.selected = false;
        }

        //give smallest
        public double returnSmallest(double first, double last)
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

        //make rectangle
        public void makeRectangle(Invoker invoker, Canvas paintSurface)
        {
            //paintSurface.Children.Clear();
            this.drawed = false;
            this.type = "Rectangle";
            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.Width = width; //set width
            newRectangle.Height = height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Blue; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, x); //set left position
            Canvas.SetTop(newRectangle, y); //set top position 
            paintSurface.Children.Add(newRectangle);
            this.drawed = true;
            this.backuprectangle = newRectangle;
            //backelement.Name = "Rectangle";
        }

        //make ellipse
        public void makeEllipse(Invoker invoker, Canvas paintSurface)
        {
            //paintSurface.Children.Clear();
            this.type = "Ellipse";
            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.Width = width;
            newEllipse.Height = height;
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Blue;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, x);//set left position
            Canvas.SetTop(newEllipse, y);//set top position
            paintSurface.Children.Add(newEllipse);
            this.drawed = true;
            this.backupellipse = newEllipse;
            //this.backelement.Name = "Ellipse";
        }

        //undo create
        public void remove(Invoker invoker, Canvas paintSurface)
        {

            this.drawed = false;

            if (this.type == "Rectangle")
            {
                // if the click source is a rectangle then we will create a new rectangle
                // and link it to the rectangle that sent the click event
                Rectangle activeRec = (Rectangle)backuprectangle; // create the link between the sender rectangle
                paintSurface.Children.Remove(activeRec); // find the rectangle and remove it from the canvas
            }
            else if (this.type == "Ellipse")
            {
                // if the click source is a rectangle then we will create a new ellipse
                // and link it to the rectangle that sent the click event
                Ellipse activeEll = (Ellipse)backupellipse; // create the link between the sender ellipse
                paintSurface.Children.Remove(activeEll); // find the ellipse and remove it from the canvas
            }

        }

        //moving shape
        public void moving(Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
        {


            if (this.type == "Rectangle")
            {
                // if the click source is a rectangle then we will create a new rectangle
                // and link it to the rectangle that sent the click event
                Rectangle activeRec = (Rectangle)element; // create the link between the sender rectangle
                paintSurface.Children.Remove(activeRec); // find the rectangle and remove it from the canvas

                Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
                newRectangle.Width = location.width; //set width
                newRectangle.Height = location.height; //set height     
                SolidColorBrush brush = new SolidColorBrush(); //brush
                brush.Color = Windows.UI.Colors.Blue; //standard brush color is blue
                newRectangle.Fill = brush; //fill color
                newRectangle.Name = "Rectangle"; //attach name
                //Canvas.SetLeft(newRectangle, e.GetCurrentPoint(paintSurface).Position.X); //set left position
                //Canvas.SetTop(newRectangle, e.GetCurrentPoint(paintSurface).Position.Y); //set top position 
                Canvas.SetLeft(newRectangle, location.x);
                Canvas.SetTop(newRectangle, location.y);
                paintSurface.Children.Add(newRectangle);
                this.backuprectangle = newRectangle;

                this.undoList.Add(newRectangle);

            }
            else if (this.type == "Ellipse")
            {
                // if the click source is a rectangle then we will create a new ellipse
                // and link it to the rectangle that sent the click event
                Ellipse activeEll = (Ellipse)element; // create the link between the sender ellipse
                paintSurface.Children.Remove(activeEll); // find the ellipse and remove it from the canvas

                Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
                newEllipse.Width = location.width;
                newEllipse.Height = location.height;
                SolidColorBrush brush = new SolidColorBrush();//brush
                brush.Color = Windows.UI.Colors.Blue;//standard brush color is blue
                newEllipse.Fill = brush;//fill color
                newEllipse.Name = "Ellipse";//attach name
                //Canvas.SetLeft(newEllipse, e.GetCurrentPoint(paintSurface).Position.X);//set left position
                //Canvas.SetTop(newEllipse, e.GetCurrentPoint(paintSurface).Position.Y);//set top position
                Canvas.SetLeft(newEllipse, location.x);//set left position
                Canvas.SetTop(newEllipse, location.y);//set top position
                paintSurface.Children.Add(newEllipse);
                this.backupellipse = newEllipse;

                this.undoList.Add(newEllipse);
            }

            location.x = element.ActualOffset.X;
            location.y = element.ActualOffset.Y;
            this.reList.Clear();
            this.redoLocList.Clear();

            this.undoLocList.Add(location);

            //paintSurface.Children.Clear();
            //x = e.GetCurrentPoint(paintSurface).Position.X;
            //y = e.GetCurrentPoint(paintSurface).Position.Y;

            //x = location.x;
            //y = location.y;

            //Canvas.SetLeft(element, x);
            //Canvas.SetTop(element, y);

            //this.x = location.x;
            //this.y = location.y;
            //this.width = location.width;
            //this.height = location.height;

            ////paintSurface.Children.Add(element);
            //this.x = location.x;
            //this.y = location.y;
            //this.width = location.width;
            //this.height = location.height;
            //paintSurface.Children.Clear();

        }

        //undo moving
        public void undoMoving(Invoker invoker, Canvas paintSurface)
        {
            //paintSurface.Children.Clear(); //repaint
            if (undoLocList.Count() > 0)
            {

                FrameworkElement element = this.undoList.Last();
                Location location = this.undoLocList.Last();

                if (this.type == "Rectangle")
                {
                    // if the click source is a rectangle then we will create a new rectangle
                    // and link it to the rectangle that sent the click event
                    Rectangle activeRec = (Rectangle)element; // create the link between the sender rectangle
                    paintSurface.Children.Remove(activeRec); // find the rectangle and remove it from the canvas

                    Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
                    newRectangle.Width = location.width; //set width
                    newRectangle.Height = location.height; //set height     
                    SolidColorBrush brush = new SolidColorBrush(); //brush
                    brush.Color = Windows.UI.Colors.Blue; //standard brush color is blue
                    newRectangle.Fill = brush; //fill color
                    newRectangle.Name = "Rectangle"; //attach name

                    Canvas.SetLeft(newRectangle, location.x);
                    Canvas.SetTop(newRectangle, location.y);
                    paintSurface.Children.Add(newRectangle);
                    this.backuprectangle = newRectangle;

                    this.reList.Add(newRectangle);

                }
                else if (this.type == "Ellipse")
                {
                    // if the click source is a rectangle then we will create a new ellipse
                    // and link it to the rectangle that sent the click event
                    Ellipse activeEll = (Ellipse)element; // create the link between the sender ellipse
                    paintSurface.Children.Remove(activeEll); // find the ellipse and remove it from the canvas

                    Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
                    newEllipse.Width = location.width;
                    newEllipse.Height = location.height;
                    SolidColorBrush brush = new SolidColorBrush();//brush
                    brush.Color = Windows.UI.Colors.Blue;//standard brush color is blue
                    newEllipse.Fill = brush;//fill color
                    newEllipse.Name = "Ellipse";//attach name

                    Canvas.SetLeft(newEllipse, location.x);//set left position
                    Canvas.SetTop(newEllipse, location.y);//set top position
                    paintSurface.Children.Add(newEllipse);
                    this.backupellipse = newEllipse;

                    this.reList.Add(newEllipse);
                }

                undoLocList.RemoveAt(undoLocList.Count - 1);
                undoList.RemoveAt(undoList.Count - 1);
                this.redoLocList.Add(location);


                //Location location = this.undoLocList.Last();

                //FrameworkElement element = this.undoList.Last();
                //FrameworkElement element = new FrameworkElement();

                //Canvas.SetLeft(element, location.x);
                //Canvas.SetTop(element, location.y);

                //paintSurface.Children.Clear();
                //FrameworkElement element = this.undoList.Last();

                //x = element.ActualOffset.X;
                //y = element.ActualOffset.Y;
                //Canvas.SetLeft(element, x);
                //Canvas.SetTop(element, y);

                //paintSurface.Children.Add(element);     

            }


        }

        //redo moving
        //public void redoMoving(Canvas paintSurface)
        //public void redoMoving(PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface)
        public void redoMoving(Invoker invoker, Canvas paintSurface)
        {
            //ICommand lastshape = invoker.redoList.Last();
            //paintSurface.Children.Clear(); //repaint
            //x = lastshape.Redo().x;



            if (this.reList.Count() > 0)
            {

                FrameworkElement element = this.reList.Last();
                Location location = this.redoLocList.Last();

                //FrameworkElement element = this.reList.Last();
                //this.reList.RemoveAt(this.reList.Count - 1);
                //x = element.ActualOffset.X;
                //y = element.ActualOffset.Y;
                ////Canvas.SetLeft(element, x);
                ////Canvas.SetTop(element, y);
                //this.undoList.Add(element);
                ////x = e.GetCurrentPoint(paintSurface).Position.X;
                ////y = e.GetCurrentPoint(paintSurface).Position.Y;
                //Canvas.SetLeft(element, this.x);
                //Canvas.SetTop(element, this.y);
                //this.undoList.Add(element);
                ////paintSurface.Children.Add(element);

                if (this.type == "Rectangle")
                {
                    // if the click source is a rectangle then we will create a new rectangle
                    // and link it to the rectangle that sent the click event
                    Rectangle activeRec = (Rectangle)element; // create the link between the sender rectangle
                    paintSurface.Children.Remove(activeRec); // find the rectangle and remove it from the canvas

                    Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
                    newRectangle.Width = width; //set width
                    newRectangle.Height = height; //set height     
                    SolidColorBrush brush = new SolidColorBrush(); //brush
                    brush.Color = Windows.UI.Colors.Blue; //standard brush color is blue
                    newRectangle.Fill = brush; //fill color
                    newRectangle.Name = "Rectangle"; //attach name
                    Canvas.SetLeft(newRectangle, this.x); //set left position
                    Canvas.SetTop(newRectangle, this.y); //set top position 
                    paintSurface.Children.Add(newRectangle);
                }
                else
                {
                    // if the click source is a rectangle then we will create a new ellipse
                    // and link it to the rectangle that sent the click event
                    Ellipse activeEll = (Ellipse)element; // create the link between the sender ellipse
                    paintSurface.Children.Remove(activeEll); // find the ellipse and remove it from the canvas

                    Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
                    newEllipse.Width = width;
                    newEllipse.Height = height;
                    SolidColorBrush brush = new SolidColorBrush();//brush
                    brush.Color = Windows.UI.Colors.Blue;//standard brush color is blue
                    newEllipse.Fill = brush;//fill color
                    newEllipse.Name = "Ellipse";//attach name
                    Canvas.SetLeft(newEllipse, this.x);//set left position
                    Canvas.SetTop(newEllipse, this.y);//set top position
                    paintSurface.Children.Add(newEllipse);
                }

                redoLocList.RemoveAt(redoLocList.Count - 1);
                reList.RemoveAt(reList.Count - 1);
                this.undoList.Add(element);
                this.undoLocList.Add(location);

            }

        }

        //resize shape
        public void resize(Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
        {
            //paintSurface.Children.Clear();
            double ex = e.GetCurrentPoint(paintSurface).Position.X;
            double ey = e.GetCurrentPoint(paintSurface).Position.Y;
            double lw = Convert.ToDouble(element.ActualOffset.X); //set width
            double lh = Convert.ToDouble(element.ActualOffset.Y); //set height
            double w = returnSmallest(ex, lw);
            double h = returnSmallest(ey, lh);
            element.Width = w;
            element.Height = h;
            this.undoList.Add(element);
        }

        //undo resize
        public void undoResize(Invoker invoker, Canvas paintSurface)
        {
            //paintSurface.Children.Clear(); //repaint
            //FrameworkElement prevelement = this.undoList.Last();
            //backelement.Width = prevelement.Width;
            //backelement.Height = prevelement.Height;
            //x = prevelement.ActualOffset.X;
            //y = prevelement.ActualOffset.Y;
            //Canvas.SetLeft(prevelement, x);
            //Canvas.SetTop(prevelement, y);
            //this.reList.Add(backelement);
            paintSurface.Children.Clear(); //repaint
            FrameworkElement element = this.undoList.Last();
            //backelement.Width = prevelement.Width;
            //backelement.Height = prevelement.Height;
            //x = prevelement.ActualOffset.X;
            //y = prevelement.ActualOffset.Y;
            //Canvas.SetLeft(prevelement, x);
            //Canvas.SetTop(prevelement, y);
            this.reList.Add(element);
        }

        //redo resize
        public void redoResize(Invoker invoker, Canvas paintSurface)
        {
            //paintSurface.Children.Clear(); //repaint
            //FrameworkElement prevelement = this.reList.Last();
            //backelement.Width = prevelement.Width;
            //backelement.Height = prevelement.Height;
            //x = prevelement.ActualOffset.X;
            //y = prevelement.ActualOffset.Y;
            //Canvas.SetLeft(prevelement, x);
            //Canvas.SetTop(prevelement, y);
            //this.undoList.Add(backelement);
            //paintSurface.Children.Clear(); //repaint
            FrameworkElement element = this.reList.Last();
            this.undoList.Add(element);
        }

        //saving
        public async void saving(Canvas paintSurface)
        {

            try
            {
                string lines = "";

                foreach (FrameworkElement child in paintSurface.Children)
                {
                    if (child is Rectangle)
                    {
                        double top = (double)child.GetValue(Canvas.TopProperty);
                        double left = (double)child.GetValue(Canvas.LeftProperty);
                        string str = "rectangle " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                        lines += str;
                    }
                    else
                    {
                        double top = (double)child.GetValue(Canvas.TopProperty);
                        double left = (double)child.GetValue(Canvas.LeftProperty);
                        string str = "ellipse " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                        lines += str;
                    }
                }
                //create and write to file
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("dp2data.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, lines);
            }
            catch (System.IO.FileNotFoundException)
            {
                FileText = "File not found.";
            }
            catch (System.IO.FileLoadException)
            {
                FileText = "File Failed to load";
            }
            catch (System.IO.IOException e)
            {
                FileText = "File IO error " + e;
            }
            catch (Exception err)
            {
                FileText = err.Message;
            }

        }

        //loading
        public async void loading(Canvas paintSurface)
        {
            //clear previous canvas
            paintSurface.Children.Clear();
            //read file
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile saveFile = await storageFolder.GetFileAsync("dp2data.txt");
            string text = await Windows.Storage.FileIO.ReadTextAsync(saveFile);
            //load shapes
            string[] readText = Regex.Split(text, "\\n+");
            foreach (string s in readText)
            {
                if (s.Length > 2)
                {
                    string[] line = Regex.Split(s, "\\s+");
                    if (line[0] == "Ellipse")
                    {
                        this.getEllipse(s, paintSurface);
                    }
                    else
                    {
                        this.getRectangle(s, paintSurface);
                    }
                }
            }
        }

        //load ellipse
        public void getEllipse(String lines, Canvas paintSurface)
        {
            string[] line = Regex.Split(lines, "\\s+");

            double x = Convert.ToDouble(line[1]);
            double y = Convert.ToDouble(line[2]);
            double width = Convert.ToDouble(line[3]);
            double height = Convert.ToDouble(line[4]);

            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.Width = width;
            newEllipse.Height = height;
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Blue;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, x);//set left position
            Canvas.SetTop(newEllipse, y);//set top position
            paintSurface.Children.Add(newEllipse);
        }

        //load rectangle
        public void getRectangle(String lines, Canvas paintSurface)
        {
            string[] line = Regex.Split(lines, "\\s+");

            double x = Convert.ToDouble(line[1]);
            double y = Convert.ToDouble(line[2]);
            double width = Convert.ToDouble(line[3]);
            double height = Convert.ToDouble(line[4]);

            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.Width = width; //set width
            newRectangle.Height = height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Blue; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, x); //set left position
            Canvas.SetTop(newRectangle, y); //set top position 
            paintSurface.Children.Add(newRectangle);
        }

    }

}

