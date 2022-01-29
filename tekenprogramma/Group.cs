using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Input;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace tekenprogramma
{

    //location class
    public class Location
    {
        public double x;
        public double y;
        public double width;
        public double height;

        public Location(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Location()
        {

        }
    }

    //class group
    public class Group : Baseshape
    {
        public double height;
        public double width;
        public double x;
        public double y;
        public string type;
        public int depth;
        public int id;
        //public List<Baseshape> groupitems;
        public Canvas selectedCanvas;

        //public List<Baseshape> children = new List<Baseshape>();

        //public List<FrameworkElement> selectedElements = new List<FrameworkElement>();
        //public List<FrameworkElement> unselectedElements = new List<FrameworkElement>();

        public List<FrameworkElement> drawnElements = new List<FrameworkElement>();
        public List<FrameworkElement> removedElements = new List<FrameworkElement>();
        public List<FrameworkElement> movedElements = new List<FrameworkElement>();

        public List<Group> addedGroups = new List<Group>();
        public List<Group> movedGroups = new List<Group>();

        public List<IComponent> drawnComponents = new List<IComponent>();
        public List<IComponent> removedComponents = new List<IComponent>();
        public List<IComponent> movedComponents = new List<IComponent>();

        public Invoker invoker;
        public FrameworkElement element;
        public Canvas lastCanvas;

        public TextBlock ornament = new TextBlock();
        public string ornamentPosition = null;
        public List<TextBlock> ornaments = new List<TextBlock>();
        public List<TextBlock> removedOrnaments = new List<TextBlock>();
        public List<string> ornamentPositions = new List<string>();
        public List<string> removedOrnamentPositions = new List<string>();

        public Group(double height, double width, double x, double y, string type, int depth, int id, Canvas selectedCanvas, Invoker invoker, FrameworkElement element) : base(height, width, x, y)
        {
            this.height = height;
            this.width = width;
            this.x = x;
            this.y = y;
            this.type = type;
            this.depth = depth;
            this.id = id;
            this.selectedCanvas = selectedCanvas;
            this.invoker = invoker;
            this.element = element;
        }

        //make new group
        public void MakeGroup(Group group, Canvas selectedCanvas, Invoker invoker)
        {
            if (invoker.selectElements.Count() > 0)
            {
                //get selected elements
                foreach (FrameworkElement elm in invoker.selectElements)
                //for (int index = 0; index < invoker.selectElementsList.Count(); index++)
                {
                    //if (invoker.selectElementsList.Count() > 0)
                    //{
                    //FrameworkElement elm = invoker.selectElementsList[index];
                    elm.Opacity = 0.9;
                    //check if selected is not already grouped element
                    int elmcheck = CheckInGroup(invoker, elm);
                    if (elmcheck == 0)
                    {
                        this.drawnElements.Add(elm);
                        //add components
                        if (elm.Name == "Rectangle")
                        {
                            Strategy component = ConcreteComponentRectangle.GetInstance();
                            //IComponent rectangle = new ConcreteComponentRectangle(elm.ActualOffset.X, elm.ActualOffset.Y, elm.Width, elm.Height);
                            //this.drawnComponents.Add(rectangle);
                            this.drawnComponents.Add(component);
                        }
                        else if (elm.Name == "Ellipse")
                        {
                            Strategy component = ConcreteComponentEllipse.GetInstance();
                            //IComponent ellipse = new ConcreteComponentEllipse(elm.ActualOffset.X, elm.ActualOffset.Y, elm.Width, elm.Height);
                            //this.drawnComponents.Add(ellipse);
                            this.drawnComponents.Add(component);
                        }

                    }
                    //remove selected
                    invoker.unselectElements.Add(elm);
                    //invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
                    //}


                }
                invoker.selectElements.Clear();
            }
            if (invoker.selectedGroups.Count() > 0)
            {
                //get selected groups
                foreach (Group selectedgroup in invoker.selectedGroups)
                {
                    this.addedGroups.Add(selectedgroup);
                    //remove selected
                    invoker.unselectedGroups.Add(selectedgroup);
                    //invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
                    SelectedGroup(selectedgroup, invoker); //remove from drawn groups
                }
                invoker.selectedGroups.Clear();
            }
            invoker.drawnGroups.Add(this);
            this.id = invoker.executer; //id

        }

        //removed selected group from drawn elements
        public void SelectedGroup(Group group, Invoker invoker)
        {
            int key = group.id;
            int inc = 0;
            int number = 0;
            foreach (Group drawn in invoker.drawnGroups)
            {
                if (drawn.id == key)
                {
                    number = inc;
                }
                inc++;
            }
            invoker.drawnGroups.RemoveAt(number);
        }

        //un group
        public void UnGroup(Canvas selectedCanvas, Invoker invoker)
        {
            Group lastgroup = invoker.drawnGroups.Last();
            invoker.drawnGroups.RemoveAt(invoker.drawnGroups.Count() - 1);
            if (lastgroup.drawnElements.Count() > 0)
            {
                //get elements
                foreach (FrameworkElement elm in lastgroup.drawnElements)
                {
                    //add selected
                    invoker.selectElements.Add(elm);
                    if (invoker.unselectElements.Count() > 0)
                    {
                        invoker.unselectElements.RemoveAt(invoker.unselectElements.Count() - 1);
                    }
                    //elm.Opacity = 0.5;
                }
            }
            if (lastgroup.addedGroups.Count() > 0)
            {
                //get groups
                foreach (Group selectedgroup in lastgroup.addedGroups)
                {
                    //add selected
                    invoker.selectedGroups.Add(selectedgroup);
                    if (invoker.unselectedGroups.Count() > 0)
                    {
                        invoker.unselectedGroups.RemoveAt(invoker.unselectedGroups.Count() - 1);
                    }
                    invoker.drawnGroups.Add(selectedgroup); //re add to drawn
                }
            }
            invoker.removedGroups.Add(lastgroup);
        }

        //re group
        public void ReGroup(Canvas selectedCanvas, Invoker invoker)
        {
            Group lastgroup = invoker.removedGroups.Last();

            if (lastgroup.drawnElements.Count() > 0)
            {
                //get elements
                foreach (FrameworkElement elm in lastgroup.drawnElements)
                {
                    //remove selected
                    invoker.unselectElements.Add(elm);
                    invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1);
                }
            }
            if (lastgroup.addedGroups.Count() > 0)
            {
                //get groups
                foreach (Group selectedgroup in lastgroup.addedGroups)
                {
                    //remove selected
                    invoker.unselectedGroups.Add(selectedgroup);
                    invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
                    invoker.drawnGroups.RemoveAt(invoker.drawnGroups.Count() - 1);
                }
            }
            invoker.drawnGroups.Add(lastgroup);
            invoker.removedGroups.RemoveAt(invoker.removedGroups.Count() - 1);
        }

        //
        //moving
        //



        //
        //undo redo move resize
        //

        //undo moving or resizing
        public void Undo(Invoker invoker, Canvas paintSurface)
        {
            //Group selectedgroup = invoker.removedGroups.Last();
            //invoker.movedGroups.RemoveAt(invoker.movedGroups.Count() - 1);
            //invoker.removedGroups.Add(selectedgroup);
            //invoker.selectedGroups.Add(selectedgroup); //re add to selected

            //shuffle unselected
            Group prevelement = invoker.movedGroups.Last();
            invoker.unselectedGroups.RemoveAt(invoker.unselectedGroups.Count() - 1); //2b-
            invoker.selectedGroups.Add(prevelement); //2a+
            //shuffle moved
            Group nextelement = invoker.unmovedGroups.Last(); //err
            invoker.movedGroups.RemoveAt(invoker.movedGroups.Count() - 1); //3a-
            invoker.unmovedGroups.RemoveAt(invoker.unmovedGroups.Count() - 1); //3b-
            //add redo
            invoker.undoGroups.Add(nextelement); //4a+
            invoker.redoGroups.Add(prevelement); //4b+
            //remove and add to drawn
            //KeyNumber(nextelement, invoker); //1-
            invoker.drawnGroups.Add(prevelement); //1+  

            if (prevelement.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in prevelement.drawnElements)
                {
                    invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
                    //invoker.removedElements.Add(movedElement);
                }
            }
            if (prevelement.movedGroups.Count() > 0)
            {
                foreach (Group subgroup in prevelement.movedGroups)
                {
                    prevelement.SubUndo(subgroup, invoker);
                }
            }
            if (prevelement.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement removedElement in prevelement.drawnElements)
                {
                    //invoker.movedElements.RemoveAt(invoker.movedElements.Count() - 1);
                    invoker.drawnElements.Add(removedElement);
                }
            }
            Repaint(invoker, paintSurface); //repaint   
        }

        public void SubUndo(Group selectedgroup, Invoker invoker)
        {
            if (selectedgroup.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in selectedgroup.drawnElements)
                {
                    invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
                    //invoker.removedElements.Add(movedElement);
                }
            }
            if (selectedgroup.movedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.movedGroups)
                {
                    selectedgroup.SubUndo(subgroup, invoker);
                }
            }
            if (selectedgroup.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement removedElement in selectedgroup.drawnElements)
                {
                    //invoker.movedElements.RemoveAt(invoker.movedElements.Count() - 1);
                    invoker.drawnElements.Add(removedElement);
                }
            }
        }

        //redo moving or resizing
        public void Redo(Invoker invoker, Canvas paintSurface)
        {
            //Group selectedgroup = invoker.removedGroups.Last();
            //invoker.removedGroups.RemoveAt(invoker.removedGroups.Count() - 1);
            //invoker.movedGroups.Add(selectedgroup);
            //invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1); //remove selected

            //shuffle selected
            Group nextelement = invoker.undoGroups.Last();
            invoker.unselectedGroups.Add(nextelement); //2b+
            invoker.selectElements.RemoveAt(invoker.selectElements.Count() - 1); //2a-
            //shuffle moved
            Group prevelement = invoker.redoGroups.Last();
            invoker.movedGroups.Add(prevelement); //3a+
            invoker.unmovedGroups.Add(nextelement); //3b+
            //undo redo
            invoker.undoGroups.RemoveAt(invoker.undoGroups.Count() - 1); ; //4a-
            invoker.redoGroups.RemoveAt(invoker.redoGroups.Count() - 1); ; //4b-
            //remove and add to drawn
            //KeyNumber(prevelement, invoker); //1-
            invoker.drawnGroups.Add(nextelement); //1+

            if (nextelement.movedElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in nextelement.movedElements)
                {
                    invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
                    //invoker.removedElements.Add(movedElement);
                    //invoker.movedElements.Add(movedElement);              
                }
            }
            if (nextelement.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in nextelement.addedGroups)
                {
                    nextelement.SubRedo(subgroup, invoker);
                }
            }
            if (nextelement.movedElements.Count() > 0)
            {
                foreach (FrameworkElement removedElement in nextelement.movedElements)
                //foreach (FrameworkElement removedElement in selectedgroup.removedElements)
                {
                    //invoker.removedElements.RemoveAt(invoker.removedElements.Count() - 1);
                    invoker.drawnElements.Add(removedElement);
                    //invoker.movedElements.Add(removedElement);
                }
            }
            Repaint(invoker, paintSurface); //repaint   
        }

        public void SubRedo(Group selectedgroup, Invoker invoker)
        {
            if (selectedgroup.movedElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in selectedgroup.movedElements)
                {
                    invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
                    //invoker.removedElements.Add(movedElement);
                    //invoker.movedElements.Add(movedElement);
                }
            }
            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    selectedgroup.SubRedo(subgroup, invoker);
                }
            }
            if (selectedgroup.movedElements.Count() > 0)
            {
                foreach (FrameworkElement removedElement in selectedgroup.movedElements)
                //foreach (FrameworkElement removedElement in selectedgroup.removedElements)
                {
                    //invoker.removedElements.RemoveAt(invoker.removedElements.Count() - 1);
                    invoker.drawnElements.Add(removedElement);
                    //invoker.movedElements.Add(removedElement);
                }
            }
        }

        //
        //resizing
        //



        //
        //other
        //

        //repaint
        public void Repaint(Invoker invoker, Canvas paintSurface)
        {
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                paintSurface.Children.Add(drawelement); //add
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
                CheckInSubgroup(group, element.AccessKey);
            }
            return counter;
        }

        public int CheckInSubgroup(Group group, string key)
        {
            int counter = 0;
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
            if (group.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    counter = subgroup.CheckInSubgroup(subgroup, key);
                }
            }
            return counter;
        }

        //see if element is in group and select the group
        public void SelectInGroup(FrameworkElement selectedElement, Invoker invoker)
        {
            string key = selectedElement.AccessKey;
            if (invoker.drawnGroups.Count() > 0)
            {
                foreach (Group group in invoker.drawnGroups)
                {
                    if (group.drawnElements.Count() > 0)
                    {
                        foreach (FrameworkElement drawn in group.drawnElements)
                        {
                            if (drawn.AccessKey == key)
                            {
                                invoker.selectedGroups.Add(group);
                                ////remove selected element from list if in group
                                //if (invoker.selectElementsList.Count() >0)
                                //{
                                //    invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
                                //}                         
                            }
                        }
                    }
                    SelectInGroupHandle(invoker, key, group, group); //subgroup recursive
                }
            }
        }

        //recursively see if element is in subgroup and select the group
        public void SelectInGroupHandle(Invoker invoker, string key, Group group, Group checkgroup)
        {
            if (checkgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in checkgroup.addedGroups)
                {
                    if (subgroup.drawnElements.Count() > 0)
                    {
                        foreach (FrameworkElement drawn in subgroup.drawnElements)
                        {
                            if (drawn.AccessKey == key)
                            {
                                invoker.selectedGroups.Add(group);
                            }
                        }
                    }

                    if (subgroup.addedGroups.Count() > 0)
                    {
                        subgroup.SelectInGroupHandle(invoker, key, group, subgroup);
                    }

                }
            }
        }

        //set group unselected
        public void UnselectGroup(FrameworkElement selectedElement, Invoker invoker)
        {
            string key = selectedElement.AccessKey;
            if (invoker.drawnGroups.Count() > 0)
            {
                foreach (Group group in invoker.drawnGroups)
                {
                    if (group.drawnElements.Count() > 0)
                    {
                        foreach (FrameworkElement drawn in group.drawnElements)
                        {
                            if (drawn.AccessKey == key)
                            {
                                if(invoker.selectedGroups.Count() > 0)
                                {
                                    invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
                                }
                                    
                            }
                        }
                    }
                    UnselectGroupHandle(invoker, key, group); //subgroup recursive
                }
            }
        }

        //set subgroup unselected
        public void UnselectGroupHandle(Invoker invoker, string key, Group group)
        {
            if (group.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    subgroup.SelectInGroupHandle(invoker, key, group, group);
                    foreach (FrameworkElement drawn in subgroup.drawnElements)
                    {
                        if (drawn.AccessKey == key)
                        {
                            if (invoker.selectedGroups.Count() >0)
                            {
                                invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
                            }
                            
                        }
                    }
                }
            }
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

        //give smallest number
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

        //moving element in group
        public FrameworkElement MovingElement(FrameworkElement element, Invoker invoker, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            KeyNumber(element, invoker); //move selected at removed
            //create at new location
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
                invoker.drawnElements.Add(newRectangle);
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
                invoker.drawnElements.Add(newEllipse);
                returnelement = newEllipse;
            }
            return returnelement;
        }

        //resizing element in group
        public FrameworkElement ResizingElement(FrameworkElement element, Invoker invoker, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            KeyNumber(element, invoker); //move selected at removed
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
                invoker.drawnElements.Add(newRectangle);
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
                invoker.drawnElements.Add(newEllipse);
                returnelement = newEllipse;
            }
            return returnelement;
        }

        //display lines for saving
        public string Display(int depth, Group group)
        {
            //Display group.
            string str = "";
            //Add group.
            int i = 0;
            while (i < depth)
            {
                str += "\t";
            }
            int groupcount = group.drawnElements.Count() + group.addedGroups.Count();
            str = str + "group " + groupcount + "\n";

            //Recursively display child nodes.
            depth = depth + 1; //add depth tab
            if (group.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement child in group.drawnElements)
                {
                    if (child is Rectangle)
                    {
                        int j = 0;
                        while (j < depth)
                        {
                            str += "\t";
                            j++;
                        }
                        str = str + "rectangle " + child.ActualOffset.X + " " + child.ActualOffset.Y + " " + child.Width + " " + child.Height + "\n";
                    }
                    //else if (child is Ellipse)
                    else
                    {
                        int j = 0;
                        while (j < depth)
                        {
                            str += "\t";
                            j++;
                        }
                        str = str + "ellipse " + child.ActualOffset.X + " " + child.ActualOffset.Y + " " + child.Width + " " + child.Height + "\n";
                    }
                }
            }
            if (group.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    subgroup.Display(depth + 1, subgroup);
                }
            }
            return str;
        }


        public void LoadGroup(Group grouping, Canvas paintSurface, Invoker invoker, int linenumber, int start, int stop, string text)
        {
            string[] readText = Regex.Split(text, "\\n+");
            FrameworkElement elm = null;
            int i = start;
            while (i < stop)
            {
                string s = readText[i];
                string[] line = Regex.Split(s, "\\s+");
                if (line[0] == "ellipse")
                {
                    i++;
                    GetEllipse(s, paintSurface, invoker);
                    elm = invoker.drawnElements.Last();
                    grouping.drawnElements.Add(elm);
                }
                else if (line[0] == "rectangle")
                {
                    i++;
                    GetRectangle(s, paintSurface, invoker);
                    elm = invoker.drawnElements.Last();
                    grouping.drawnElements.Add(elm);
                }
                else if (line[0] == "group")
                {
                    i++;
                    invoker.executer++;
                    Group subgrouping = new Group(0, 0, 0, 0, "group", 0, invoker.executer, paintSurface, invoker, elm);
                    LoadGroup(subgrouping, paintSurface, invoker, Convert.ToInt32(line[1]), i, i + Convert.ToInt32(line[1]), text);
                    grouping.addedGroups.Add(subgrouping);
                }
            }
            invoker.drawnGroups.Add(grouping);
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



    }
}