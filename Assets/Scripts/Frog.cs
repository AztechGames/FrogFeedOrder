using System;
using UnityEngine;

public class Frog : MonoBehaviour
{
    public ColorData.mColor color;
    public LineRenderer tongueRenderer => GetComponent<LineRenderer>();
    public float extendSpeed = 5f;

    [HideInInspector] public bool isExtending = false;
    [HideInInspector] public bool isRetracting = false;
    [HideInInspector] public float currentLength = 0f;
    [HideInInspector] public Vector3 currentTongueDirection;
    [HideInInspector] public Vector3 TPos;
    [HideInInspector] public bool isHit;
    [HideInInspector] public Vector3 newTonguePosition;
    [HideInInspector] public bool collect;
    [HideInInspector] public float maxTongueLength = 1f;

    public AudioClip grapeSound, tongueSound, disolveSound;
    public Transform tongueTipCollider;

    public void InitializeTongue()
    {
        isHit = false;
        collect = false;
        TPos = transform.position + Vector3.up * 0.2f;
        tongueRenderer.positionCount = 2;
        tongueRenderer.SetPositions(new Vector3[] { TPos, TPos });
        currentTongueDirection = -transform.forward;
        newTonguePosition = TPos + currentTongueDirection * currentLength;
    }

    private void Update()
    {
        if (isExtending) ExtendTongueForward();
        if (isRetracting) RetractTongue();
        UpdateTongueTipColliderPosition();
    }

    private void ExtendTongueForward()
    {
        currentLength = Mathf.MoveTowards(currentLength, maxTongueLength, extendSpeed * Time.deltaTime);
        tongueRenderer.SetPosition(tongueRenderer.positionCount - 1, newTonguePosition);

        if (currentLength >= maxTongueLength)
        {
            collect = true;
            PlaySound(tongueSound);
            isExtending = false;
            isRetracting = true;
        }
    }

    private void RetractTongue()
    {
        currentLength = Mathf.MoveTowards(currentLength, 0f, extendSpeed * Time.deltaTime);
        tongueRenderer.SetPosition(tongueRenderer.positionCount - 1, newTonguePosition);
        tongueTipCollider.GetComponent<BoxCollider>().enabled = false;

        if (currentLength <= 0f)
        {
            isRetracting = false;
            currentTongueDirection = transform.forward;
            if (collect)
            {
                PlaySound(disolveSound);
                GameManager.Instance.TotalFrog--;
                Destroy(transform.parent.gameObject);
            }
        }
    }

    public void ExtendTongue()
    {
        if (!isExtending && !isRetracting)
        {
            GameManager.Instance.MoveLeft--;
            tongueTipCollider.GetComponent<BoxCollider>().enabled = true;
            isExtending = true;
            InitializeTongue();
            currentLength = 0f;
        }
    }

    private void UpdateTongueTipColliderPosition()
    {
        tongueTipCollider.position = tongueRenderer.GetPosition(tongueRenderer.positionCount - 1);
    }

    private void PlaySound(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}