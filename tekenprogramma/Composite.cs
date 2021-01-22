using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tekenprogramma
{
    class Composite
    {
        public double height;
        public double width;
        public double left;
        public double top;
        public string type;
        public int id;
        public List<Composite> groupitems;

        public Composite(double height, double width, double left, double top, string type, int id)
        {
            this.height = height;
            this.width = width;
            this.left = left;
            this.top = top;
            this.type = type;
            this.id = id;
        }

        public void Add(Composite g)
        {
            groupitems.Add(g);
        }

        public void Remove(Composite g)
        {
            groupitems.Remove(g);
        }


        public List<Composite> getComposites()
        {    
            return groupitems;
        }
    }
}
