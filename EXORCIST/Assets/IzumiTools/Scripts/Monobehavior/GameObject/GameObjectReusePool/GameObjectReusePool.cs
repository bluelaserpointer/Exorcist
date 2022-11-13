using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameObjectReusePool : MonoBehaviour
{
    public T Get<T>(System.Func<T> generator, System.Action<T> reactivator = null) where T : Component
    {
        T component;
        foreach (Transform childTf in transform)
        {
            if (childTf.gameObject.activeSelf)
                continue;
            if((component = childTf.GetComponent<T>()) != null)
            {
                component.gameObject.SetActive(true);
                reactivator?.Invoke(component);
                return component;
            }
        }
        (component = generator.Invoke()).transform.SetParent(transform, false);
        return component;
    }
    public void InactivateAll()
    {
        foreach (Transform childTf in transform)
        {
            childTf.gameObject.SetActive(false);
        }
    }
}
