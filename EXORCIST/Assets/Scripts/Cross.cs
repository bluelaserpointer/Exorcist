using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer _baseSpriteRenderer;
    [SerializeField]
    ParticleSystem _particle;
    [SerializeField]
    Interactable _interactable;

    public bool IsEmitting
    {
        get => _particle.emission.enabled;
        set
        {
            var emission = _particle.emission;
            emission.enabled = value;
        }
    }
    private void Awake()
    {
        IsEmitting = false;
    }
    public void OnInteractCross()
    {
        GameManager.Player.HasCross = !GameManager.Player.HasCross;
    }
    public void OnHasCrossChange(bool cond)
    {
        transform.SetParent(cond ? GameManager.Player.transform : null, true);
        if (cond)
            transform.localPosition = Vector3.zero;
        _interactable.leftClickText = cond ? "∑≈÷√" : " ∞»°";
        _baseSpriteRenderer.enabled = !cond;
        GameManager.SetUIHasCross(cond);
    }
}
