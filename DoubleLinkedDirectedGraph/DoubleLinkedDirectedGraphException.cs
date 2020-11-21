using System;
using System.Collections.Generic;
using System.Text;

namespace DoubleLinkedDirectedGraph
{
    public class DoubleLinkedDirectedGraphException:Exception
    {
        public DoubleLinkedDirectedGraphException(string message):base(message)
        { 
        }

        public DoubleLinkedDirectedGraphException(Exception ex, string message):base(message, ex)
        {
        }
    }
}
