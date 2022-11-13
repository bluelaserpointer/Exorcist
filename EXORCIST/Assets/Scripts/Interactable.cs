using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    public string leftClickText;
    public string rightClickText;
    [SerializeField]
    UnityEvent _onLeftClick, _onRightClick;

    public UnityEvent OnLeftClick => _onLeftClick;
    public UnityEvent OnRightClick => _onRightClick;
}
