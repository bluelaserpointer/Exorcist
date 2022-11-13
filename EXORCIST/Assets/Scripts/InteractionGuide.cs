using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class InteractionGuide : MonoBehaviour
{
    [SerializeField]
    Text _leftClickText, _rightClickText;

    [SerializeField]
    CanvasGroup _canvasGroup;

    public string LeftClickText
    {
        get => _leftClickText.text;
        set => _leftClickText.text = value;
    }
    public string RightClickText
    {
        get => _rightClickText.text;
        set => _rightClickText.text = value;
    }
    private void Update()
    {
        if(Vector2.Distance(transform.position, GameManager.Player.transform.position) < GameManager.Player.InteractionRange)
        {
            _canvasGroup.alpha = 1;
        }
        else
        {
            _canvasGroup.alpha = 0.75F;
        }
    }
    public void SetTarget(Interactable interactable)
    {
        transform.position = interactable.transform.position;
        _leftClickText.text = interactable.leftClickText;
        _rightClickText.text = interactable.rightClickText;
    }
}
