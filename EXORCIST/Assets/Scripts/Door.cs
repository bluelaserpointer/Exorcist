using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    [SerializeField]
    DoorState _state;
    [SerializeField]
    KeyType _keyType;
    [SerializeField]
    Interactable _interactable;
    [SerializeField]
    Collider2D _collider;
    [SerializeField]
    SpriteRenderer _baseSpriteRenderer;
    [SerializeField]
    SpriteRenderer _lockSpriteRenderer;
    [SerializeField]
    Sprite _openSprite, _closeSprite, _lockSprite, _unlockSprite;
    [SerializeField]
    AudioClip _lockSE, _unlockSE, _openCloseSE, _noKeySE;
    public enum DoorState { Open, Close, Lock}
    public KeyType KeyType => _keyType;
    public DoorState State => _state;
    private void Start()
    {
        SetState(_state, true);
    }
    public void SetState(DoorState state, bool initialize = false)
    {
        if (!initialize && _state == state)
            return;
        switch(state)
        {
            case DoorState.Open:
                _collider.isTrigger = true;
                _baseSpriteRenderer.sprite = _openSprite;
                _baseSpriteRenderer.sortingLayerName = "GroundItem";
                _lockSpriteRenderer.enabled = false;
                _interactable.leftClickText = "关门";
                _interactable.rightClickText = "";
                if(!initialize)
                    AudioSource.PlayClipAtPoint(_openCloseSE, transform.position);
                break;
            case DoorState.Close:
                _collider.isTrigger = false;
                _baseSpriteRenderer.sprite = _closeSprite;
                _baseSpriteRenderer.sortingLayerName = "Wall";
                _lockSpriteRenderer.enabled = true;
                _lockSpriteRenderer.sprite = _unlockSprite;
                _interactable.leftClickText = "开门";
                _interactable.rightClickText = "上锁";
                if (!initialize)
                    AudioSource.PlayClipAtPoint(_state == DoorState.Open ? _openCloseSE : _unlockSE, transform.position);
                break;
            case DoorState.Lock:
                _collider.isTrigger = false;
                _baseSpriteRenderer.sprite = _closeSprite;
                _baseSpriteRenderer.sortingLayerName = "Wall";
                _lockSpriteRenderer.enabled = true;
                _lockSpriteRenderer.sprite = _lockSprite;
                _interactable.leftClickText = "";
                _interactable.rightClickText = "开锁";
                if (!initialize)
                    AudioSource.PlayClipAtPoint(_lockSE, transform.position);
                break;
        }
        _state = state;
    }
    public void OnEnemyOpen()
    {
        if (_state == DoorState.Close)
            SetState(DoorState.Open);
        else if (_state == DoorState.Lock)
            AudioSource.PlayClipAtPoint(_noKeySE, transform.position);
    }
    public void OnLeftClick()
    {
        if (_state == DoorState.Close)
            SetState(DoorState.Open);
        else if (_state == DoorState.Open)
            SetState(DoorState.Close);
        else if(_state == DoorState.Lock)
            AudioSource.PlayClipAtPoint(_noKeySE, transform.position);
    }
    public void OnRightClick()
    {
        if (_state == DoorState.Open)
            return;
        if (GameManager.Player.keyList.Contains(KeyType))
        {
            if (_state == DoorState.Close)
                SetState(DoorState.Lock);
            else if (_state == DoorState.Lock)
                SetState(DoorState.Close);
        }
    }
    private void OnValidate()
    {
        if (_lockSprite == null)
            return;
        switch (KeyType)
        {
            case KeyType.RED:
                _lockSpriteRenderer.color = Color.red;
                break;
            case KeyType.GREEN:
                _lockSpriteRenderer.color = Color.green;
                break;
            case KeyType.BLUE:
                _lockSpriteRenderer.color = Color.blue;
                break;
        }
    }
}
