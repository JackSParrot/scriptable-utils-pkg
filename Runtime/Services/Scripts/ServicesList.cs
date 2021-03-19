using UnityEngine;

namespace JackSParrot.Utils
{
    [CreateAssetMenu(fileName = "New Services List", menuName = "JackSParrot/Variables/ServicesList", order = 1)]
    public class ServicesList : ListSO<Service>
    {
        public bool IsEditor = false;
        public bool IsIos = false;
        public bool IsAndroid = false;
        public bool IsWindows = false;
        public bool IsMac = false;
        public bool IsLinux = false;
        public bool IsWebGL = false;
    }
}