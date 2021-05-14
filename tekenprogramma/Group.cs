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

        public Canvas selectedCanvas;

        public List<FrameworkElement> drawnElements = new List<FrameworkElement>();
        public List<FrameworkElement> removedElements = new List<FrameworkElement>();
        public List<FrameworkElement> movedElements = new List<FrameworkElement>();

        public List<IComponent> drawnComponents = new List<IComponent>();
        public List<IComponent> removedComponents = new List<IComponent>();
        public List<IComponent> movedComponents = new List<IComponent>();

        public List<Group> addedGroups = new List<Group>();
        public List<Group> movedGroups = new List<Group>();

        public Invoker invoker;
        public FrameworkElement element;
        public Canvas lastCanvas;

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
            if (invoker.selectElementsList.Count() > 0)
            {
                //get selected elements
                foreach (FrameworkElement elm in invoker.selectElementsList)
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
                            IComponent rectangle = new ConcreteComponentRectangle(elm.ActualOffset.X, elm.ActualOffset.Y, elm.Width, elm.Height);
                            this.drawnComponents.Add(rectangle);
                        }
                        else if (elm.Name == "Ellipse")
                        {
                            IComponent ellipse = new ConcreteComponentEllipse(elm.ActualOffset.X, elm.ActualOffset.Y, elm.Width, elm.Height);
                            this.drawnComponents.Add(ellipse);
                        }

                    }
                    //remove selected
                    invoker.unselectElementsList.Add(elm);
                    //invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
                    //}


                }
                invoker.selectElementsList.Clear();
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
                    invoker.selectElementsList.Add(elm);
                    if (invoker.unselectElementsList.Count() > 0)
                    {
                        invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count() - 1);
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
                    invoker.unselectElementsList.Add(elm);
                    invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
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
            Group selectedgroup = invoker.removedGroups.Last();
            invoker.movedGroups.RemoveAt(invoker.movedGroups.Count() - 1);
            invoker.removedGroups.Add(selectedgroup);
            invoker.selectedGroups.Add(selectedgroup); //re add to selected

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
            Group selectedgroup = invoker.removedGroups.Last();
            invoker.removedGroups.RemoveAt(invoker.removedGroups.Count() - 1);
            invoker.movedGroups.Add(selectedgroup);
            invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1); //remove selected

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
                    SelectInGroupHandle(invoker, key, group); //subgroup recursive
                }
            }
        }

        //recursively see if element is in subgroup and select the group
        public void SelectInGroupHandle(Invoker invoker, string key, Group group)
        {
            if (group.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    subgroup.SelectInGroupHandle(invoker, key, group);
                    foreach (FrameworkElement drawn in subgroup.drawnElements)
                    {
                        if (drawn.AccessKey == key)
                        {
                            invoker.selectedGroups.Add(group);
                        }
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
                                invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
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
                    subgroup.SelectInGroupHandle(invoker, key, group);
                    foreach (FrameworkElement drawn in subgroup.drawnElements)
                    {
                        if (drawn.AccessKey == key)
                        {
                            invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
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
    }
}