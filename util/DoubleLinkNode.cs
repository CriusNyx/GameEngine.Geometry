using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.util
{

    public class DoubleLinkNode<T>
    {
        public T value;
        public DoubleLinkNode<T> next, previous;

        public DoubleLinkNode(T value = default(T), DoubleLinkNode<T> next = null, DoubleLinkNode<T> previous = null)
        {
            this.value = value;
            this.next = next;
            this.previous = previous;
        }
    }
}