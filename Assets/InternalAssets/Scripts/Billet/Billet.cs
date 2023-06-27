using DG.Tweening;

using EventBusSystem;

using Spawners;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class Billet : PoolObject
{
    [SerializeField] private RectTransform _inputFieldTrans;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private ContentSizeFitter _sizeFitter;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _contentText;
    [SerializeField, Space] private Image _background;
    [SerializeField] private Material _baseMaterial;
    [SerializeField] private Material _selectMaterial;
    [SerializeField] private float _pressDelay = 0.2f;
    
    public int ID { get; private set; }
    public bool IsSelected => _isSecleted;
    public Vector3 InitialHorizontalDragPosition { get; set; }
    public Vector3 InitialVerticalDragPosition { get; set; }
    
    private string _titleTextFormat = "Плашка {0}";
    private Vector2 _inputFiledInitialSizeDelta;

    private bool _isSecleted = false;
    private bool _isOpenedForEditing = false;

    private float _currentTimeSincePress = 0f;

    
    private void Awake()
    {
        _inputFiledInitialSizeDelta = _inputFieldTrans.sizeDelta;
        _sizeFitter.enabled = false;
        _inputFieldTrans.sizeDelta = new Vector2(_inputFiledInitialSizeDelta.x, 0f);
    }
    
    private void Update()
    {
        _currentTimeSincePress += Time.deltaTime;
    }
    
    public void Setup(int id, Vector3 initialPostion)
    {
        _isSecleted = false;
        _isOpenedForEditing = false;

        _contentText.text = string.Empty;
        _background.material = _baseMaterial;

        _sizeFitter.enabled = false;
        _inputFieldTrans.sizeDelta = new Vector2(_inputFiledInitialSizeDelta.x, 0f);

        ID = id;
        transform.position = initialPostion;
        
        _titleText.text = string.Format(_titleTextFormat, ID + 1);

        transform.DOShakeRotation(1f, 20f, 12, 100).Play();
    }

    public void CalculateInitialDragPosition()
    {

    }

    public void Select(bool on, bool ignoreDelay = false)
    {
        if (_currentTimeSincePress < _pressDelay && ignoreDelay == false)
        {
            return;
        }

        _currentTimeSincePress = 0f;

        if (on == true)
        {
            if (_isSecleted == false)
            {
                _isSecleted = true;
                _background.material = _selectMaterial;

                //Debug.Log($"Выделяем плашку !!!");
            }
            else
            {
                OpenForTextEditing(true);
            }
        }
        else
        {
            if(_isSecleted == true)
            {
                OpenForTextEditing(false);

                _isSecleted = false;
                _background.material = _baseMaterial;

                //Debug.Log($"Снимаем выделение с плашки !!!");
            }
        }
    }

    private void OpenForTextEditing(bool on)
    {
        if(on == true)
        {
            if(_isOpenedForEditing == false)
            {
                _inputFieldTrans.sizeDelta = _inputFiledInitialSizeDelta;

                Tween scaleAnimation = transform.DOScale(new Vector3(0.6f, 1.3f, 1f), 0.25f);

                scaleAnimation.OnComplete(() =>
                {
                    transform.localScale = Vector3.one;
                    _sizeFitter.enabled = true;
                    _isOpenedForEditing = true;

                    //Debug.Log($"Открываем плашку для редактирования текста !!!");
                });

            scaleAnimation.Play();
        }
        }
        else
        {
            if(_isOpenedForEditing == true)
            {
                Tween scaleAnimation = transform.DOScale(new Vector3(0.6f, 1.3f, 1f), 0.25f);

                scaleAnimation.OnComplete(() =>
                {
                    transform.localScale = Vector3.one;

                    _sizeFitter.enabled = false;
                    _inputFieldTrans.sizeDelta = new Vector2(_inputFiledInitialSizeDelta.x, 0f);
                    _isOpenedForEditing = false;

                    //Debug.Log($"Закрываем плашку для редактирования текста !!!");
                });

                scaleAnimation.Play();
            }
        }
    }
}