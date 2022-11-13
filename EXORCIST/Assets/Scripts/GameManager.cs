using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField]
    Camera _camera;
    [SerializeField]
    Player _player;
    [SerializeField]
    Cross _cross;
    [SerializeField]
    InteractionGuide _interactionGuide;

    [Header("UI")]
    [SerializeField]
    Image[] _healthIcons;
    [SerializeField]
    Image _crossIcon;
    [SerializeField]
    Image _redKeyIcon, _greenKeyIcon, _blueKeyIcon;

    public static Player Player => Instance._player;
    public static Cross Cross => Instance._cross;
    public static Camera Camera => Instance._camera;
    public static Interactable MouseOveringInteractable { get; private set; }
    private void Awake()
    {
        Instance = this;
        _interactionGuide.gameObject.SetActive(false);
    }
    void Update()
    {
        //click interaction
        MouseOveringInteractable = null;
        foreach (var hit in Physics2D.GetRayIntersectionAll(Camera.ScreenPointToRay(Input.mousePosition)))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                MouseOveringInteractable = interactable;
                break;
            }
        }
        if(MouseOveringInteractable == null)
        {
            _interactionGuide.gameObject.SetActive(false);
        }
        else
        {
            _interactionGuide.gameObject.SetActive(true);
            _interactionGuide.SetTarget(MouseOveringInteractable);
            if (Vector2.Distance(MouseOveringInteractable.transform.position, Player.transform.position) < Player.InteractionRange)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    MouseOveringInteractable?.OnLeftClick.Invoke();
                }
                if (Input.GetMouseButtonDown(1))
                {
                    MouseOveringInteractable?.OnRightClick.Invoke();
                }
            }
        }
    }
    public static bool TestRayObstruction(Vector2 origin, Vector2 target)
    {
        foreach (var hitInfo in Physics2D.RaycastAll(origin, target - origin, Vector2.Distance(origin, target)))
        {
            Collider2D collider = hitInfo.collider;
            if (collider.isTrigger || collider.tag != "Wall")
                continue;
            return true;
        }
        return false;
    }
    public static void SetUIHasCross(bool cond)
    {
        Instance._crossIcon.enabled = cond;
    }
    public static void SetUIPlayerHealth(int health)
    {
        for(int i = 0; i < Instance._healthIcons.Length; ++i)
        {
            Instance._healthIcons[i].enabled = i < health;
        }
    }
    public static void SetUIObtainKey(KeyType keyType)
    {
        switch(keyType)
        {
            case KeyType.RED:
                Instance._redKeyIcon.enabled = true;
                break;
            case KeyType.GREEN:
                Instance._greenKeyIcon.enabled = true;
                break;
            case KeyType.BLUE:
                Instance._blueKeyIcon.enabled = true;
                break;
        }
    }
    public static void GameOver()
    {

    }
}
