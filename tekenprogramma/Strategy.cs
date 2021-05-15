using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Input;
using System.Collections.Generic;

namespace tekenprogramma
{
    /*
    public interface IStrategy
    {
        FrameworkElement Execute(double left, double top, double width, double height, FrameworkElement g, bool selected, Invoker invoker);
    }
    */


    public abstract class Strategy : IComponent
    {
        public static Strategy strategy;

        public static Strategy GetInstance()
        {
            return strategy;
        }

        public abstract FrameworkElement Accept(IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement, Location location);
        public abstract string Write(IWriter visitor, FrameworkElement element, Canvas paintSurface);

        public abstract string ToString(double x, double y, double width, double height);

        public abstract FrameworkElement Execute(double left, double top, double width, double height, FrameworkElement g, bool selected, Invoker invoker);
    }


    /*
    //rectangle singleton
    public class RectangleStrategy : Strategy
    {
        public static Strategy strategy;

        private RectangleStrategy()
        {

        }

        // Prevents the strategy from instantiating multiple times
        public static Strategy GetInstance()
        {
            if (strategy == null)
            {
                strategy = new RectangleStrategy();
            }
            return strategy;
        }

        public override String ToString(double x, double y, double width, double height)
        {
            string str = "rectangle" + x + " " + y + " " + width + " " + height;
            return str;
        }

        public override FrameworkElement Execute(double x, double y, double width, double height, FrameworkElement g, bool selected, Invoker invoker)
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
            g = newRectangle;
            return g;
        }
    }

    //ellipse singleton
    public class EllipseStrategy : Strategy
    {
        public static Strategy strategy;

        private EllipseStrategy()
        {

        }

        // Prevents the strategy from instantiating multiple times
        public static Strategy GetInstance()
        {
            if (strategy == null)
            {
                strategy = new EllipseStrategy();
            }
            return strategy;
        }

        public override String ToString(double x, double y, double width, double height)
        {
            string str = "ellipse" + x + " " + y + " " + width + " " + height;
            return str;
        }

        public override FrameworkElement Execute(double x, double y, double width, double height, FrameworkElement g, bool selected, Invoker invoker)
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
            g = newEllipse;
            return g;
        }
    }

    public class Context
    {
        private Strategy strategy;

        public Context(Strategy strategy)
        {
            this.strategy = strategy;
        }

        //public void ExecuteStrategy(double x, double y, double width, double height, FrameworkElement g, bool selected, Invoker invoker)
        //{
        //    strategy.Execute(x, y, width, height, g, selected, invoker);
        //    //return strategy;
        //}

        //public FrameworkElement ExecuteStrategy(double x, double y, double width, double height, FrameworkElement g, bool selected, Invoker invoker)
        //{
        //    this.strategy = strategy.Execute(x, y, width, height, g, selected, invoker);
        //    return strategy;
        //}

        public FrameworkElement ExecuteStrategy(double x, double y, double width, double height, FrameworkElement g, bool selected, Invoker invoker)
        {
            return strategy.Execute(x, y, width, height, g, selected, invoker);
            //    return strategy;
        }
    }

    */

}

