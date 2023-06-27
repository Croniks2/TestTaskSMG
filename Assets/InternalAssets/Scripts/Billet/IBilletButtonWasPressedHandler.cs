using EventBusSystem;

public interface IBilletButtonWasPressedHandler : IGlobalSubscriber
{
    void HandleBilletButtonPressed();
}