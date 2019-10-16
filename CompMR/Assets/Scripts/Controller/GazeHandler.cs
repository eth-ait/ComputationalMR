using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Assets.Scripts.Event;
using Assets.Scripts.Model;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Logger = Assets.Scripts.Util.Logger;

namespace Assets.Scripts.Controller
{
    public class GazeHandler : MonoBehaviour
    {
        public string PathToGazeServer = @"D:\Work\ETH\projects\ComputationalMixedReality\development\CogLoadTest\pupil-unity-gaze-server.py";
        [Unity.Collections.ReadOnly] public float IPA = 0.0f;
        [Unity.Collections.ReadOnly] public float IPA_Raw = 0.0f;

        public float NormalizeUpperBound = Constants.IPA_UPPER_BOUND;
        public float NormalizeLowerBound = Constants.IPA_LOWER_BOUND;


        public float SmoothingAlpha = 0.001f;
        public string IPAStringFormat = "F2";

        public float RegularizerMin = .2f;
        public float RegularizerMax = .4f;

        public bool NormalizeIPA = true;
        public bool EnableDebugView = true;

        public GameObject DebugVis;
        public TextMesh DebugTextMesh;

        static UdpClient udp;
        Thread thread;
        static readonly object lockObject = new object();
        string returnData = "";
        bool precessData = false;
        private Process _process;

        private List<float> CogLoadValues;
        private bool _newValueReceived;

        public AppModel AppModel;

        private float _rawValue;

        public event EventHandler<GazeDataEventArgs> NewIPAValueAvailable;

        void Start()
        {
            DebugVis.transform.parent.gameObject.SetActive(EnableDebugView);

            CogLoadValues = new List<float>();

            StartGazeServer(PathToGazeServer, string.Empty);

            thread = new Thread(ThreadMethod);
            thread.Start();
        }

        void Update()
        {
            if (_newValueReceived)
            {
                _newValueReceived = false;

                if (DebugVis != null)
                {
                    DebugVis.transform.localScale = new Vector3(1, IPA, 1);
                    UnityUtil.SetColor(DebugVis, IPA , Mathf.Clamp(1.2f - IPA, .0f, 1f), .2f, true);
                }

                if (DebugTextMesh != null)
                    DebugTextMesh.text = IPA.ToString(IPAStringFormat);

                if (AppModel != null)
                {
                    AppModel.User.CognitiveCapacity = 1.0f - IPA;
                    AppModel.User.CognitiveLoad = IPA;
                }
            }
        }

        void OnApplicationQuit()
        {
            udp.Close();
            thread.Abort();

            if (_process != null)
            {
                _process.CloseMainWindow();
                _process.Close();
            }

        }

        private void ThreadMethod()
        {
            udp = new UdpClient(50000);
            udp.EnableBroadcast = true;

            while (true)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 50000);

                byte[] receiveBytes = udp.Receive(ref RemoteIpEndPoint);

                lock (lockObject)
                {
                    returnData = Encoding.UTF8.GetString(receiveBytes);

                    var success = float.TryParse(returnData.Substring(4, returnData.Length - 4 - 1), out _rawValue);

                    if (!success)
                        continue;

                    IPA_Raw = _rawValue;

                    var clampedRawValue = Mathf.Clamp(_rawValue, NormalizeLowerBound, NormalizeUpperBound);

                    var normalizedValue = Normalize(clampedRawValue);

                    normalizedValue = RegularizerMin + (RegularizerMax - RegularizerMin) * normalizedValue;

                    IPA = NormalizeIPA ? normalizedValue : clampedRawValue;

                    if (CogLoadValues.Count > 2)
                        IPA = IPA * SmoothingAlpha + CogLoadValues[CogLoadValues.Count - 1] * (1.0f - SmoothingAlpha);
                    
                    CogLoadValues.Add(IPA);

                    _newValueReceived = true;

                    NewIPAValueAvailable?.Invoke(this, new GazeDataEventArgs(normalizedValue, _rawValue, IPA));

                    //Debug.Log(returnData + " / " + IPA);
                }
            }
        }

        private float Normalize(float value)
        {
            return (value - NormalizeLowerBound) / (NormalizeUpperBound - NormalizeLowerBound);
        }

        private void StartGazeServer(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"{cmd} {args}",
                UseShellExecute = true
            };

            Debug.Log("starting process " + start);
            _process = Process.Start(start);
        }
    }
}
