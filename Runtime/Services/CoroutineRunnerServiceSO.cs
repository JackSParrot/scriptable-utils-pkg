using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Utils
{
    [CreateAssetMenu(fileName = "New CoroutineRunner", menuName = "JackSParrot/Services/CoroutineRunner", order = 1)]
    public class CoroutineRunnerServiceSO : Service
    {
        public class Runner : MonoBehaviour {}

        private Runner _runner = null;
        private Dictionary<object, List<Coroutine>> _running = new Dictionary<object, List<Coroutine>>();

        public static CoroutineRunnerServiceSO CreateInstance()
        {
            return CreateInstance<CoroutineRunnerServiceSO>();
        }

        public Coroutine StartCoroutine(object sender, IEnumerator coroutine)
        {
            if(sender == null)
            {
                return null;
            }
            if(_runner == null)
            {
                OnEnable();
            }
            var ret = _runner.StartCoroutine(coroutine);
            if(!_running.ContainsKey(sender))
            {
                _running.Add(sender, new List<Coroutine>());
            }
            _running[sender].Add(ret);
            _runner.StartCoroutine(RunCoroutine(sender, ret));
            return ret;
        }

        public void StopCoroutine(object sender, Coroutine coroutine)
        {
            if (sender == null || !_running.ContainsKey(sender) || !_running[sender].Contains(coroutine))
            {
                return;
            }
            _running[sender].Remove(coroutine);
            _runner.StopCoroutine(coroutine);
        }

        public void StopAllCoroutines(object sender)
        {
            if (sender == null || !_running.ContainsKey(sender))
            {
                return;
            }
            foreach(var cor in _running[sender])
            {
                if(cor != null)
                {
                    _runner.StopCoroutine(cor);
                }
            }
            _running.Remove(sender);
        }

        public override void Dispose()
        {
            _running.Clear();
            if(_runner != null)
            {
                Destroy(_runner.gameObject);
            }
        }

        private void OnEnable()
        {
            if (Application.isPlaying && _runner == null)
            {
                _runner = new GameObject("CoroutineRunner").AddComponent<Runner>();
                DontDestroyOnLoad(_runner.gameObject);
            }
        }

        private void OnDisable()
        {
            Dispose();
        }

        private IEnumerator RunCoroutine(object sender, Coroutine coroutine)
        {
            yield return coroutine;
            _running[sender].Remove(coroutine);
        }
    }
}