using EventBusSystem;

using UnityEngine;
using UnityEngine.UI;

public class BilletButton : MonoBehaviour, IBilletWasCreatedHandler
{
    [SerializeField] Button _button;
    [SerializeField] Image _buttonImage;
    [SerializeField] Color _baseButtonColor;
    [SerializeField] Color _selectedButtonColor;

    private bool _billetCreationButtonIsActive = false;

    private void OnEnable()
    {
        EventBus.Subscribe(this);
        _button.onClick.AddListener(OnClickEventHandler);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
        EventBus.Unsubscribe(this);
    }

    private void OnClickEventHandler()
    {
        if(_billetCreationButtonIsActive == false)
        {
            //Debug.Log($"Кнопка создания плашки нажата !!!");

            _billetCreationButtonIsActive = true;
            _buttonImage.color = _selectedButtonColor;
            EventBus.RaiseEvent<IBilletButtonWasPressedHandler>(h => h.HandleBilletButtonPressed());
        }
    }

    public void HandleBilletWasCreated()
    {
        _buttonImage.color = _baseButtonColor;
        _billetCreationButtonIsActive = false;
    }
}