using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class LogPrefix
    {
        public const string TIMESTAMP = "timestamp";

        public const string PRIMARY_TASK = "primary_task";
        public const string SECONDARY_TASK = "secondary_task";
        public const string PARTICIPANT = "participant";
        public const string CONDITION = "condition";
        public const string TRIAL = "trial";
        public const string ENABLED = "enabled";
        public const string METHOD = "method";
    }

    public class Logger : MonoBehaviour
    {
        public StudyModel StudyModel;
        public int FlushLineCounter = 100;

        public string Delimiter = ";";
        public string LogFilePrefix = @"log";
        public string LogPath = @"./";

        private int _currentLineCounter = 0;
        private StringBuilder _currentLogString;

        private Dictionary<string, string> _prefixes;
        private string _dayTimeStamp;

        private string GetFilename()
        {
            return _dayTimeStamp + "-" + "p" + StudyModel.ParticipantID.ToString("D") + "-" + LogFilePrefix + ".csv";
        }

        public Dictionary<string, string> Prefixes
        {
            get
            {
                if (_prefixes == null)
                    _prefixes = new Dictionary<string, string>();

                return _prefixes;
            }
        }


        void Start()
        {
            _dayTimeStamp = DateTime.Now.ToString("yyyyMMdd-HHmm");
            _currentLogString = new StringBuilder();

            SetPrefix(LogPrefix.PARTICIPANT, StudyModel.ParticipantID.ToString("D"));
            SetPrefix(LogPrefix.CONDITION, "-1");
            SetPrefix(LogPrefix.PRIMARY_TASK, "none");
            SetPrefix(LogPrefix.TRIAL, "-1");

            //WriteHeader();
        }

        void OnApplicationQuit()
        {
            FlushLog();
        }

        void Update()
        {

        }

        private void WriteHeader()
        {
            var logString = LogPrefix.TIMESTAMP + Delimiter;
            foreach (var key in Prefixes.Keys)
            {
                logString += key + Delimiter;
            }

            logString += Environment.NewLine;

            var path = Path.Combine(LogPath, GetFilename());
            File.AppendAllText(path, logString, Encoding.UTF8);
        }

        public void Log(params string[] values)
        {
            //var path = Path.Combine(LogPath, GetFilename());

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");

            var logString = timestamp + Delimiter;

            foreach (var key in Prefixes.Keys)
            {
                var value = Prefixes[key];
                logString += value + Delimiter;
            }

            foreach (var value in values)
            {
                logString += value + Delimiter;
            }

            logString += Environment.NewLine;

            //Use SB for async write in case performance becomes an issue. For now, good enough.
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine(logString);

            //Defer write in case performance becomes an issue. For now, good enough.
            _currentLogString.Append(logString);
            _currentLineCounter++;

            if (_currentLineCounter >= FlushLineCounter)
                FlushLog();
        }

        public void SetPrefix(string key, float value)
        {
            SetPrefix(key, value.ToString("F4"));
        }

        public void SetPrefix(string key, int value)
        {
            SetPrefix(key, value.ToString("D"));
        }

        public void SetPrefix(string key, string value)
        {
            if (Prefixes.ContainsKey(key))
                Prefixes[key] = value;
            else
                Prefixes.Add(key, value);
        }

        private void FlushLog()
        {
            var path = Path.Combine(LogPath, GetFilename());
            File.AppendAllText(path, _currentLogString.ToString());

            _currentLineCounter = 0;
            _currentLogString.Clear();
        }
    }
}
