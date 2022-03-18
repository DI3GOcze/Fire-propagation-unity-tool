using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class FireGridCell 
{
    public float health;
    public float burnedThreshold;

    
    public delegate void IgnitedAction();
    public event IgnitedAction OnIgnited;    
    [SerializeField] bool _isOnFire = false;
    public bool isOnFire 
    {
        get { return _isOnFire; }
        set 
        { 
            _isOnFire = value; 
            if(value == true && OnIgnited != null)
                OnIgnited();
        }
    }
    
    public delegate void BurnedAction();
    public event IgnitedAction OnBurned;
    [SerializeField] bool _isBurned = false;
    public bool isBurned
    {
        get { return _isBurned; }
        set 
        { 
            _isBurned = value; 
            if(value == true && OnBurned != null)
                OnBurned();
        }
    }
    
    private List<GameObject> _references;
    private FireSensor _sensor;

    public FireGridCell(float health = 0, float burnedThreshold = -300, List<GameObject> references = null, FireSensor sensor = null)
    {
        this.health = health;
        this.burnedThreshold = burnedThreshold;
        this._sensor = sensor;   

        if(references == null)
        {
            this._references = new List<GameObject>();
        } 
        else
        {
            this._references = references;
        }

        OnIgnited += IgniteCell;
        OnBurned += BurnCell;
    }

    public void DestroyReferences()
    {
        foreach (var item in _references)
        {
            GameObject.Destroy(item);
        }
        _references.Clear();
    } 

    public void SetOnFire()
    {
        health = 0;
        isOnFire = true;
    }

    private void IgniteCell()
    {
        _sensor.ChangeColor(Color.red);
    }

    private void BurnCell()
    {
        _sensor.ChangeColor(new Color(0.22f,0.11f,0.1f));
    }

    public void TakeDamage(float damage)
    {       
        // If cell is already burned, dont do anything
        if(isBurned)
            return;
        
        if(health <= burnedThreshold)
        {
            isBurned = true;
        }
        
        health -= damage * Time.deltaTime;

        if(!isOnFire && health <= 0)
            isOnFire = true; 
    }
    
    public void AttachSensor(FireSensor sensor)
    {
        _sensor = sensor;
        if(_sensor != null)
        {
            _sensor.OnClicked += SetOnFire;
        }
    }
    
}
