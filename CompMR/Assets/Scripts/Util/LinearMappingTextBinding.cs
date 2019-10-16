using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class LinearMappingTextBinding : MonoBehaviour
{
    public LinearMapping Mapping;
    public TextMesh Text;
    public AppModel AppModel;
    private bool _started;

    void Awake()
    {
        Mapping.value = AppModel.User.CognitiveCapacity;
        Text.text = AppModel.User.CognitiveCapacity.ToString("F3");
    }

    void Update()
    {
        if (_started)
        {
            AppModel.User.CognitiveCapacity = Mapping.value;
            Text.text = AppModel.User.CognitiveCapacity.ToString("F3");
        }
        else
        {
            _started = true;
            Mapping.value = AppModel.User.CognitiveCapacity;
            Text.text = AppModel.User.CognitiveCapacity.ToString("F3");
        }
    }
}
