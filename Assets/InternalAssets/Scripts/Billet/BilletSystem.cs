using System.Collections.Generic;

using DG.Tweening;

using EventBusSystem;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BilletSystem : MonoBehaviour, IBilletButtonWasPressedHandler
{
    [SerializeField] private GraphicRaycaster _graphicRaycaster;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField, Space] private Transform _cameraTransform;
    [SerializeField] private Transform _billetsParent;
    [SerializeField] private BilletsPool _billetsPool;

    [SerializeField] private float _maxTerrainHeight = 50f;
    [SerializeField] private float _maxBilletsCount = 999f;
    [SerializeField] private LayerMask _forHorizontalDrag;
    [SerializeField] private LayerMask _forVerticalDrag;

    private float _minBilletHeight = 0f;
    private float _maxBilletHeight = 0f;

    private bool _billetCreationModeIsActive = false;
    
    private List<RaycastResult> _raycastResults;
    private Dictionary<int, Billet> _billets;
    private int _newBilletID = -1;

    private List<int> _tempIdList;
    private List<Billet> _tempBilletsList;

    private bool _startToDrag = false;
    private Vector3 _initialHorizontalDragPosition;
    private Vector3 _initialVerticalDragPosition;
    private Vector3 _endDragPosition;

    private float _delayBeforeStartingToTrag = 0.2f;
    private float _currentDragDelay = 0f;

    void Awake()
    {
        DOTween.defaultAutoPlay = AutoPlay.None;
        DOTween.defaultAutoKill = false;
        DOTween.defaultRecyclable = true;

        _raycastResults = new List<RaycastResult>();
        _billets = new Dictionary<int, Billet>();
        _tempIdList = new List<int>();
        _tempBilletsList = new List<Billet>();

        _minBilletHeight = _maxTerrainHeight + 5f;
        _maxBilletHeight = _minBilletHeight + 100f;
    }

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    private void Update()
    {
        ProcessBillets();
        TryToCreateBillet();
        TryToDeleteBillets();
        TryToDragBillets();
    }

    private void TryToCreateBillet()
    {
        if (_billetCreationModeIsActive == true && Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;

            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z + 100f));
            Ray ray = new Ray(_cameraTransform.position, worldMousePos - _cameraTransform.position);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _forHorizontalDrag))
            {
                // Создаем плашку
                Billet newBillet = _billetsPool.Spawn();

                // Сетапим плашку
                newBillet.transform.SetParent(_billetsParent);
                newBillet.Setup(++_newBilletID, new Vector3(hit.point.x, _minBilletHeight, hit.point.z));

                _billets.Add(newBillet.ID, newBillet);
                newBillet.Select(true, true);

                _billetCreationModeIsActive = false;
                EventBus.RaiseEvent<IBilletWasCreatedHandler>(h => h.HandleBilletWasCreated());
            }
        }
    }

    private void ProcessBillets()
    {
        bool haveMouse0Input = Input.GetKeyUp(KeyCode.Mouse0) == true;
        bool haveCtrlInput = Input.GetKey(KeyCode.LeftControl) == true;

        if (haveMouse0Input == true)
        {
            PointerEventData pointerEventData = new PointerEventData(_eventSystem) { position = Input.mousePosition };
            _raycastResults.Clear();
            _graphicRaycaster.Raycast(pointerEventData, _raycastResults);

            if (_raycastResults.Count > 0)
            {
                _tempBilletsList.Clear();

                foreach (var raycastResult in _raycastResults)
                {
                    if (raycastResult.gameObject.TryGetComponent(out BilletRaycastTarget billetTarget))
                    {
                        billetTarget.Billet.Select(true);
                        _tempIdList.Add(billetTarget.Billet.ID);
                    }
                }

                if(haveCtrlInput == false)
                {
                    foreach (var billet in _billets)
                    {
                        foreach (var id in _tempIdList)
                        {
                            if (billet.Key != id)
                            {
                                billet.Value.Select(false);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var billet in _billets)
                {
                    billet.Value.Select(false);
                }
            }
        }
    }
    
    private void TryToDeleteBillets()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            _tempBilletsList.Clear();

            foreach (var billetPair in _billets)
            {
                var billet = billetPair.Value;
                if (billet.IsSelected == true)
                {
                    _tempBilletsList.Add(billet);
                }
            }

            _tempBilletsList.ForEach(b => 
            { 
                b.ReturnToPool();
                _billets.Remove(b.ID);
            });
        }
    }

    private void TryToDragBillets()
    {
        if(_startToDrag == false)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) == true)
            {
                PointerEventData pointerEventData = new PointerEventData(_eventSystem) { position = Input.mousePosition };
                _raycastResults.Clear();
                _graphicRaycaster.Raycast(pointerEventData, _raycastResults);

                if (_raycastResults.Count > 0)
                {
                    RaycastResult raycastResult = _raycastResults[0];
                    if (raycastResult.gameObject.TryGetComponent(out BilletRaycastTarget billetTarget))
                    {
                        if(billetTarget.Billet.IsSelected == true)
                        {
                            _initialHorizontalDragPosition = raycastResult.worldPosition;
                            _startToDrag = true;
                            
                            foreach (var billetPair in _billets)
                            {
                                Billet billet = billetPair.Value;

                                if (billet.IsSelected == true)
                                {
                                    billet.InitialHorizontalDragPosition = billet.transform.position;
                                    billet.InitialVerticalDragPosition = billet.transform.position;
                                }
                            }

                            RaycastHit hit;
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _forVerticalDrag))
                            {
                                _initialVerticalDragPosition = hit.point;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Mouse0) == true)
            {
                _currentDragDelay += Time.deltaTime;

                if (_currentDragDelay >= _delayBeforeStartingToTrag)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    
                    if(Input.GetKey(KeyCode.LeftAlt) == true)
                    {
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _forVerticalDrag))
                        {
                            _endDragPosition = hit.point;
                            Vector3 translation = _endDragPosition - _initialVerticalDragPosition;

                            foreach (var billetPair in _billets)
                            {
                                Billet billiet = billetPair.Value;
                                
                                Vector3 currentPosition = billiet.transform.position;
                                currentPosition.y = billiet.InitialVerticalDragPosition.y;

                                if (billiet.IsSelected == true)
                                {
                                    Vector3 newPosition = currentPosition + new Vector3(0f, translation.y, 0f);

                                    if(newPosition.y < _minBilletHeight)
                                    {
                                        newPosition.y = _minBilletHeight;
                                    }
                                    else if (newPosition.y > _maxBilletHeight)
                                    {
                                        newPosition.y = _maxBilletHeight;
                                    }

                                    billiet.transform.position = newPosition;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _forHorizontalDrag))
                        {
                            _endDragPosition = hit.point;
                            Vector3 translation = _endDragPosition - _initialHorizontalDragPosition;

                            foreach (var billetPair in _billets)
                            {
                                Billet billiet = billetPair.Value;

                                Vector3 currentPosition = billiet.transform.position;
                                currentPosition.x = billiet.InitialHorizontalDragPosition.x;
                                currentPosition.z = billiet.InitialHorizontalDragPosition.z;

                                if (billiet.IsSelected == true)
                                {
                                    billiet.transform.position = currentPosition + new Vector3(translation.x, 0f, translation.z);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                _currentDragDelay = 0f;
                _startToDrag = false;
            }
        }
    }

    public void HandleBilletButtonPressed()
    {
        _billetCreationModeIsActive = true;
    }
}