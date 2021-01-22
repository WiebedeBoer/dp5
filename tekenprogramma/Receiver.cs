using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tekenprogramma
{
    class Receiver
    {
        private List<ICommand> action = new List<ICommand>();

        public void takeOrder(ICommand order)
        {
            action.Add(order);
        }

        public void placeOrders()
        {

            foreach (ICommand order in action)
            {
                order.Execute();
            }
            action.Clear();
        }
    }
}
