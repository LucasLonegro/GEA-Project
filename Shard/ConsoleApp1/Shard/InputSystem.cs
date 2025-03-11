/*
 *
 *   Any game object interested in listening for input events will need to register itself
 *       with this manager.   It handles the informing of all listener objects when an
 *       event is raised.
 *   @author Michael Heron
 *   @version 1.0
 *
 */

using System;
using System.Collections.Generic;

namespace Shard
{
    internal class KeyMapping
    {
        private readonly Dictionary<int, int> map = new();

        public void setMapping(int key, int value)
        {
            map.Add(key, value);
        }

        public InputEvent getMapping(InputEvent key)
        {
            if (!map.ContainsKey(key.Key))
                return key;
            InputEvent mappedEvent = key.copyOf();
            mappedEvent.Key = map[key.Key];
            return mappedEvent;
        }

        public override string ToString()
        {
            return map.ToString();
        }
    }

    abstract class InputSystem
    {
        private readonly Dictionary<InputListener, KeyMapping> myListeners;

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

        public void setMapping(InputListener il, int eventTypeFrom, int eventTypeTo)
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
                InputEvent hold = il.Value.getMapping(ie);
                Console.WriteLine(hold.Key + " vs " + ie.Key);
                il.Key.handleInput(hold, eventType);
            }
        }

        public abstract void getInput();
    }
}