using UnityEngine;

// Reference video: https://www.youtube.com/watch?v=q-BY4G5Rkoo
// This script allows an object to be rotated by clicking and dragging the mouse.   

public class ClickRotateObject : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f; // Speed of rotation

    private void OnMouseDrag()
    {
        float XaxisRotation = Input.GetAxis("Mouse X") * rotationSpeed/* * Time.deltaTime*/;
        float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSpeed/* * Time.deltaTime*/;

        transform.Rotate(Vector3.down, XaxisRotation, Space.World); // Rotate around the Y-axis
        transform.Rotate(Vector3.right, YaxisRotation, Space.World); // Rotate around the X-axis

        LockLogic lockLogic = GetComponent<LockLogic>();
        if(!lockLogic.WrenchHasBeenParented)
        {
            if (FindFirstObjectByType<TensionWrench>() != null)
            {
                FindFirstObjectByType<TensionWrench>().transform.Rotate(Vector3.down, XaxisRotation, Space.World);
                FindFirstObjectByType<TensionWrench>().transform.Rotate(Vector3.right, YaxisRotation, Space.World);
            }
        }

    }
}
