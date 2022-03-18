using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSensor : MonoBehaviour
{
    Renderer _renderer;
    public FireGridCell _cell;
    public delegate void ClickAction();
    public event ClickAction OnClicked;
    int couter = 0;

    private void Awake() {
        _renderer = GetComponent<Renderer>();
    }
    
    private void OnTriggerEnter(Collider other) {
        couter++;
        
    }

    private void OnTriggerExit(Collider other) {
        couter--;
    }

    private void OnMouseDown() {
        if(OnClicked != null)
        {
            OnClicked();
        }
    }

    public void ChangeColor(Color color)
    {
        _renderer.material.SetColor("_BaseColor", color);
    }
}
