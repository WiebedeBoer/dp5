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
    //interface command
    public interface ICommand
    {
        void Execute();
        void Undo();
        void Redo();
    }

    //class invoker
    public class Invoker
    {
        public List<ICommand> actionsList = new List<ICommand>();
        public List<ICommand> redoList = new List<ICommand>();

        public Invoker()
        {
            this.actionsList = new List<ICommand>();
            this.redoList = new List<ICommand>();

        }

        //execute
        public void Execute(ICommand cmd)
        {
            actionsList.Add(cmd);
            redoList.Clear();
            cmd.Execute();
        }

        //undo
        public void Undo()
        {
            if (actionsList.Count >= 1)
            {
                ICommand cmd = actionsList.Last();
                actionsList.RemoveAt(actionsList.Count - 1);
                redoList.Add(cmd);
                cmd.Undo();
            }
        }

        //redo
        public void Redo()
        {
            if (redoList.Count >= 1)
            {
                ICommand cmd = redoList.Last();
                actionsList.Add(cmd);
                redoList.RemoveAt(redoList.Count - 1);
                cmd.Redo();
                //repaint actions
                //foreach (ICommand icmd in actionsList)
                //{
                //    icmd.Execute();
                //}

            }
        }
    }

    //class make rectangle
    public class MakeRectangles : ICommand
    {
        private Shape shape;
        private Invoker invoker;
        private Canvas paintSurface;

        public MakeRectangles(Shape shape, Invoker invoker, Canvas paintSurface)
        {
            this.shape = shape;
            this.invoker = invoker;
            this.paintSurface = paintSurface;
        }

        public void Execute()
        {
            this.shape.makeRectangle(this.invoker, this.paintSurface);
        }

        public void Undo()
        {
            this.shape.remove(this.invoker, this.paintSurface);
        }

        public void Redo()
        {
            this.shape.makeRectangle(this.invoker, this.paintSurface);
        }
    }

    //class make ellipse
    public class MakeEllipses : ICommand
    {
        private Shape shape;
        private Invoker invoker;
        private Canvas paintSurface;

        public MakeEllipses(Shape shape, Invoker invoker, Canvas paintSurface)
        {
            this.shape = shape;
            this.invoker = invoker;
            this.paintSurface = paintSurface;
        }

        public void Execute()
        {
            this.shape.makeEllipse(this.invoker, this.paintSurface);
        }

        public void Undo()
        {
            this.shape.remove(this.invoker, this.paintSurface);
        }

        public void Redo()
        {
            this.shape.makeEllipse(this.invoker, this.paintSurface);
        }
    }

    //class moving
    public class Moving : ICommand
    {

        private PointerRoutedEventArgs e;
        private Shape shape;
        private Invoker invoker;
        private Canvas paintSurface;
        private FrameworkElement element;
        private Location location;

        public Moving(Shape shape, PointerRoutedEventArgs e, Canvas paintSurface, Invoker invoker, FrameworkElement element, Location location)
        {
            this.e = e;
            this.shape = shape;
            this.invoker = invoker;
            this.paintSurface = paintSurface;
            this.element = element;
            this.location = location;
        }

        public void Execute()
        {
            this.shape.moving(this.invoker, this.e, this.element, this.paintSurface, this.location);
        }

        public void Undo()
        {
            //this.shape.undoMoving(this.invoker, this.paintSurface, this.location);
            this.shape.undoMoving(this.invoker, this.paintSurface);
            //this.shape.remove(this.invoker, this.paintSurface);
            //this.shape.undoMoving();
            //this.shape.remove(this.invoker, this.paintSurface);
        }

        public void Redo()
        {
            //this.shape.redoMoving(this.paintSurface);
            //this.shape.moving(this.e, this.element, this.paintSurface, this.location);
            //this.shape.redoMoving(this.e, this.element, this.paintSurface);
            //this.shape.redoMoving();
            this.shape.redoMoving(this.invoker, this.paintSurface);
            //this.shape.moving(this.invoker, this.e, this.element, this.paintSurface, this.location);
        }
    }

    //class resize
    public class Resize : ICommand
    {

        private PointerRoutedEventArgs e;
        private Shape shape;
        private Invoker invoker;
        private Canvas paintSurface;
        private FrameworkElement element;
        private Location location;

        public Resize(Shape shape, PointerRoutedEventArgs e, Canvas paintSurface, Invoker invoker, FrameworkElement element, Location location)
        {

            this.e = e;
            this.shape = shape;
            this.invoker = invoker;
            this.paintSurface = paintSurface;
            this.element = element;
            this.location = location;
        }

        public void Execute()
        {
            this.shape.resize(this.invoker, this.e, this.element, this.paintSurface, this.location);
        }

        public void Undo()
        {
            //this.shape.remove(this.invoker, this.paintSurface);
            //this.shape.remove(this.invoker, this.paintSurface);
            this.shape.undoResize(this.invoker, this.paintSurface);
        }

        public void Redo()
        {
            this.shape.redoResize(this.invoker, this.paintSurface);
            //this.shape.resize(this.invoker, this.e,this.element,this.paintSurface);
        }
    }

    //class select
    public class Select : ICommand
    {

        private PointerRoutedEventArgs e;
        private Shape shape;

        public Select(Shape shape, PointerRoutedEventArgs e)
        {

            this.e = e;
            this.shape = shape;
        }

        public void Execute()
        {
            this.shape.select(this.e);
        }

        public void Undo()
        {
            this.shape.deselect(this.e);
        }

        public void Redo()
        {
            this.shape.select(this.e);
        }
    }

    //class deselect
    public class Deselect : ICommand
    {

        private PointerRoutedEventArgs e;
        private Shape shape;

        public Deselect(Shape shape, PointerRoutedEventArgs e)
        {

            this.e = e;
            this.shape = shape;
        }

        public void Execute()
        {
            this.shape.deselect(this.e);
        }

        public void Undo()
        {
            this.shape.select(this.e);
        }

        public void Redo()
        {
            this.shape.deselect(this.e);
        }
    }

    //class saving
    public class Saved : ICommand
    {
        private Shape mycommand;
        private Canvas paintSurface;

        public Saved(Shape mycommand, Canvas paintSurface)
        {
            this.mycommand = mycommand;
            this.paintSurface = paintSurface;
        }

        public void Execute()
        {
            this.mycommand.saving(paintSurface);
        }

        public void Undo()
        {
            this.paintSurface.Children.Clear();
        }

        public void Redo()
        {
            this.paintSurface.Children.Clear();
        }
    }

    //class load
    public class Loaded : ICommand
    {
        private Shape mycommand;
        private Canvas paintSurface;

        public Loaded(Shape mycommand, Canvas paintSurface)
        {
            this.mycommand = mycommand;
            this.paintSurface = paintSurface;
        }

        public void Execute()
        {
            this.mycommand.loading(this.paintSurface);
        }

        public void Undo()
        {
            this.paintSurface.Children.Clear();
        }

        public void Redo()
        {
            this.paintSurface.Children.Clear();
        }
    }

}
