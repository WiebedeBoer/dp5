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

    //shape class
    public class Shape : Baseshape
    {
        public double x;
        public double y;
        public double width;
        public double height;

        public Invoker invoker;
        public Canvas paintSurface;

        public FrameworkElement prevelement; //prev element
        public FrameworkElement nextelement; //next element
        public FrameworkElement selectedElement; //selected element

        public IComponent prevcomponent;
        public IComponent nextcomponent;

        //file IO
        public string fileText { get; set; }

        //shape
        public Shape(double x, double y, double width, double height) : base(height, width, x, y)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        // Selects the shape
        public void Select(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface)
        {
            selectedElement = e.OriginalSource as FrameworkElement;//2af
            selectedElement.Opacity = 0.6; //fill opacity
            invoker.selectElements.Add(selectedElement); //2a+
            //see if in group
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            group.SelectInGroup(selectedElement, invoker);
        }

        // Deselects the shape
        public void Deselect(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface)
        {
            selectedElement = invoker.selectElements.Last(); //2af  //er1
            selectedElement.Opacity = 1; //fill opacity
            invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1); //2a-
            invoker.unselectElements.Add(selectedElement); //2b+
            //see if in group
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            group.UnselectGroup(selectedElement, invoker);
        }

        // Reselect the shape
        public void Reselect(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface)
        {
            selectedElement = invoker.unselectElements.Last(); //2bf
            selectedElement.Opacity = 0.6; //fill opacity
            invoker.unselectElements.RemoveAt(invoker.unselectElements.Count() - 1); //2b-
            invoker.selectElements.Add(selectedElement); //2a+
            //see if in group
            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            group.SelectInGroup(selectedElement, invoker);
        }



        //
        //repaint
        //
        public void Repaint(Invoker invoker, Canvas paintSurface)
        {
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                paintSurface.Children.Add(drawelement); //add
            }
        }

        //
        //creation
        //

        //create rectangle
        public void MakeRectangle(Invoker invoker, Canvas paintSurface)
        {
            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.AccessKey = invoker.executer.ToString();
            newRectangle.Width = width; //set width
            newRectangle.Height = height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Blue; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, x); //set left position
            Canvas.SetTop(newRectangle, y); //set top position 
            invoker.drawnElements.Add(newRectangle);
            Repaint(invoker, paintSurface); //repaint

            Strategy component = ConcreteComponentRectangle.GetInstance();
            invoker.drawnComponents.Add(component);
        }

        //create ellipse
        public void MakeEllipse(Invoker invoker, Canvas paintSurface)
        {
            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.AccessKey = invoker.executer.ToString();
            newEllipse.Width = width;
            newEllipse.Height = height;
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Blue;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, x);//set left position
            Canvas.SetTop(newEllipse, y);//set top position   
            invoker.drawnElements.Add(newEllipse);
            Repaint(invoker, paintSurface); //repaint

            Strategy component = ConcreteComponentEllipse.GetInstance();
            invoker.drawnComponents.Add(component);
        }

        //
        //undo redo
        //

        //undo create
        public void Remove(Invoker invoker, Canvas paintSurface)
        {
            //remove previous
            prevelement = invoker.drawnElements.Last();
            invoker.removedElements.Add(prevelement);
            invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
            Repaint(invoker, paintSurface); //repaint

            prevcomponent = invoker.drawnComponents.Last();
            invoker.removedComponents.Add(prevcomponent);
            invoker.drawnComponents.RemoveAt(invoker.drawnComponents.Count() - 1);
        }

        //redo create
        public void Add(Invoker invoker, Canvas paintSurface)
        {
            //create next
            nextelement = invoker.removedElements.Last();
            invoker.drawnElements.Add(nextelement);
            invoker.removedElements.RemoveAt(invoker.removedElements.Count() - 1);
            Repaint(invoker, paintSurface); //repaint

            nextcomponent = invoker.removedComponents.Last();
            invoker.drawnComponents.Add(nextcomponent);
            invoker.removedComponents.RemoveAt(invoker.removedComponents.Count() - 1);
        }

        //
        //moving
        //

        //new moving shape
        public void Moving(Invoker invoker, Canvas paintSurface, Location location, FrameworkElement element)
        {
            //FrameworkElement element = invoker.selectElementsList.Last();
            KeyNumber(element, invoker); //move selected at removed
            //create at new location
            MovingElement(element, invoker, paintSurface, location);
            Repaint(invoker, paintSurface); //repaint
        }


        public FrameworkElement MovingElement(FrameworkElement element, Invoker invoker, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            //remove selected element from drawn
            KeyNumber(element, invoker); //1-
            //add selected to unselect
            invoker.unselectElements.Add(element); //2b+
            //remove from selected
            invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1); //2a-
            //add to moved
            invoker.movedElements.Add(element); //3a+
            if (element.Name == "Rectangle")
            {
                Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
                newRectangle.AccessKey = invoker.executer.ToString();
                newRectangle.Width = location.width; //set width
                newRectangle.Height = location.height; //set height     
                SolidColorBrush brush = new SolidColorBrush(); //brush
                brush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
                newRectangle.Fill = brush; //fill color
                newRectangle.Name = "Rectangle"; //attach name
                Canvas.SetLeft(newRectangle, location.x);//set left position
                Canvas.SetTop(newRectangle, location.y); //set top position          
                //add new to drawn
                invoker.drawnElements.Add(newRectangle); //1+
                //add undo
                invoker.unmovedElements.Add(newRectangle); //3b+
                returnelement = newRectangle;
            }
            else if (element.Name == "Ellipse")
            {
                Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
                newEllipse.AccessKey = invoker.executer.ToString();
                newEllipse.Width = location.width;
                newEllipse.Height = location.height;
                SolidColorBrush brush = new SolidColorBrush();//brush
                brush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
                newEllipse.Fill = brush;//fill color
                newEllipse.Name = "Ellipse";//attach name
                Canvas.SetLeft(newEllipse, location.x);//set left position
                Canvas.SetTop(newEllipse, location.y);//set top position
                //add new to drawn
                invoker.drawnElements.Add(newEllipse); //1+
                //add undo
                invoker.unmovedElements.Add(newEllipse); //3b+
                returnelement = newEllipse;
            }
            return returnelement;
        }

        //remove selected element by access key
        public void KeyNumber(FrameworkElement element, Invoker invoker)
        {
            string key = element.AccessKey;
            int inc = 0;
            int number = 0;
            foreach (FrameworkElement drawn in invoker.drawnElements)
            {
                if (drawn.AccessKey == key)
                {
                    number = inc;
                }
                inc++;
            }
            invoker.drawnElements.RemoveAt(number);
            invoker.removedElements.Add(element);
            invoker.movedElements.Add(element);
        }

        //move back element
        public void MoveBack(Invoker invoker, Canvas paintSurface)
        {
            //shuffle unselected
            prevelement = invoker.movedElements.Last();
            invoker.unselectElements.RemoveAt(invoker.unselectElements.Count() - 1); //2b-
            invoker.selectElements.Add(prevelement); //2a+
            //shuffle moved
            nextelement = invoker.unmovedElements.Last();
            invoker.movedElements.RemoveAt(invoker.movedElements.Count() - 1); //3a-
            invoker.unmovedElements.RemoveAt(invoker.unmovedElements.Count() - 1); //3b-
            //add redo
            invoker.undoElements.Add(nextelement); //4a+
            invoker.redoElements.Add(prevelement); //4b+
            //remove and add to drawn
            KeyNumber(nextelement, invoker); //1-
            invoker.drawnElements.Add(prevelement); //1+
            //repaint surface
            Repaint(invoker, paintSurface);
        }

        //move back element
        public void MoveAgain(Invoker invoker, Canvas paintSurface)
        {
            //shuffle selected
            nextelement = invoker.undoElements.Last();
            invoker.unselectElements.Add(nextelement); //2b+
            invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1); //2a-
            //shuffle moved
            prevelement = invoker.redoElements.Last();
            invoker.movedElements.Add(prevelement); //3a+
            invoker.unmovedElements.Add(nextelement); //3b+
            //undo redo
            invoker.undoElements.RemoveAt(invoker.undoElements.Count() - 1); ; //4a-
            invoker.redoElements.RemoveAt(invoker.redoElements.Count() - 1); ; //4b-
            //remove and add to drawn
            KeyNumber(prevelement, invoker); //1-
            invoker.drawnElements.Add(nextelement); //1+
            //repaint surface
            Repaint(invoker, paintSurface);
        }

        //
        //resizing
        //

        //resize shape
        public void Resize(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement element)
        {
            //FrameworkElement element = invoker.selectElementsList.Last();
            //KeyNumber(element, invoker); //move selected at removed

            //calculate size
            double ex = e.GetCurrentPoint(paintSurface).Position.X;
            double ey = e.GetCurrentPoint(paintSurface).Position.Y;
            double lw = Convert.ToDouble(element.ActualOffset.X); //set width
            double lh = Convert.ToDouble(element.ActualOffset.Y); //set height
            double w = ReturnSmallest(ex, lw);
            double h = ReturnSmallest(ey, lh);

            Location location = new Location();
            location.x = Convert.ToDouble(element.ActualOffset.X);
            location.y = Convert.ToDouble(element.ActualOffset.Y);
            location.width = w;
            location.height = h;

            ResizingElement(invoker, e, paintSurface, location, element);
            Repaint(invoker, paintSurface); //repaint
        }

        public FrameworkElement ResizingElement(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, Location location, FrameworkElement element)
        {
            FrameworkElement returnelement = null;
            //remove selected element from drawn
            KeyNumber(element, invoker); //1-
            //add selected to unselect
            invoker.unselectElements.Add(element); //2b+
            //remove from selected
            invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1); //2a-
            //add to moved
            invoker.movedElements.Add(element); //3a+
            //create at new size
            if (element.Name == "Rectangle")
            {
                Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
                newRectangle.AccessKey = invoker.executer.ToString();
                newRectangle.Width = location.width; //set width
                newRectangle.Height = location.height; //set height     
                SolidColorBrush brush = new SolidColorBrush(); //brush
                brush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
                newRectangle.Fill = brush; //fill color
                newRectangle.Name = "Rectangle"; //attach name
                Canvas.SetLeft(newRectangle, location.x);
                Canvas.SetTop(newRectangle, location.y);
                //add new to drawn
                invoker.drawnElements.Add(newRectangle); //1+
                //add undo
                invoker.unmovedElements.Add(newRectangle); //3b+
                returnelement = newRectangle;
            }
            else if (element.Name == "Ellipse")
            {
                Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
                newEllipse.AccessKey = invoker.executer.ToString();
                newEllipse.Width = location.width; //set width
                newEllipse.Height = location.height; //set height 
                SolidColorBrush brush = new SolidColorBrush();//brush
                brush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
                newEllipse.Fill = brush;//fill color
                newEllipse.Name = "Ellipse";//attach name
                Canvas.SetLeft(newEllipse, location.x);//set left position
                Canvas.SetTop(newEllipse, location.y);//set top position
                //add new to drawn
                invoker.drawnElements.Add(newEllipse); //1+
                //add undo
                invoker.unmovedElements.Add(newEllipse); //3b+
                returnelement = newEllipse;
            }
            return returnelement;
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


        //check if element is already in group
        public int CheckInGroup(Invoker invoker, FrameworkElement element)
        {
            int counter = 0;
            foreach (Group group in invoker.drawnGroups)
            {
                if (group.drawnElements.Count() > 0)
                {
                    foreach (FrameworkElement groupelement in group.drawnElements)
                    {
                        if (groupelement.AccessKey == element.AccessKey)
                        {
                            counter++;
                        }
                    }
                }
            }
            return counter;
        }

        //
        //saving
        //
        public async void Saving(Canvas paintSurface, Invoker invoker)
        {

            try
            {
                string lines = "";
                //ungrouped and drawn
                foreach (FrameworkElement child in paintSurface.Children)
                {
                    int elmcheck = CheckInGroup(invoker, child); //see if already in group
                    if (elmcheck == 0)
                    {
                        if (child is Rectangle)
                        {
                            //ornament
                            //if (child.ornament.Text.Length > 5 && child.ornamentPosition.Length > 1)
                            //{
                            //    string ostr = "ornament " + child.ornamentPosition + " " + child.ornament.Text;
                            //}
                            //element
                            double top = (double)child.GetValue(Canvas.TopProperty);
                            double left = (double)child.GetValue(Canvas.LeftProperty);
                            string str = "rectangle " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                            lines += str;
                        }
                        else if (child is Ellipse)
                        {
                            //ornament
                            //if (child.ornament.Text.Length > 5 && child.ornamentPosition.Length > 1)
                            //{
                            //    string ostr = "ornament " + child.ornamentPosition + " " + child.ornament.Text;
                            //}
                            //element
                            double top = (double)child.GetValue(Canvas.TopProperty);
                            double left = (double)child.GetValue(Canvas.LeftProperty);
                            string str = "ellipse " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                            lines += str;
                        }
                    }
                }
                //grouped and drawn
                foreach (Group group in invoker.drawnGroups)
                {
                    //ornament
                    if (group.ornament.Text.Length > 5 && group.ornamentPosition.Length > 1)
                    {
                        string ostr = "ornament " + group.ornamentPosition + " " + group.ornament.Text;
                    }
                    //group
                    string gstr = group.Display(0, group);
                    lines += gstr;
                }
                //create and write to file
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("dp5data.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, lines);
            }
            //file errors
            catch (System.IO.FileNotFoundException)
            {
                fileText = "File not found.";
            }
            catch (System.IO.FileLoadException)
            {
                fileText = "File Failed to load.";
            }
            catch (System.IO.IOException e)
            {
                fileText = "File IO error " + e;
            }
            catch (Exception err)
            {
                fileText = err.Message;
            }

        }

        //
        //loading
        //
        public async void Loading(Canvas paintSurface, Invoker invoker)
        {
            //clear previous canvas
            paintSurface.Children.Clear();
            //clear invoker
            invoker.drawnElements.Clear();
            invoker.removedElements.Clear();
            invoker.unmovedElements.Clear();
            invoker.movedElements.Clear();
            invoker.selectElements.Clear();
            invoker.unselectElements.Clear();
            invoker.undoElements.Clear();
            invoker.redoElements.Clear();
            invoker.drawnGroups.Clear();
            invoker.removedGroups.Clear();
            invoker.movedGroups.Clear();
            invoker.unmovedGroups.Clear();
            invoker.selectedGroups.Clear();
            invoker.unselectedGroups.Clear();
            invoker.undoGroups.Clear();
            invoker.redoGroups.Clear();
            invoker.executer = 0;
            invoker.counter = 0;
            //read file
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile saveFile = await storageFolder.GetFileAsync("dp5data.txt");
            string text = await Windows.Storage.FileIO.ReadTextAsync(saveFile);
            //load shapes
            string[] readText = Regex.Split(text, "\\n+");
            int i = 0;
            //make groups and shapes
            foreach (string s in readText)
            {
                if (s.Length > 2)
                {
                    invoker.executer++;
                    i++;
                    string notabs = s.Replace("\t", "");
                    string[] line = Regex.Split(notabs, "\\s+");
                    //remake shapes
                    if (line[0] == "ellipse")
                    {
                        Shape shape = new Shape(Convert.ToDouble(line[1]), Convert.ToDouble(line[2]), Convert.ToDouble(line[3]), Convert.ToDouble(line[4]));
                        ICommand place = new MakeEllipses(shape, invoker, paintSurface);
                        invoker.Execute(place);
                    }
                    else if (line[0] == "rectangle")
                    {
                        Shape shape = new Shape(Convert.ToDouble(line[1]), Convert.ToDouble(line[2]), Convert.ToDouble(line[3]), Convert.ToDouble(line[4]));
                        ICommand place = new MakeRectangles(shape, invoker, paintSurface);
                        invoker.Execute(place);
                    }
                    //remake groups
                    else if (line[0] == "group")
                    {
                        FrameworkElement selectedElement = null;
                        Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
                        ICommand place = new MakeGroup(group, paintSurface, invoker);
                        invoker.Execute(place);
                    }
                }
            }
            //re add ornaments to elements
            int ec = invoker.drawnElements.Count();
            int gc = invoker.drawnGroups.Count();
            string ge = "";
            //foreach (string s in readText)
            for (int inc = readText.Count(); inc > 0; inc--)
            {
                string s = readText[inc];
                string notabs = s.Replace("\t", "");
                string[] line = Regex.Split(notabs, "\\s+");
                //remake shapes
                if (line[0] == "ellipse")
                {
                    ec--;
                    ge = "element";
                }
                else if (line[0] == "rectangle")
                {
                    ec--;
                    ge = "element";
                }
                else if (line[0] == "group")
                {
                    gc--;
                    ge = "group";
                    Group selectedgroup = invoker.drawnGroups[gc];
                    invoker.selectedGroups.Add(selectedgroup);
                }

            }
            //clear selected groups afterwards
            int rme = 0;
            foreach (Group group in invoker.drawnGroups)
            {
                invoker.selectedGroups.RemoveAt(rme);
                rme++;
            }

            int j = 0; //line increment
            int g = 0;//group increment
            //re add elements to groups
            foreach (string s in readText)
            {
                if (s.Length > 2)
                {
                    string notabs = s.Replace("\t", "");
                    string[] line = Regex.Split(notabs, "\\s+");
                    if (line[0] == "group")
                    {
                        GetGroupElements(readText, j, Convert.ToInt32(line[1]), g, invoker);
                        g++;
                    }
                    j++;
                }
            }
            int maingroup = 0; //main group increment
            int k = 0; //line increment
            //remake subgroups and add elements
            foreach (string s in readText)
            {
                if (s.Length > 2)
                {
                    string[] line = Regex.Split(s, "\\s+");
                    int tabcount = s.Length - s.Replace("/", "").Length;

                    //if (s[0] != '\t')
                    if (tabcount < 0)
                    {
                        if (line[0] == "group")
                        {
                            GetSubGroups(readText, maingroup, 0, k, k + Convert.ToInt32(line[1]), invoker);
                        }
                    }
                    maingroup++;
                }
                k++;
            }

        }

        //load ellipse
        public void GetEllipse(String lines, Canvas paintSurface, Invoker invoker)
        {
            string[] line = Regex.Split(lines, "\\s+");

            double x = Convert.ToDouble(line[1]);
            double y = Convert.ToDouble(line[2]);
            double width = Convert.ToDouble(line[3]);
            double height = Convert.ToDouble(line[4]);

            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.AccessKey = invoker.executer.ToString();
            newEllipse.Width = width;
            newEllipse.Height = height;
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Blue;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, x);//set left position
            Canvas.SetTop(newEllipse, y);//set top position
            paintSurface.Children.Add(newEllipse);
            invoker.drawnElements.Add(newEllipse);
        }

        //load rectangle
        public void GetRectangle(String lines, Canvas paintSurface, Invoker invoker)
        {
            string[] line = Regex.Split(lines, "\\s+");

            double x = Convert.ToDouble(line[1]);
            double y = Convert.ToDouble(line[2]);
            double width = Convert.ToDouble(line[3]);
            double height = Convert.ToDouble(line[4]);

            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.AccessKey = invoker.executer.ToString();
            newRectangle.Width = width; //set width
            newRectangle.Height = height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Blue; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, x); //set left position
            Canvas.SetTop(newRectangle, y); //set top position 
            paintSurface.Children.Add(newRectangle);
            invoker.drawnElements.Add(newRectangle);
        }

        //re attach element to group
        public void GetGroupElements(string[] readText, int start, int stop, int group, Invoker invoker)
        {
            Group attachgroup = invoker.drawnGroups[group];
            for (int i = start; i < stop; i++)
            {
                FrameworkElement elm = invoker.drawnElements[i];
                attachgroup.drawnElements.Add(elm);
            }

        }

        /*
        //get ornaments of elements
        public void GetElementsOrnaments(Canvas paintSurface, Invoker invoker)
        {

        }

        //get ornament of groups
        public void GetGroupsOrnaments(Canvas paintSurface, Invoker invoker)
        {

        }
        */

        //re attach subgroups to group
        public void GetSubGroups(string[] readText, int group, int depth, int start, int stop, Invoker invoker)
        {
            Group maingroup = invoker.drawnGroups[group];
            while (start < stop)
            {
                string s = readText[start];
                string notabs = s.Replace("\t", "");
                string tab = "\t";
                int tablength = tab.Length;
                int notablength = notabs.Length;
                int slength = s.Length;
                int subdepth = (slength - notablength) / tablength;

                if (subdepth == (depth + 1))
                {
                    string[] line = Regex.Split(notabs, "\\s+");
                    if (line[0] == "group")
                    {
                        group++;
                        Group subgroup = invoker.drawnGroups[group];
                        maingroup.addedGroups.Add(subgroup);
                        invoker.drawnGroups.RemoveAt(group);

                        GetSubGroups(readText, group, depth + 1, start, start + Convert.ToInt32(line[1]), invoker);
                    }
                    start++;
                }
            }
        }

    }

}

