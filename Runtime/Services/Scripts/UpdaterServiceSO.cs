using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JackSParrot.Utils
{
    [CreateAssetMenu(fileName = "New UpdaterService", menuName = "JackSParrot/Services/UpdaterService")]
    public class UpdaterServiceSO : Service
    {
        private class Updater : MonoBehaviour
        {
            public event Action<float> OnUpdate = (dt) => { };
            void Update()
            {
                OnUpdate(Time.deltaTime);
            }
        }

        [SerializeField] private UnityEvent<float> _updateEvent = new UnityEvent<float>();

        private readonly List<Action<float>> _registeredCallbacks = new List<Action<float>>();
        private readonly List<Action<float>> _callbacksToRegister = new List<Action<float>>();
        private readonly List<Action<float>> _callbacksToUnregister = new List<Action<float>>();
        private Updater _updater = null;

        public static UpdaterServiceSO CreateInstance()
        {
            return CreateInstance<UpdaterServiceSO>();
        }

        public void ScheduleUpdate(Action<float> updateCallback)
        {
            CheckUpdater();
            if (!_callbacksToRegister.Contains(updateCallback) && !_registeredCallbacks.Contains(updateCallback))
            {
                _callbacksToRegister.Add(updateCallback);
            }
        }

        public void UnscheduleUpdate(Action<float> updateCallback)
        {
            if (!_callbacksToUnregister.Contains(updateCallback))
            {
                _callbacksToUnregister.Add(updateCallback);
            }
        }

        public override void Dispose()
        {
            if (_updater != null)
            {
                _updater.OnUpdate -= InternalUpdate;
                Destroy(_updater.gameObject);
                _updater = null;
            }
            _registeredCallbacks.Clear();
            _callbacksToRegister.Clear();
            _callbacksToUnregister.Clear();
        }

        private void InternalUpdate(float dt)
        {
            for (int i = 0; i < _callbacksToUnregister.Count; ++i)
            {
                Action<float> current = _callbacksToUnregister[i];
                if (_registeredCallbacks.Contains(current))
                {
                    _registeredCallbacks.Remove(current);
                }
            }
            _callbacksToUnregister.Clear();
            for (int i = 0; i < _callbacksToRegister.Count; ++i)
            {
                _registeredCallbacks.Add(_callbacksToRegister[i]);
            }
            _callbacksToRegister.Clear();
            _registeredCallbacks.RemoveAll(m => m == null);
            for (int i = 0; i < _registeredCallbacks.Count; ++i)
            {
                _registeredCallbacks[i].Invoke(dt);
            }
            _updateEvent.Invoke(dt);
        }

        private void CheckUpdater()
        {
            if (_updater == null)
            {
                _updater = new GameObject("UpdateRunner").AddComponent<Updater>();
                DontDestroyOnLoad(_updater.gameObject);
                _updater.OnUpdate += InternalUpdate;
            }
        }

        private void OnDisable()
        {
            Dispose();
        }
    }
}
