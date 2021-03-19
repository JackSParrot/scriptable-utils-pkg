using System;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Utils
{
    public abstract class Service : ScriptableObject, IDisposable 
    {
        public virtual void Dispose() { }
    }

    [CreateAssetMenu(fileName = "New ServiceLocator", menuName = "JackSParrot/Services/ServiceLocator", order = 0)]
    public class ServiceLocatorSO : ScriptableObject
    {
        [SerializeField] private List<ServicesList> _globalServices = new List<ServicesList>();
        private readonly Dictionary<Type, IDisposable> _services = new Dictionary<Type, IDisposable>();
        private bool _initialized = false;

        public static ServiceLocatorSO CreateInstance()
        {
            return CreateInstance<ServiceLocatorSO>();
        }

        private void Initialize()
        {
            if(_initialized)
            {
                return;
            }
            foreach(ServicesList servicesList in _globalServices)
            {
                bool isValidEditor = (servicesList.IsEditor && Application.isEditor) || (!servicesList.IsEditor && !Application.isEditor);
                if(!isValidEditor)
                {
                    continue;
                }
                RuntimePlatform currentPlatform = Application.platform;
                bool isValidPlatform = servicesList.IsAndroid && currentPlatform == RuntimePlatform.Android ||
                                       servicesList.IsIos && currentPlatform == RuntimePlatform.IPhonePlayer ||
                                       servicesList.IsWindows && currentPlatform == RuntimePlatform.WindowsPlayer || 
                                       servicesList.IsMac && currentPlatform == RuntimePlatform.OSXPlayer ||
                                       servicesList.IsWebGL && currentPlatform == RuntimePlatform.WebGLPlayer ||
                                       servicesList.IsLinux && currentPlatform == RuntimePlatform.LinuxPlayer ||
                                       servicesList.IsLinux && currentPlatform == RuntimePlatform.LinuxEditor ||
                                       servicesList.IsMac && currentPlatform == RuntimePlatform.OSXEditor ||
                                       servicesList.IsWindows && currentPlatform == RuntimePlatform.WindowsEditor;
                if(isValidPlatform)
                {
                    foreach(var service in servicesList.Elements)
                    {
                        _services.Add(service.GetType(), service);
                    }
                }
            }
            _initialized = true;
        }

        public void RegisterService<T>(T service, bool overwrite = false) where T : class, IDisposable
        {
            Initialize();
            Type type = typeof(T);
            if (_services.ContainsKey(type))
            {
                if (overwrite)
                {
                    UnregisterService<T>();
                }
                else
                {
                    Debug.LogError("Tried to add an already existing service to the service locator: " + type.Name);
                    return;
                }
            }
            _services.Add(type, service);
        }

        public void RegisterService<T>() where T : class, IDisposable, new()
        {
            RegisterService(new T());
        }

        public bool HasService<T>() where T : IDisposable
        {
            Initialize();
            return _services.ContainsKey(typeof(T));
        }

        public T GetService<T>() where T : IDisposable
        {
            Initialize();
            if (!_services.TryGetValue(typeof(T), out IDisposable service))
            {
                Debug.LogError($"Tried to get a non registered service from the service locator: {typeof(T).Name}");
                return default;
            }
            return (T)service;
        }

        public void UnregisterService<T>()
        {
            Type type = typeof(T);
            if (_services.ContainsKey(type))
            {
                _services[type].Dispose();
                _services.Remove(type);
            }
        }

        public void UnregisterAll()
        {
            foreach (KeyValuePair<Type, IDisposable> service in _services)
            {
                service.Value.Dispose();
            }
            _services.Clear();
        }

        private void OnDisable()
        {
            UnregisterAll();
            _initialized = false;
        }
    }
}
