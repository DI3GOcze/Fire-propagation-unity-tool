using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSnesor : MonoBehaviour
{
    Renderer _renderer;

    private void Awake() {
        _renderer = GetComponent<Renderer>();
    }
    
    private void OnTriggerEnter(Collider other) {
        _renderer.material.SetColor("_BaseColor", Color.red);
    }

    private void OnTriggerExit(Collider other) {
        _renderer.material.SetColor("_BaseColor", Color.green);
    }
}
