using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSensor : MonoBehaviour
{
    public delegate void ClickAction();
    public event ClickAction OnClicked;

    public delegate void ObjectEnteredAction(FireableObject fireableObject);
    public event ObjectEnteredAction OnObjectEntered;

    public delegate void ObjectExiteddAction(FireableObject fireableObject);
    public event ObjectExiteddAction OnObjectExited;
    
    private void OnTriggerEnter(Collider other) {       
        if(other.gameObject.TryGetComponent<FireableObject>(out var fireableObject))
        {
            OnObjectEntered?.Invoke(fireableObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.TryGetComponent<FireableObject>(out var fireableObject))
        {
            OnObjectExited?.Invoke(fireableObject);
        }
    }

    private void OnMouseDown() {
        OnClicked?.Invoke();
    }
}
