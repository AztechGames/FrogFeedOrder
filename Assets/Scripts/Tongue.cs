using System.Collections.Generic;
using UnityEngine;

public class Tongue : MonoBehaviour
{
    public Frog frog;
    private Vector3[] hitPoints = new Vector3[20];
    private int hitCount = 0;
    public List<Cell> AllObject = new();

    private void OnTriggerEnter(Collider other)
    {
        if (transform.parent == other.transform || other.GetComponent<Cell>().isHidden) return;

        var otherCell = other.GetComponent<Cell>();
        var colorData = other.GetComponent<ColorData>();

        if (other.GetComponentInChildren<Frog>() == null && colorData.color == frog.color && !otherCell.isHidden)
        {
            AllObject.Add(otherCell);

            if (other.CompareTag("Grape"))
            {
                frog.maxTongueLength += 0.8f;
                PlaySound(frog.grapeSound);
                other.GetComponent<Animator>().SetTrigger("Touch");
                return;
            }
            frog.maxTongueLength = 2f;
            frog.isHit = true;
            hitCount++;
            hitPoints[hitCount] = other.transform.position + Vector3.up * 0.2f;
            frog.currentTongueDirection = other.transform.GetChild(0).forward;
            UpdateTongueRenderer();
        }
        else
        {
            PlaySound(frog.tongueSound);
            AllObject.Clear();
            frog.isExtending = false;
            frog.isRetracting = true;
        }
    }

    private void Update()
    {
        if (frog.isRetracting && frog.isHit && IsWithinHitDistance())
        {
            RetractTongue();
        }

        UpdateTonguePosition();

        if (frog.collect && frog.isRetracting)
        {
            frog.extendSpeed += AllObject.Count/5f;
            CollectObjects();
        }
    }

    private bool IsWithinHitDistance()
    {
        return Vector3.Distance(frog.tongueRenderer.GetPosition(frog.tongueRenderer.positionCount - 1), hitPoints[hitCount]) <= 0.5f;
    }

    private void RetractTongue()
    {
        if (hitCount == 1) frog.currentLength = Vector3.Distance(frog.tongueRenderer.GetPosition(0), frog.tongueRenderer.GetPosition(frog.tongueRenderer.positionCount - 1));
        else frog.currentLength = Vector3.Distance(hitPoints[hitCount - 1], frog.tongueRenderer.GetPosition(frog.tongueRenderer.positionCount - 1));
        frog.tongueRenderer.positionCount--;
        hitCount--;
        frog.currentTongueDirection = (frog.tongueRenderer.GetPosition(frog.tongueRenderer.positionCount - 1) - frog.tongueRenderer.GetPosition(frog.tongueRenderer.positionCount - 2)).normalized;

        if (hitCount == 0) frog.isHit = false;
    }

    private void UpdateTonguePosition()
    {
        frog.newTonguePosition = frog.isHit
            ? hitPoints[hitCount] + frog.currentTongueDirection * frog.currentLength
            : frog.TPos + frog.currentTongueDirection * frog.currentLength;
    }

    private void CollectObjects()
    {
        foreach (var obj in AllObject)
        {
            obj.tongue = transform;
            obj.SingleCollected();
        }
        AllObject.Clear();
    }

    private void UpdateTongueRenderer()
    {
        frog.tongueRenderer.SetPosition(frog.tongueRenderer.positionCount - 1, hitPoints[hitCount]);
        frog.tongueRenderer.positionCount++;
        frog.tongueRenderer.SetPosition(frog.tongueRenderer.positionCount - 1, hitPoints[hitCount]);
        frog.currentLength = Vector3.Distance(hitPoints[hitCount], frog.tongueRenderer.GetPosition(frog.tongueRenderer.positionCount - 2));
    }

    private void PlaySound(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}