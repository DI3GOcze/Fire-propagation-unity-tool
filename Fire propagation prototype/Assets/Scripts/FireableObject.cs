using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Renderer))]
public class FireableObject : MonoBehaviour
{
    [SerializeField] GameObject _firePrefab;
    [SerializeField] GameObject _fireModelSpawnPoint;
    [SerializeField] float igniteTreshold = 100f;
    [SerializeField] float temperatureDecreaseRate = 20f;
    [SerializeField] float temperatureIncreaseRate = 40f;
    [SerializeField] float _temperature;
    /// <summary>
    /// Damage taken per second when on fire
    /// </summary>
    public float onFireDamage = 5;
    private bool _isOnFire = false;
    public bool isOnFire 
    {
        get { return _isOnFire; }
        set 
        {
            if(_isOnFire == value)
                return; 

            _isOnFire = value;

            if(value == true)
            {
                CrateFireModel();
            }
            else // value == false
            {
                DestroyFireModel();
            }
        }
    }
    public bool isDead = false;
    public int isInIgnitedCells = 0;
    private void Start() {
        CoolDownTemperature();
    }

    virtual public void TakeDamage(float damage)
    {
        
    }

    public void CoolDownTemperature()
    {
        _temperature = 0;
        isOnFire = false;
    }


    /// <summary>
    /// FireDamageObject() is called every frame from FireGridCell, if the cell is on fire
    /// </summary>
    public void DealFireDamage(float amount)
    {
        TakeDamage(amount);
    }

    /// <summary>
    /// If object is in ignited cell too long it catches on fire it self
    /// </summary>
    public void IncreaseTemperature(float amount)
    {
        _temperature += amount;

        // If the temperature exceeded ignite treshold
        if(_temperature >= igniteTreshold)
        {
            _temperature = igniteTreshold;

            isOnFire = true;
        }
    }

    public void DecreaseTemperature(float amount)
    {
        _temperature -= amount;

        // If the temperature decreases enough, put down fire
        if(_temperature <= 0)
        {
            _temperature = 0;

            isOnFire = false;
        }
    }

    protected void CrateFireModel()
    {
        Instantiate(_firePrefab, _fireModelSpawnPoint.transform.position, _firePrefab.transform.rotation, _fireModelSpawnPoint.transform);
    }

    protected void DestroyFireModel()
    {
        foreach (Transform item in _fireModelSpawnPoint.transform)
        {
            Destroy(item.gameObject);
        }
    }

    public void PutDownFire()
    {
        isOnFire = false;
    }

    void Update() {
        
        // If the object is on fire, it shloud take damage
        if(isOnFire)
        {
            TakeDamage(onFireDamage);           
        }    

        // If object is in on fire cell, temperature should increase,
        // othervise temperature shoul decrease over time.
        if(isInIgnitedCells > 0)
            IncreaseTemperature(temperatureIncreaseRate * Time.deltaTime);
        else
            DecreaseTemperature(temperatureDecreaseRate * Time.deltaTime);
        

    }
}
