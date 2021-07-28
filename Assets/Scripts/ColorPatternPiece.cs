using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPatternPiece : MonoBehaviour
{
    [SerializeField] float distanceErrorTolerance = 0.1f;
    [SerializeField] float rotationErrorTolerance = 0.1f;
    [SerializeField] Vector3 originalPosition;
    [SerializeField] Vector3 randomPosition;
    [SerializeField] Vector3 originalRotation;
    [SerializeField] Vector3 randomRotation;

    public GameObject highlight;

    private void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation.eulerAngles;
        RemoveHighlight();
    }

    internal void RegisterData(Vector3 pos, Vector3 rot)
    {
        randomPosition = pos;
        randomRotation = rot;
    }

    internal void ReturnToPosition(float time)
    {
        LeanTween.move(gameObject, randomPosition, time);
        LeanTween.rotateLocal(gameObject, randomRotation, time);
    }

    internal void ShowHighlight()
    {
        highlight.SetActive(true);
    }

    

    internal void RemoveHighlight()
    {
        highlight.SetActive(false);
    }

    internal bool CheckIfValid()
    {
        //check if position matches
        return (originalPosition - transform.position).magnitude <= distanceErrorTolerance && (transform.rotation.eulerAngles - originalRotation).magnitude <= rotationErrorTolerance;
    }

    internal void SetToPosition()
    {
        transform.position = originalPosition;
    }
}
