using System.Collections;
using UnityEngine;

public struct NeighbourCells 
{
    public FireGridCell top;
    public FireGridCell bottom;
    public FireGridCell right;
    public FireGridCell left;
}

public class FirePropagationManager : MonoBehaviour
{
    public int gridHeight = 50;
    public int gridWidth = 50;
    public float cellSize = 2;
    public float cellHeightMultiplier = 5;
    public bool enableFollowTerrain = true;

    [SerializeField] float _maxTerrainHeight = 500;
    [SerializeField] LayerMask _terrainMask;
    [SerializeField] GameObject _sensorPrefab;
    [SerializeField] Material _sensorMaterial;
    [SerializeField] [Range(1f,100f)] float _damageMultiplier;
    
    private FireGridCell [,] _grid;

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

    private void Start()
    {
        _grid = new FireGridCell[gridHeight, gridWidth];
        InitiateRandom();
        RenderGrid();
    }

    private void Update() {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                FireGridCell currentCell = _grid[y,x];
                var cells = GetCellNeighbours(x,y);
                
                float damage = 0;

                if(currentCell.isOnFire)
                    damage++;

                foreach (var item in cells)
                {
                    if(item == null)
                        continue;

                    if(item.isOnFire)
                        damage++;

                }
                currentCell.TakeDamage(damage * _damageMultiplier);

            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>array[4] (indexes: 0 => top, 1 => right, 2 => bottom, 3 => left)</returns>
    FireGridCell[] GetCellNeighbours(int x, int y)
    {       
        FireGridCell[] cells = new FireGridCell[4];

        // Top cell
        if(y - 1 >= 0)
            cells[0] = _grid[y - 1,x];
        else
            cells[0] = null;

        // Right cell
        if(x + 1 < gridWidth)
            cells[1] = _grid[y,x + 1];
        else
            cells[1] = null;

        // Bottom cell
        if(y + 1 < gridHeight)
            cells[2] = _grid[y + 1,x];
        else
            cells[2] = null;

        // Top cell
        if(x - 1 >= 0)
            cells[3] = _grid[y,x - 1];
        else
            cells[3] = null;

        // // Top left cell
        // if(y + 1 < gridHeight && x - 1 >= 0)
        //     cells[4] = _grid[y + 1,x - 1];
        // else
        //     cells[4] = null;

        // // Top right cell
        // if(y + 1 < gridHeight && x + 1 < gridWidth)
        //     cells[5] = _grid[y + 1,x + 1];
        // else
        //     cells[5] = null;

        // // Bottom right cell
        // if(y - 1 >= 0 && x + 1 < gridWidth)
        //     cells[6] = _grid[y - 1,x + 1];
        // else
        //     cells[6] = null;

        // // Bottom left cell
        // if(y - 1 >= 0 && x - 1 >= 0)
        //     cells[7] = _grid[y - 1,x - 1];
        // else
        //     cells[7] = null;

        return cells;
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
                    // var sensor = Instantiate(_sensorPrefab, pos, _sensorPrefab.transform.rotation, this.transform);
                    FireSensor sensor = CreateNewSensor(pos);
                    sensor._cell = _grid[y,x];

                    _grid[y,x].AttachSensor(sensor);
                }
            }
        }  
    }

    FireSensor CreateNewSensor(Vector3 position)
    {
        FireSensor sensor = Instantiate(_sensorPrefab).GetComponent<FireSensor>();
        sensor.transform.localScale = new Vector3(sensor.transform.localScale.x * cellSize, sensor.transform.localScale.y * cellSize*cellHeightMultiplier, sensor.transform.localScale.z * cellSize);  
        sensor.transform.position = position;
    
        return sensor;
    }

    public Vector3 GetCellPosition(int x, int y)
    {
        Vector2 position2D = new Vector2(transform.position.x + cellSize * (x + .5f - gridWidth * .5f), transform.position.z + cellSize * (y + .5f - gridHeight * .5f));
        Vector3 position3D;

        // If we find position on terrain
        
        if(Physics.Raycast(new Vector3(position2D.x, _maxTerrainHeight, position2D.y), Vector3.down, out RaycastHit hit, float.PositiveInfinity, _terrainMask) && enableFollowTerrain)
        {
            position3D = hit.point;
        } else
        {
            position3D = new Vector3(position2D.x, transform.position.y, position2D.y);
        }

        return position3D;
    }

    // private void OnMouseDown() {
    //     if(_grid == null)
    //         return;

    //     KillThemAll();
    // }   

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
                    _grid[y,x] = new FireGridCell(Random.Range(10f, 80f), -800);
                }
            }
        }
    }
}
