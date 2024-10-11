using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.GetComponentInChildren<Frog>() != null && !hit.collider.GetComponent<Cell>().isHidden)
                {
                    hit.collider.GetComponentInChildren<Frog>().ExtendTongue();
                }
            }
        }
    }

}
