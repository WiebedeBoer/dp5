using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tekenprogramma
{
    class Group
    {
        public double height;
        public double width;
        public double x;
        public double y;
        public string type;
        public int id;
        public List<Group> groupitems;

        public Group(double height, double width, double x, double y, string type, int id)
        {
            this.height = height;
            this.width = width;
            this.x = x;
            this.y = y;
            this.type = type;
            this.id = id;
        }

        public void AddGroup(Group newgroup)
        {
            groupitems.Add(newgroup);
        }

        /*
        public Group FindID(int id)
        {
            for (int c = 0; c < groupitems.Count; c++)
            {
                if (groupitems[c].id == id)
                {
                    return groupitems[c];
                }
                else
                {
                    Group tmp = FindID(id);
                    if (tmp.id == id)
                    {
                        return tmp;
                    }
                }
            }
        }*/
    }
}
