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
            this.shape.MakeRectangle(this.invoker, this.paintSurface);
        }

        public void Undo()
        {
            this.shape.Remove(this.invoker, this.paintSurface);
        }

        public void Redo()
        {
            this.shape.Add(this.invoker, this.paintSurface);
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
            this.shape.MakeEllipse(this.invoker, this.paintSurface);

            //Context context = new Context(new EllipseStrategy());
            //Strategy ellipseStrategy = EllipseStrategy.GetInstance();
            //context.ExecuteStrategy(ExecuteStrategy(x, y, width, height, g, selected, invoker));
        }

        public void Undo()
        {
            this.shape.Remove(this.invoker, this.paintSurface);
        }

        public void Redo()
        {
            this.shape.Add(this.invoker, this.paintSurface);
        }
    }

    //class moving
    public class Moving : ICommand
    {
        private Shape shape;
        private Invoker invoker;
        private Canvas paintSurface;
        private FrameworkElement element;
        private Location location;

        public Moving(Shape shape, Invoker invoker, Location location, Canvas paintSurface, FrameworkElement element)
        {

            this.shape = shape;
            this.invoker = invoker;
            this.paintSurface = paintSurface;
            this.element = element;
            this.location = location;
        }

        public void Execute()
        {
            //this.shape.Moving(this.invoker, this.paintSurface, this.location, this.element);

            //MoveClient mover = new MoveClient();
            //IVisitor visitor = new ConcreteVisitorMove();
            //Group selectedgroup = this.invoker.selectedGroups.Last();
            //mover.Client(selectedgroup.drawnComponents, selectedgroup.drawnElements, selectedgroup, visitor, this.invoker, this.e, this.paintSurface, this.element);

            if (this.element.Name == "Rectangle")
            {
                IVisitor visitor = new ConcreteVisitorMove();
                ConcreteComponentRectangle component = new ConcreteComponentRectangle(this.location.x, this.location.y, this.location.width, this.location.height);
                visitor.VisitConcreteComponentRectangle(component, this.invoker, this.element, this.paintSurface, this.location);
            }
            else if (this.element.Name == "Ellipse")
            {
                IVisitor visitor = new ConcreteVisitorMove();
                ConcreteComponentEllipse component = new ConcreteComponentEllipse(this.location.x, this.location.y, this.location.width, this.location.height);
                visitor.VisitConcreteComponentEllipse(component, this.invoker, this.element, this.paintSurface, this.location);
            }
            this.shape.Repaint(this.invoker, this.paintSurface);
        }

        public void Undo()
        {
            this.shape.MoveBack(this.invoker, this.paintSurface);
        }

        public void Redo()
        {
            this.shape.MoveAgain(this.invoker, this.paintSurface);
        }
    }

    //class resize
    public class Resize : ICommand
    {
        private Shape shape;
        private Invoker invoker;
        private Canvas paintSurface;
        private FrameworkElement element;
        private Location location;
        private PointerRoutedEventArgs e;

        public Resize(Shape shape, Invoker invoker, PointerRoutedEventArgs e, Location location, Canvas paintSurface, FrameworkElement element)
        {
            this.shape = shape;
            this.invoker = invoker;
            this.paintSurface = paintSurface;
            this.element = element;
            this.location = location;
            this.e = e;
        }

        public void Execute()
        {
            //this.shape.Resize(this.invoker, this.e, this.paintSurface, this.element);

            //ResizeClient resizer = new ResizeClient();
            //IVisitor visitor = new ConcreteVisitorResize();
            //Group selectedgroup = this.invoker.selectedGroups.Last();
            //resizer.Client(selectedgroup.drawnComponents, selectedgroup.drawnElements, selectedgroup, visitor, this.invoker, this.e, this.paintSurface, this.element);

            if (this.element.Name == "Rectangle")
            {
                IVisitor visitor = new ConcreteVisitorResize();
                ConcreteComponentRectangle component = new ConcreteComponentRectangle(this.location.x, this.location.y, this.location.width, this.location.height);
                visitor.VisitConcreteComponentRectangle(component, this.invoker, this.element, this.paintSurface, this.location);
            }
            else if (this.element.Name == "Ellipse")
            {
                IVisitor visitor = new ConcreteVisitorResize();
                ConcreteComponentEllipse component = new ConcreteComponentEllipse(this.location.x, this.location.y, this.location.width, this.location.height);
                visitor.VisitConcreteComponentEllipse(component, this.invoker, this.element, this.paintSurface, this.location);
            }
            this.shape.Repaint(this.invoker, this.paintSurface);
        }

        public void Undo()
        {
            this.shape.MoveBack(this.invoker, this.paintSurface);
        }

        public void Redo()
        {
            this.shape.MoveAgain(this.invoker, this.paintSurface);
        }
    }

    //class select
    public class Select : ICommand
    {

        private PointerRoutedEventArgs e;
        private Shape shape;
        private Invoker invoker;
        private Canvas paintSurface;

        public Select(Shape shape, PointerRoutedEventArgs e, Invoker invoker, Canvas paintSurface)
        {
            this.e = e;
            this.shape = shape;
            this.invoker = invoker;
            this.paintSurface = paintSurface;
        }

        public void Execute()
        {
            this.shape.Select(this.invoker, this.e, this.paintSurface);
        }

        public void Undo()
        {
            this.shape.Deselect(this.invoker, this.e, this.paintSurface);
        }

        public void Redo()
        {
            this.shape.Reselect(this.invoker, this.e, this.paintSurface);
        }
    }




    //class saving
    public class Saved : ICommand
    {
        private Shape mycommand;
        private Canvas paintSurface;
        private Invoker invoker;

        public Saved(Shape mycommand, Canvas paintSurface, Invoker invoker)
        {
            this.mycommand = mycommand;
            this.paintSurface = paintSurface;
            this.invoker = invoker;
        }

        public void Execute()
        {
            //this.mycommand.Saving(paintSurface, invoker);
            WriteClient writer = new WriteClient();
            IWriter visitor = new ConcreteVisitorWrite();
            writer.Client(this.paintSurface, this.invoker, visitor);
        }

        public void Undo()
        {
            //this.paintSurface.Children.Clear();
        }

        public void Redo()
        {
            //this.paintSurface.Children.Clear();
        }
    }

    //class load
    public class Loaded : ICommand
    {
        private Shape mycommand;
        private Canvas paintSurface;
        private Invoker invoker;
        public Loaded(Shape mycommand, Canvas paintSurface, Invoker invoker)
        {
            this.mycommand = mycommand;
            this.paintSurface = paintSurface;
            this.invoker = invoker;
        }

        public void Execute()
        {
            this.mycommand.Loading(this.paintSurface, this.invoker);
        }

        public void Undo()
        {
            //this.paintSurface.Children.Clear();
        }

        public void Redo()
        {
            //this.paintSurface.Children.Clear();
        }
    }

    //class make group
    public class MakeGroup : ICommand
    {
        private Group mycommand;
        private Canvas selectedCanvas;
        private Invoker invoker;
        //private FrameworkElement element;

        //public MakeGroup(Group mycommand, Canvas selectedCanvas, Invoker invoker, FrameworkElement element)
        public MakeGroup(Group mycommand, Canvas selectedCanvas, Invoker invoker)
        {
            this.mycommand = mycommand;
            this.selectedCanvas = selectedCanvas;
            this.invoker = invoker;
            //this.element = element;
        }

        public void Execute()
        {
            this.mycommand.MakeGroup(this.mycommand, this.selectedCanvas, this.invoker);
        }

        public void Undo()
        {
            //this.mycommand.UnGroup(this.mycommand, this.selectedCanvas, this.invoker,this.element);
            this.mycommand.UnGroup(this.selectedCanvas, this.invoker);
        }

        public void Redo()
        {
            this.mycommand.ReGroup(this.selectedCanvas, this.invoker);
        }
    }

    //class resize group
    public class ResizeGroup : ICommand
    {
        private Group mycommand;
        private Canvas paintSurface;
        private Invoker invoker;
        private FrameworkElement element;
        private PointerRoutedEventArgs e;

        public ResizeGroup(Group mycommand, PointerRoutedEventArgs e, Canvas paintSurface, Invoker invoker, FrameworkElement element)
        {
            this.mycommand = mycommand;
            this.invoker = invoker;
            this.paintSurface = paintSurface;
            this.element = element;
            this.e = e;
        }

        public void Execute()
        {
            //this.mycommand.Resize(this.invoker, this.e, this.paintSurface, this.element);

            ResizeClient resizer = new ResizeClient();
            IVisitor visitor = new ConcreteVisitorResize();
            Group selectedgroup = this.invoker.selectedGroups.Last();
            resizer.Client(selectedgroup.drawnComponents, selectedgroup.drawnElements, selectedgroup, visitor, this.invoker, this.e, this.paintSurface, this.element);
        }

        public void Undo()
        {
            this.mycommand.Undo(this.invoker, this.paintSurface);
        }

        public void Redo()
        {
            this.mycommand.Redo(this.invoker, this.paintSurface);
        }
    }

    //class move group
    public class MoveGroup : ICommand
    {
        private Group mycommand;
        private Canvas paintSurface;
        private Invoker invoker;
        private FrameworkElement element;
        private PointerRoutedEventArgs e;

        public MoveGroup(Group mycommand, PointerRoutedEventArgs e, Canvas paintSurface, Invoker invoker, FrameworkElement element)
        {
            this.mycommand = mycommand;
            this.invoker = invoker;
            this.paintSurface = paintSurface;
            this.element = element;
            this.e = e;
        }

        public void Execute()
        {
            //this.mycommand.Moving(this.invoker, this.e, this.paintSurface, this.element);
            MoveClient mover = new MoveClient();
            IVisitor visitor = new ConcreteVisitorMove();
            Group selectedgroup = this.invoker.selectedGroups.Last();
            mover.Client(selectedgroup.drawnComponents, selectedgroup.drawnElements, selectedgroup, visitor, this.invoker, this.e, this.paintSurface, this.element);
        }

        public void Undo()
        {
            this.mycommand.Undo(this.invoker, this.paintSurface);
        }

        public void Redo()
        {
            this.mycommand.Redo(this.invoker, this.paintSurface);
        }
    }

    /*
    //class load group
    public class LoadGroup : ICommand
    {
        private Group mycommand;
        private Canvas selectedCanvas;
        private Invoker invoker;

        public LoadGroup(Group mycommand, Canvas selectedCanvas, Invoker invoker)
        {
            this.mycommand = mycommand;
            this.selectedCanvas = selectedCanvas;
            this.invoker = invoker;
        }

        public void Execute()
        {
            this.mycommand.LoadGroup(this.mycommand, this.selectedCanvas, this.invoker);
        }

        public void Undo()
        {
            this.mycommand.UnloadGroup(this.selectedCanvas, this.invoker);
        }

        public void Redo()
        {
            this.mycommand.ReloadGroup(this.selectedCanvas, this.invoker);
        }
    }
    */

    ////class select
    //public class Deselect : ICommand
    //{

    //    private PointerRoutedEventArgs e;
    //    private Shape shape;
    //    private Invoker invoker;

    //    public Deselect(Shape shape, PointerRoutedEventArgs e, Invoker invoker)
    //    {
    //        this.e = e;
    //        this.shape = shape;
    //    }

    //    public void Execute()
    //    {
    //        this.shape.Deselect(this.invoker, this.e);
    //    }

    //    public void Undo()
    //    {
    //        this.shape.Select(this.invoker, this.e);
    //    }

    //    public void Redo()
    //    {
    //        this.shape.Deselect(this.invoker, this.e);
    //    }
    //}

}
