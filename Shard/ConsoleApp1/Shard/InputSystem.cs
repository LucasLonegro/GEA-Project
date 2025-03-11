/*
 *
 *   Any game object interested in listening for input events will need to register itself
 *       with this manager.   It handles the informing of all listener objects when an
 *       event is raised.
 *   @author Michael Heron
 *   @version 1.0
 *
 */

using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;

namespace Shard
{
    internal class KeyMapping
    {
        private Dictionary<string, string> map = new Dictionary<string, string>();

        public void setMapping(string key, string value)
        {
            map.Add(key, value);
        }

        public string getMapping(string key)
        {
            if (map.TryGetValue(key, out string value) && value != null)
                return value;
            return key;
        }
    }

    abstract class InputSystem
    {
        private Dictionary<InputListener, KeyMapping> myListeners;

        public virtual void initialize()
        {
        }

        public InputSystem()
        {
            myListeners = new Dictionary<InputListener, KeyMapping>();
        }

        public void addListener(InputListener il)
        {
            if (myListeners.ContainsKey(il) == false)
            {
                myListeners.Add(il, new KeyMapping());
            }
        }

        public void setMapping(InputListener il, string eventTypeFrom, string eventTypeTo)
        {
            myListeners[il].setMapping(eventTypeFrom, eventTypeTo);
        }

        public void removeListener(InputListener il)
        {
            myListeners.Remove(il);
        }

        public void informListeners(InputEvent ie, string eventType)
        {
            foreach (var il in myListeners)
            {
                il.Key.handleInput(ie, il.Value.getMapping(eventType));
            }
        }

        public abstract void getInput();
    }
}