using EventBusSystem;

public interface IBilletWasCreatedHandler : IGlobalSubscriber
{
    void HandleBilletWasCreated();
}