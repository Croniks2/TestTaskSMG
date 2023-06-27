using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private Vector2 _range = new Vector2(100, 100);

    
    private void Update()
    {
        Vector3 moveDirection;
        bool haveMoveInput = CheckInput(out moveDirection);

        if(haveMoveInput == true)
        {
            Move(moveDirection);
        }
    }

    private bool CheckInput(out Vector3 direction)
    {
        bool haveMoveInput = Input.GetKey(KeyCode.Mouse1);
        bool haveNotRotateInput = !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftControl));

        if (haveMoveInput == true && haveNotRotateInput == true)
        {
            float x = Input.GetAxis("Mouse X");
            float z = Input.GetAxis("Mouse Y");
            
            Vector3 right = transform.right * x * -1;
            Vector3 forward = transform.forward * z * -1;

            direction = (forward + right).normalized;

            Vector3 newBasisX = _cameraTransform.right;
            Vector3 newBasisY = _cameraTransform.up;
            Vector3 newBasisZ = _cameraTransform.forward;

            Matrix4x4 cameraTransformMatrix = new Matrix4x4(
                new Vector4(newBasisX.x, newBasisX.y, newBasisX.z, 0f),
                new Vector4(newBasisY.x, newBasisY.y, newBasisY.z, 0f),
                new Vector4(newBasisZ.x, newBasisZ.y, newBasisZ.z, 0f),
                new Vector4(0f, 0f, 0f, 1f)
            );

            direction = cameraTransformMatrix.MultiplyPoint3x4(direction);

            return true;
        }

        direction = Vector3.zero;
        return false;
    }

    private void Move(Vector3 direction)
    {
        direction = Quaternion.AngleAxis(-40f, _cameraTransform.right) * direction;
        //Debug.DrawRay(_cameraTransform.position, direction, Color.green, 10f);
     
        Vector3 currentPosition = _cameraTransform.position;
        _cameraTransform.Translate(direction * _speed * Time.deltaTime, Space.World);

        Vector3 newPosition = _cameraTransform.position;

        if (IsInBounds(newPosition) == false)
        {
            _cameraTransform.position = currentPosition;
        }
    }

    private bool IsInBounds(Vector3 position)
    {
        return position.x > -_range.x &&
               position.x < _range.x &&
               position.z > -_range.y &&
               position.z < _range.y;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 5f);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(_range.x * 2f, 5f, _range.y * 2f));
    }
}