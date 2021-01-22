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
    
    public abstract class Strategy
    {
        public static Strategy strategy;

        public static Strategy getInstance()
        {
            return strategy;
        }

        public abstract String toString();

        public abstract void execute(double left, double top, double width, double height, Shape g, boolean selected);
    }
    
    public class RectangleStrategy : Strategy
    {
        public static Strategy strategy;

        private RectangleStrategy()
        {

        }

        // Prevents the strategy from instantiating multiple times
        public static Strategy getInstance()
        {
            if (strategy == null)
            {
                strategy = new RectangleStrategy();
            }
            return strategy;
        }

        public String toString()
        {
            return "rectangle";
        }

        public void Execute(double left, double top, double width, double height, Rectangle g, boolean selected)
        {

        }
    }

    public class EllipseStrategy : Strategy
    {
        public static Strategy strategy;

        private EllipseStrategy()
        {

        }

        // Prevents the strategy from instantiating multiple times
        public static Strategy getInstance()
        {
            if (strategy == null)
            {
                strategy = new EllipseStrategy();
            }
            return strategy;
        }

        public String toString()
        {
            return "ellipse";
        }

        public void Execute(double left, double top, double width, double height, Ellipse g, boolean selected)
        {

        }
    }
}
