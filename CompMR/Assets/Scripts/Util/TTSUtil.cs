using UnityEngine;
using System.Collections;
using SpeechLib;
using System.Xml;
using System.IO;

namespace Assets.Scripts.Util
{

    public class TTSUtil
    {
        private static TTSUtil _instance = null;
        private static readonly object lockObject = new object();

        public static TTSUtil GetInstance()
        {
            lock (lockObject)
            {
                return _instance ?? (_instance = new TTSUtil());
            }
        }

        private readonly SpVoice _voice;

        private TTSUtil()
        {
            _voice = new SpVoice
            {
                Volume = 100,
                Rate = 1
            };
        }

        public void Speak(string value)
        {
            _voice.Speak(value, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        }
    }
}