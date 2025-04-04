using UnityEngine;

public class Pick : MonoBehaviour, ITool
{
    public bool CheckAlignment()
    {
        Ray ray = new Ray(transform.position, transform.up);
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            if (hit.collider.gameObject.GetComponent<Keyhole>())
            {
                //Debug.Log("Lock is aligned");
                return true;
            }
        }
        return false;
    }
}
