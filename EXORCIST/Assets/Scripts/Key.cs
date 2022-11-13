using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyType { RED, GREEN, BLUE }
[DisallowMultipleComponent]
public class Key : MonoBehaviour
{
    [SerializeField]
    KeyType _keyType;
    [SerializeField]
    SpriteRenderer _spriteRenderer;

    public KeyType KeyType => _keyType;

    public void GetKey()
    {
        GameManager.Player.keyList.Add(KeyType);
        GameManager.SetUIObtainKey(KeyType);
        Destroy(gameObject);
    }
    private void OnValidate()
    {
        if (_spriteRenderer == null)
            return;
        switch (KeyType)
        {
            case KeyType.RED:
                _spriteRenderer.color = Color.red;
                break;
            case KeyType.GREEN:
                _spriteRenderer.color = Color.green;
                break;
            case KeyType.BLUE:
                _spriteRenderer.color = Color.blue;
                break;
        }
    }
}
