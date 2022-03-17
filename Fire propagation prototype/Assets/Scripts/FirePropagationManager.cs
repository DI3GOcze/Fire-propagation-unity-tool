using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGridCell 
{
    public int value;
    public List<GameObject> references;

    public FireGridCell(int value = 0, List<GameObject> references = null)
    {
        this.value = value;
        if(references == null)
        {
            this.references = new List<GameObject>();
        } 
        else
        {
            this.references = references;
        }
    }

    public void DestroyReferences()
    {
        foreach (var item in references)
        {
            GameObject.Destroy(item);
        }
        references.Clear();
    } 
    
}

public class FirePropagationManager : MonoBehaviour
{
    public int gridHeight;
    public int gridWidth;
    public float cellSize;
    [SerializeField] float _maxTerrainHeight;
    [SerializeField] LayerMask _terrainMask;
    [SerializeField] Material _sensorMaterial;
    FireGridCell [,] _grid;

    private static FirePropagationManager _instance;
    public static FirePropagationManager Instance { get { return _instance; } }

    // Singleton
    private void Awake() {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        } 

        _instance = this;
    }

    void Start()
    {
        _grid = new FireGridCell[gridHeight, gridWidth];
        InitiateRandom();
        RenderGrid();
    }

    void RenderGrid()
    {
        if(_grid != null)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    Vector3 pos = GetCellPosition(x, y);

                    var color = (_grid[y,x].value == 1) ? Color.black : Color.white;

                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.AddComponent<FireSnesor>();
                    
                    var boxCollider = cube.GetComponent<BoxCollider>();                 
                    boxCollider.isTrigger = true;
                    boxCollider.size = new Vector3(1, 1, 1);
                    cube.transform.position = pos;
                    cube.transform.localScale = new Vector3(cellSize*.9f, cellSize*2, cellSize*.9f);
                    cube.GetComponent<Renderer>().material = _sensorMaterial;
                    

                    _grid[y,x].references.Add(cube);
                }
            }
        }  
    }

    public Vector3 GetCellPosition(int x, int y)
    {
        Vector2 position2D = new Vector2(transform.position.x + cellSize * (x + .5f - gridWidth * .5f), transform.position.z + cellSize * (y + .5f - gridHeight * .5f));
        Vector3 position3D;

        // If we find position on terrain
        
        if(Physics.Raycast(new Vector3(position2D.x, _maxTerrainHeight, position2D.y), Vector3.down, out RaycastHit hit, float.PositiveInfinity, _terrainMask))
        {
            position3D = hit.point;
        } else
        {
            position3D = new Vector3(position2D.x, 0, position2D.y);
        }

        return position3D;
    }

    private void OnMouseDown() {
        if(_grid == null)
            return;

        KillThemAll();
    }   

    void KillThemAll()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                _grid[y,x].DestroyReferences();
            }
        }
    }

    void InitiateRandom()
    {
        if(_grid != null)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    _grid[y,x] = new FireGridCell(Random.Range(0,2));
                }
            }
        }
    }
}
