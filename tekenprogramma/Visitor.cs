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

    public interface IVisitor
    {
        void visitRectangle(Rectangle rectangle);
        void visitEllipse(Ellipse ellipse);
    }

    public class visitResize : IVisitor
    {

        public void visitRectangle(Rectangle rectangle)
        {
        }

        public void visitEllipse(Ellipse ellipse)
        {
        }
    }

    public class visitMove : IVisitor
    {

        public void visitRectangle(Rectangle rectangle)
        {
        }

        public void visitEllipse(Ellipse ellipse)
        {
        }
    }

    public class visitWrite : IVisitor
    {

        public void visitRectangle(Rectangle rectangle)
        {
        }

        public void visitEllipse(Ellipse ellipse)
        {
        }
    }
}
