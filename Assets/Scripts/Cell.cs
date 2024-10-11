using UnityEngine;

public class Cell : MonoBehaviour
{
    [HideInInspector]public bool collected;
    [HideInInspector]public Transform tongue;
    [HideInInspector]public bool isHidden;
    
    private void Update()
    {
        if (collected) Collected();
        
        float rayDistance = 1.0f;  

        if (Physics.Raycast(transform.position, Vector3.up, rayDistance, ~LayerMask.GetMask("Water")))
        {
            transform.GetChild(0).gameObject.SetActive(false);
            isHidden = true;
        }
        else
        {
            isHidden = false;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void SingleCollected()
    {
        if(tongue == null) return;
        if (transform.CompareTag("Grape"))
        {
            collected = true;
        }
        transform.SetParent(tongue.parent);
    }

    public void Collected()
    {
        if (Vector3.Distance(tongue.position, transform.position) <= 0.5f)
        {
            if(Vector3.Distance(tongue.transform.parent.position, transform.position) <= 1f)
            {
                GetComponent<BoxCollider>().enabled = false;
            }
            transform.SetParent(tongue);
            GetComponent<MeshRenderer>().enabled = false;
            transform.localPosition = Vector3.zero;
        }
    }
}