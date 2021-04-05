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

        public abstract void execute(double left, double top, double width, double height, FrameworkElement g, bool selected);
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

        public override String toString()
        {
            return "rectangle";
        }

        public override void execute(double left, double top, double width, double height, FrameworkElement g, bool selected)
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

        public override String toString()
        {
            return "ellipse";
        }

        public override void execute(double left, double top, double width, double height, FrameworkElement g, bool selected)
        {

        }
    }
}
