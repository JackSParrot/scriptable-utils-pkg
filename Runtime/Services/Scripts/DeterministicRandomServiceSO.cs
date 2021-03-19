using System;
using UnityEngine;

namespace JackSParrot.Utils
{
    [CreateAssetMenu(fileName = "New DeterministicRandomService", menuName = "JackSParrot/Services/DeterministicRandom", order = 1)]
    public class DeterministicRandomServiceSO : Service
    {
        [SerializeField] private long _seed = 0;

        private long _prev = 0;
        const long m = 4294967296; // aka 2^32
        const long a = 1664525;
        const long c = 1013904223;

        public static DeterministicRandomServiceSO CreateInstance(long seed)
        {
            DeterministicRandomServiceSO retVal = CreateInstance<DeterministicRandomServiceSO>();
            retVal.Init(seed);
            return retVal;
        }

        public void Init(long seed)
        {
            _prev = seed;
        }

        public void Init(string seed)
        {
            long nseed = 0;
            for(int i = 0; i < seed.Length; ++i)
            {
                nseed += seed[i];
            }
            _prev = nseed;
        }

        public long Next()
        {
            _prev = ((a * _prev) + c) % m;
            return _prev;
        }

        public long Next(long min, long max)
        {
            Next();
            return (_prev % max + min) % max;
        }

        private void OnEnable()
        {
            Init(_seed);
        }
    }
}