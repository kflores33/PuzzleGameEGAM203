using UnityEngine;

public class Keyhole : MonoBehaviour
{
    public void RotateKeyhole(float angle)
    {
        Vector3 transformAngle = new Vector3(0, angle, 0);

        this.transform.Rotate(transformAngle);
    }
}
