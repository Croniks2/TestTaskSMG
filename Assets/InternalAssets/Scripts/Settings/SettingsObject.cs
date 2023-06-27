using System;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Settings", order = 1)]
public class SettingsObject : ScriptableObject, ISettingsGetter, ISettingsSetter
{
    public event Action SettingsChanged;
    
    public CameraSettings CameraSettings { get => _cameraSettings; set => _cameraSettings = value; }
    [SerializeField] private CameraSettings _cameraSettings;
    
    [SerializeField, Space] private bool _overridePlayerPrefs = false;

    
    public void SaveSettings()
    {
        if (_overridePlayerPrefs == false)
        {
            PlayerPrefs.Save();
        }
        
        SettingsChanged?.Invoke();
    }

    public void LoadSettings()
    {
        if (_overridePlayerPrefs == true)
        {
            return;
        }
    }
}

[Serializable]
public class CameraSettings
{
    public Vector3 initialCameraPosition = Vector3.zero;
    public float maxCameraCicleRadiusLength = 0f;
    public Vector3 cameraCicleRadiusDirection = Vector3.zero;
}

public class PlayerPrefsSettingsNames
{
    
}