using EventBusSystem;

public interface IRotationCameraHandler : IGlobalSubscriber
{
    void HandleCameraRotation(float y);
}