using System;

public interface ISettingsGetter 
{
    public event Action SettingsChanged;

    public CameraSettings CameraSettings { get; }
}