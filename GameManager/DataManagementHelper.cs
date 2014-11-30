using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager
{
    static class DataManagementHelper
    {

        private static List<EventHandler> globalEventHandlers = new List<EventHandler>();
        private static List<KeyValuePair<EventHandler, Delegate[]>> globalDelegates = new List<KeyValuePair<EventHandler, Delegate[]>>();

        public static void ClearAllEventHandlers()
        {

            GlobalEventHandlers.Clear();
        }

        public static void AddNewEventHandler(EventHandler e)
        {

            GlobalEventHandlers.Add(e);
            GlobalDelegates.Add(new KeyValuePair<EventHandler, Delegate[]>(e, e.GetInvocationList()));
        }

        public static List<KeyValuePair<EventHandler, Delegate[]>> GlobalDelegates
        {

            get
            {

                return globalDelegates;
            }

            set
            {

                if (value != globalDelegates)
                {

                    globalDelegates = value;
                }
            }
        }

        public static List<EventHandler> GlobalEventHandlers
        {

            get
            {

                return globalEventHandlers;
            }

            set
            {

                if (value != globalEventHandlers)
                {

                    globalEventHandlers = value;
                }
            }
        }
    }
}
