using UnityEngine;

public class BilletRaycastTarget : MonoBehaviour
{
    public Billet Billet => _billet;
    [SerializeField] private Billet _billet;
}