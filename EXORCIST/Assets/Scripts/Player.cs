using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D _rigidbody;
    [SerializeField]
    SpriteRenderer _baseSpriteRenderer;

    [Header("Mobility")]
    [Min(0f)]
    [SerializeField]
    float _moveSpeed = 3;

    [Header("Interaction")]
    [Min(0f)]
    [SerializeField]
    float _interactionRange;

    [Header("SE")]
    [SerializeField]
    AudioSource _footstepSE;
    [SerializeField]
    AudioSource _skillSE;
    public Vector2 InputVec { get; private set; }
    public float InteractionRange => _interactionRange;
    public readonly List<KeyType> keyList = new List<KeyType>();
    [HideInInspector]
    public IzumiTools.Cooldown damageBlinkCD = new IzumiTools.Cooldown(1.5F);
    public bool HasCross
    {
        get => GameManager.Cross.transform.parent == transform;
        set
        {
            GameManager.Cross.OnHasCrossChange(value);
        }
    }
    public int Health { get; private set; }
    private void Awake()
    {
        damageBlinkCD.value = damageBlinkCD.requiredValue;
    }
    private void Start()
    {
        GameManager.SetUIHasCross(HasCross);
        GameManager.SetUIPlayerHealth(Health = 3);
    }
    void Update()
    {
        InputVec = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if(InputVec != Vector2.zero)
        {
            _footstepSE.gameObject.SetActive(true);
        }
        else
        {
            _footstepSE.gameObject.SetActive(false);
        }
        if (Input.GetMouseButton(1) && (GameManager.MouseOveringInteractable == null || GameManager.MouseOveringInteractable.rightClickText.Length == 0))
        {
            _skillSE.gameObject.SetActive(true);
            GameManager.Cross.IsEmitting = true;
        }
        else
        {
            _skillSE.gameObject.SetActive(false);
            GameManager.Cross.IsEmitting = false;
        }
        if (HasCross && (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(2)))
        {
            HasCross = false;
        }
        if(!damageBlinkCD.IsReady)
        {
            damageBlinkCD.AddDeltaTime();
            _baseSpriteRenderer.enabled = (int)(damageBlinkCD.value * 8) % 2 == 0;
        }
    }
    public void Damage(int damage)
    {
        GameManager.SetUIPlayerHealth(Health -= damage);
        if (Health < 0)
        {
            enabled = false;
            _baseSpriteRenderer.enabled = false;
            GameManager.GameOver();
        }
        else
        {
            damageBlinkCD.value = 0;
        }
    }
    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + InputVec * _moveSpeed * Time.fixedDeltaTime);
    }
}
