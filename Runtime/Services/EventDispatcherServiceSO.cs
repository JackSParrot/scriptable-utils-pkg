using System;
using UnityEngine;
using System.Collections.Generic;

namespace JackSParrot.Utils
{
    [CreateAssetMenu(fileName = "New Event Dispatcher", menuName = "JackSParrot/Services/EventDispatcher", order = 1)]
    public class EventDispatcherServiceSO : Service
    {
        private class Listener
        {
            public Type EventType;
            public Delegate EventDelegate;
        }

        private readonly Dictionary<Type, List<Delegate>> _listeners = new Dictionary<Type, List<Delegate>>();
        private readonly List<Listener> _listenersToAdd = new List<Listener>();
        private readonly List<Listener> _listenersToRemove = new List<Listener>();
        private bool _processing = false;

        public static EventDispatcherServiceSO CreateInstance()
        {
            return CreateInstance<EventDispatcherServiceSO>();
        }

        public void AddListener<T>(Action<T> listener) where T : class
        {
            Listener evListener = new Listener { EventType = typeof(T), EventDelegate = listener };
            if (_processing)
            {
                _listenersToAdd.Add(evListener);
            }
            else
            {
                AddListenerInternal(evListener);
            }
        }

        public void RemoveListener<T>(Action<T> listener) where T : class
        {
            Listener evListener = new Listener { EventType = typeof(T), EventDelegate = listener };
            if (_processing)
            {
                _listenersToRemove.Add(evListener);
            }
            else
            {
                RemoveListenerInternal(evListener);
            }
        }

        public void Raise<T>() where T : class, new()
        {
            Raise(new T());
        }

        public void Raise<T>(T e) where T : class
        {
            if (_processing)
            {
                Debug.LogWarning("Triggered an event while processing a previous event");
            }

            Debug.Assert(e != null, "Raised a null event");
            Type type = e.GetType();
            if (!_listeners.TryGetValue(type, out List<Delegate> listeners))
            {
                Debug.Log("Raised event with no listeners");
                return;
            }

            _processing = true;
            listeners.RemoveAll(del => del == null);
            foreach (Delegate listener in listeners)
            {
                if (listener is Action<T> castedDelegate)
                {
                    castedDelegate(e);
                }
            }
            _processing = false;

            foreach (Listener listenerToAdd in _listenersToAdd)
            {
                AddListenerInternal(listenerToAdd);
            }
            _listenersToAdd.Clear();

            foreach (Listener listenerToRemove in _listenersToRemove)
            {
                RemoveListenerInternal(listenerToRemove);
            }
            _listenersToRemove.Clear();
        }

        public override void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            _listeners.Clear();
            _listenersToAdd.Clear();
            _listenersToRemove.Clear();
        }

        private void OnDisable()
        {
            Clear();
        }

        #region Internals
        private void AddListenerInternal(Listener listener)
        {
            Debug.Assert(listener != null, "Added a null listener.");
            if (!_listeners.TryGetValue(listener.EventType, out List<Delegate> delegateList))
            {
                delegateList = new List<Delegate>();
                _listeners[listener.EventType] = delegateList;
            }
            Debug.Assert(delegateList.Find(e => e == listener.EventDelegate) == null, "Added duplicated event listener to the event dispatcher.");
            delegateList.Add(listener.EventDelegate);
        }

        private void RemoveListenerInternal(Listener listener)
        {
            if (listener != null && _listeners.TryGetValue(listener.EventType, out List<Delegate> group))
            {
                group.RemoveAll(e => e == listener.EventDelegate);
            }
        }
#endregion
    }
}
