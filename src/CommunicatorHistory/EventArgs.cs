using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicatorHistory
{
    public class EventArgs<T> : EventArgs
    {
        public T EventData { get; private set; }

        public EventArgs(T eventData)
        {
            EventData = eventData;
        }
    }
}
