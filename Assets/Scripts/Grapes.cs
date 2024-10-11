using System;
using UnityEngine;

public class Grapes : MonoBehaviour
{
    GameObject upperGrape;
    BoxCollider boxCollider => GetComponent<BoxCollider>();
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != gameObject && other.transform.position.y > transform.position.y)
        {
            upperGrape = other.gameObject;
            transform.GetChild(0).gameObject.SetActive(false);
            boxCollider.enabled = false;
        }
    }

    private void Update()
    {
        if (upperGrape == null)
        {
            boxCollider.enabled = true;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
