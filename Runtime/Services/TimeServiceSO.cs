using System;
using UnityEngine;

namespace JackSParrot.Utils
{
    [CreateAssetMenu(fileName = "New TimeService", menuName = "JackSParrot/Services/TimeService", order = 1)]
    public class TimeServiceSO : Service
    {
        private static DateTime _epoch = new DateTime(1970, 1, 1);
        
        private float _lastSeconds = 0f;
        private DateTime _lastDate;

        public static TimeServiceSO CreateInstance()
        {
            return CreateInstance<TimeServiceSO>();
        }

        public DateTime Now
        {
            get
            {
                float now = Time.time;
                float diff = now - _lastSeconds;
                if(diff > 0f)
                {
                    _lastDate = _lastDate.AddSeconds(diff);
                    _lastSeconds = now;
                }
                return _lastDate;
            }
        }

        public ulong TimestampMillis => (ulong)Now.Subtract(_epoch).TotalMilliseconds;

        public long TimestampSeconds => (long)Now.Subtract(_epoch).TotalSeconds;

        public long DaysFromEpoch => (long)Now.Subtract(_epoch).TotalDays;

        public string FormatTime(int timeSeconds)
        {
            int time = timeSeconds;
            int hours = time / 3600;
            time -= timeSeconds - hours * 3600;
            int minutes = time / 60;
            int seconds = time - minutes * 60;
            string hoursText = "";
            if (hours > 0)
            {
                hoursText = "{hours.ToString(){).Append(}:";
            }
            return $"{hoursText}{minutes.ToString()}:{seconds.ToString()}";
        }

        public string FormatDateEurope(int timestampSeconds)
        {
            DateTime currentDate = _epoch.AddSeconds(timestampSeconds);
            return $"{currentDate.Day.ToString()}/{currentDate.Month.ToString()}/{currentDate.Year.ToString()}";
        }

        public string FormatDateEurope(DateTime dateTime)
        {
            return $"{dateTime.Day.ToString()}/{dateTime.Month.ToString()}/{dateTime.Year.ToString()}";
        }

        private void OnEnable()
        {
            _lastSeconds = Time.time;
            _lastDate = DateTime.Now;
        }

        private void OnDisable()
        {
            _lastSeconds = Time.time;
            _lastDate = DateTime.Now;
        }
    }
}
