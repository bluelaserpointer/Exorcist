using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class WraithNormal : MonoBehaviour
{
    [SerializeField]
    float sightDistance = 7;
    [SerializeField]
    float _maxHealth = 12;
    [SerializeField]
    Image _healthFillRing;
    [SerializeField]
    IzumiTools.Cooldown _doorOpenAttemptCD = new IzumiTools.Cooldown(0.5F);
    [SerializeField]
    float attackRange = 2;
    [SerializeField]
    IzumiTools.Cooldown _attackCD = new IzumiTools.Cooldown(2);
    [SerializeField]
    IzumiTools.Cooldown _anotherPathAttemptCD = new IzumiTools.Cooldown(4F);
    [SerializeField]
    float _calmdownTimeLength = 0.2F;

    [SerializeField]
    Rigidbody2D _rigidbody;
    [SerializeField]
    NavMeshAgent _navMeshAgent;
    [SerializeField]
    SpriteRenderer _baseSpriteRenderer;

    [Header("Emotion")]
    [SerializeField]
    SpriteRenderer _emotionSpriteRenderer;
    [SerializeField]
    Sprite _idleEmotion;
    [SerializeField]
    Sprite _chaseEmotion;
    [SerializeField]
    Sprite _runEmotion;
    [SerializeField]
    Sprite _angryEmotion;

    [Header("SE")]
    [SerializeField]
    AudioClip _chaseSE;
    [SerializeField]
    AudioClip _runSE;
    [SerializeField]
    AudioClip _angrySE;
    [SerializeField]
    AudioClip _attackSE;
    [SerializeField]
    AudioClip _deadSE;

    public float Health { get; private set; }
    public enum State { Idle, Chase, Run, Angry }
    public State CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                _emotionDisplayRemainingTime = 1.5F;
                switch (_currentState = value)
                {
                    case State.Idle:
                        _emotionSpriteRenderer.sprite = _idleEmotion;
                        break;
                    case State.Chase:
                        _attackCD.Ratio = 0.75F;
                        _emotionSpriteRenderer.sprite = _chaseEmotion;
                        AudioSource.PlayClipAtPoint(_chaseSE, transform.position);
                        break;
                    case State.Run:
                        _emotionSpriteRenderer.sprite = _runEmotion;
                        AudioSource.PlayClipAtPoint(_runSE, transform.position);
                        break;
                    case State.Angry:
                        _anotherPathAttemptCD.value = _anotherPathAttemptCD.requiredValue;
                        _emotionSpriteRenderer.sprite = _angryEmotion;
                        AudioSource.PlayClipAtPoint(_angrySE, transform.position);
                        break;
                }
            }
        }
    }

    float stateRemainingTime;
    State _currentState;
    float _emotionDisplayRemainingTime;
    Vector2 _rigidbodyMoveVec;
    float _lastHurtTime;
    private void Awake()
    {
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _currentState = State.Idle;
        Health = _maxHealth;
    }
    void Update()
    {
        _healthFillRing.fillAmount = Health / _maxHealth;
        if (_emotionDisplayRemainingTime > 0)
        {
            _emotionDisplayRemainingTime -= Time.deltaTime;
            _emotionSpriteRenderer.gameObject.SetActive(true);
        }
        else
        {
            _emotionSpriteRenderer.gameObject.SetActive(false);
        }
        _rigidbodyMoveVec = Vector2.zero;
        if(CurrentState == State.Idle)
        {
            Health = Mathf.Min(_maxHealth, Health + Time.deltaTime * 8);
        }
        Vector2 desiredMoveVec = Vector2.zero;
        switch (CurrentState)
        {
            case State.Idle:
                if (Vector2.Distance(transform.position, GameManager.Player.transform.position) < sightDistance
                    && !GameManager.TestRayObstruction(transform.position, GameManager.Player.transform.position))
                {
                    CurrentState = State.Chase;
                }
                break;
            case State.Angry:
            case State.Chase:
                _attackCD.AddDeltaTime();
                if(!_attackCD.IsReady)
                {
                    if ((int)(_attackCD.value * 10) % 2 == 0)
                    {
                        _baseSpriteRenderer.transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        _baseSpriteRenderer.transform.localPosition = Vector3.up * 0.15F;
                    }
                    _navMeshAgent.isStopped = true;
                }
                else
                {
                    _baseSpriteRenderer.transform.localPosition = Vector3.zero;
                    _navMeshAgent.isStopped = false;
                    Vector2 playerPos = GameManager.Player.transform.position;
                    if (CurrentState == State.Angry)
                    {
                        if(_anotherPathAttemptCD.AddDeltaTimeAndEat())
                            _navMeshAgent.SetDestination(transform.position + (Vector3)Random.insideUnitCircle.normalized * 12);
                        if(Time.timeSinceLevelLoad - _lastHurtTime > _calmdownTimeLength)
                        {
                            CurrentState = State.Idle;
                        }
                    }
                    else
                    {
                        _navMeshAgent.SetDestination(new Vector3(playerPos.x, playerPos.y, transform.position.z));
                    }
                    desiredMoveVec = _navMeshAgent.desiredVelocity;
                    if (Vector2.Distance(GameManager.Player.transform.position, transform.position) <= attackRange
                            && !GameManager.TestRayObstruction(GameManager.Player.transform.position, transform.position))
                    {
                        _attackCD.Eat();
                        GameManager.Player.Damage(1);
                        AudioSource.PlayClipAtPoint(_attackSE, transform.position);
                    }
                }
                break;
            case State.Run:
                if (stateRemainingTime > 0)
                {
                    _navMeshAgent.isStopped = true;
                    stateRemainingTime -= Time.deltaTime;
                    Vector2 crossPos = GameManager.Cross.transform.position;
                    desiredMoveVec = _rigidbodyMoveVec = ((Vector2)transform.position - crossPos).normalized;
                }
                else
                {
                    stateRemainingTime = 0;
                    CurrentState = State.Idle;
                }
                break;
        }
        //open door
        _doorOpenAttemptCD.AddDeltaTime();
        if(_doorOpenAttemptCD.IsReady)
        {
            foreach (var hitInfo in Physics2D.RaycastAll(transform.position, desiredMoveVec, 1))
            {
                Door door = hitInfo.collider.gameObject.GetComponent<Door>();
                if (door != null)
                {
                    _doorOpenAttemptCD.Eat();
                    door.OnEnemyOpen();
                    break;
                }
            }
        }
    }
    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _rigidbodyMoveVec * _navMeshAgent.speed * Time.fixedDeltaTime);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Cross cross = collision.gameObject.GetComponentInParent<Cross>();
        if (cross != null)
        {
            if (cross.IsEmitting)
            {
                if (!GameManager.TestRayObstruction(cross.transform.position, transform.position))
                {
                    Health -= Time.deltaTime;
                    _lastHurtTime = Time.timeSinceLevelLoad;
                    if (Health <= 0)
                    {
                        Dead();
                    }
                    else if (Health < 8)
                    {
                        CurrentState = State.Angry;
                        stateRemainingTime = 5;
                    }
                    else
                    {
                        if (CurrentState == State.Run || stateRemainingTime <= 0)
                        {
                            CurrentState = State.Run;
                            stateRemainingTime = 5;
                        }
                    }
                }
            }
        }
    }
    public void Dead()
    {
        AudioSource.PlayClipAtPoint(_deadSE, transform.position);
        Destroy(gameObject);
    }
}
