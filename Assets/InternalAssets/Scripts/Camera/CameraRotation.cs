using EventBusSystem;

using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _speed = 15f;

    
    private void Update()
    {
        bool rotationDirection;
        bool haveRotationInput = CheckInput(out rotationDirection);

        if (haveRotationInput == true)
        {
            Vector3 rotationPoint;
            bool haveRotationPoint = CalculateRotationPoint(out rotationPoint);

            if(haveRotationPoint == true)
            {
                Rotate(rotationPoint, rotationDirection);
            }
        }
    }

    private bool CheckInput(out bool direction)
    {
        if(Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftShift))
        {
            direction = true;
            return true;
        }
        else if(Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftControl))
        {
            direction = false;
            return true;
        }

        direction = false;
        return false;
    }
    
    private bool CalculateRotationPoint(out Vector3 rotaionPointPos)
    {
        RaycastHit hit;
        Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            rotaionPointPos = hit.point;
            return true;
        }
        
        rotaionPointPos = Vector3.zero;
        return false;
    }

    private void Rotate(Vector3 rotationPoint, bool rotationDirection)
    {
        int sign = rotationDirection == true ? 1 : -1;
        Vector3 rotationAxis = Vector3.up * sign;

        _cameraTransform.RotateAround(rotationPoint, rotationAxis, _speed * Time.deltaTime);
    }
}