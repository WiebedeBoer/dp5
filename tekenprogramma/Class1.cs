using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tekenprogramma
{
    public class Shape {
        public void Create()
        {


        }
    }

    class CreateRectangle
    {
        private Shape rectangle;
        private int leftCoord;
        private int topCoord;
        private int rightCoord;
        private int bottomCoord;
        private string actionType;

        public CreateRectangle(Shape rectangle)
        {
            this.rectangle = rectangle;
        }

        public void execute()
        {
            rectangle.Create();
        }

        public void OnPointerPressed()
        {
            this.actionType = "selected";
        }
    }

    class CreateElipse
    {
        private Shape elipse;
        private int leftCoord;
        private int topCoord;
        private int rightCoord;
        private int bottomCoord;
        private string actionType;

        public CreateElipse(Shape elipse)
        {
            this.elipse = elipse;
        }

        public void execute()
        {
            elipse.Create();
        }

        public void OnPointerPressed()
        {
            this.actionType = "selected";
        }
    }

    class CreateShape
    {
    }

}
