using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class FireGridCell : MonoBehaviour
{
    // How long cell stays burned
    [SerializeField] float burnedPersistanceTime = 20f;

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
            // If value doesnt change dont do anything
            if(_isOnFire == value)
                return;
            
            _isOnFire = value; 
            if(value == true)
            {
                // If cell is on fire, it cannot be on burned
                isBurned = false;

                OnIgnited?.Invoke();
                
                // Inform objects in cell, that cell is on fire
                foreach (FireableObject fireableObject in _objectsInCell)
                {
                    fireableObject.isInIgnitedCells++;
                }
            } 
            else
            {
                // Inform objects in cell, that cell was put down
                foreach (FireableObject fireableObject in _objectsInCell)
                {
                    fireableObject.isInIgnitedCells--;
                }
            }
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
            // If value doesnt change dont do anything
            if(_isBurned == value)
                return;
            
            _isBurned = value; 
            
            if(value == true && OnBurned != null)
            {
                // If cell is burned down, it cannot be on fire
                isOnFire = false;
                OnBurned();
                StartCoroutine(CellBurnnedTimer());
            }
        }
    }
    
    [SerializeField] List<FireableObject> _objectsInCell;
    [SerializeField] GameObject _fireParticlesPrefab;
    [SerializeField] GameObject _fireParticleParent;
    private FireSensor _sensor;

    private void Awake() {
        _sensor = GetComponent<FireSensor>();
        _objectsInCell = new List<FireableObject>();

        OnIgnited += IgniteCell;
        OnBurned += BurnCell;

        _sensor.OnClicked += SetOnFire;
        _sensor.OnObjectEntered += AddFireableObject;
        _sensor.OnObjectExited += RemoveFireableObject;
    }

    private void Start() {
        health += health * Random.Range(-0.5f, 0.5f);
        
        
    }

    public void DestroyReferences()
    {
        foreach (var item in _objectsInCell)
        {
            GameObject.Destroy(item);
        }
        _objectsInCell.Clear();
    } 

    public void DamageCellObjects()
    {
        foreach (FireableObject item in _objectsInCell)
        {
                item.DealFireDamage(30 * Time.deltaTime);
        }
    }

    // Sets the cell on fire and also sets all objects inside this cell on fire
    public void SetOnFire()
    {
        isOnFire = true;
    }

    // This damages the cell - only for simulation of fire spread 
    // Doesnt damage objects inside cell, only sets them 
    public void TakeDamage(float damage)
    {       
        // If cell is already burned, dont do anything
        if(isBurned)
            return;
               
        health -= damage * Time.deltaTime;

        if(health <= burnedThreshold)
            isBurned = true;

        // If cell isnt already on fire or isnt already burned and ...
        if(!isOnFire && !isBurned && health <= 0)
            SetOnFire();
    }

    // After burnedPersistanceTime resets the cell, so it can be ignited again
    IEnumerator CellBurnnedTimer()
    {
        yield return new WaitForSeconds(burnedPersistanceTime);
        isBurned = false;
    }

    private void IgniteCell()
    {
        Instantiate(_fireParticlesPrefab, _fireParticleParent.transform.position, _fireParticlesPrefab.transform.rotation, _fireParticleParent.transform);
    }

    private void BurnCell()
    {
        foreach (Transform child in _fireParticleParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void AddFireableObject(FireableObject fireableObject)
    {
        if(fireableObject == null)
            return;

        _objectsInCell.Add(fireableObject);   

        // If cell is already on fire when entering
        if(isOnFire)
            fireableObject.isInIgnitedCells++;
    }

    private void RemoveFireableObject(FireableObject fireableObject)
    {
        if(fireableObject == null)
            return;
        
        _objectsInCell.Remove(fireableObject);

        // if cell is still on fire, when leaving
        if(isOnFire)
        {
            fireableObject.isInIgnitedCells--;
        }
    }
    
}
