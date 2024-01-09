using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = System.Random;
using Graphs;
using IO.Swagger.Model;
using Unity.VisualScripting;

public class OfficeMapGenerator : MonoBehaviour
{
    enum CellType {
        None,
        Room,
        Hallway
    }

    [Header("Prefabs Settings")] 
    [SerializeField] private List<GameObject> roomPrefabs;
    [SerializeField] private List<GameObject> stairPrefabs;
    [SerializeField] private GameObject hallwayPrefab;
    
    [Header("Room Settings")] 
    [SerializeField] private Vector3 cellSize;
    [Header("Generator Settings")] 
    [SerializeField] private int seed;
    [SerializeField] private int tryMaxCount = 1000;
    [SerializeField] private int floorCount;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private int roomCount;

    [SerializeField] private int[] minStairCount;
    
    Random random;
    Grid2D<CellType>[] grid;
    [SerializeField] List<Room> rooms;
    [SerializeField] List<Room> stairs;
    List<Wall>[] walls;
    Delaunay2D[] delaunay;
    HashSet<Prim.Edge>[] selectedEdges;

    private bool[,,] checker;
    void Start() {
        Generate();
    }

    void Generate() {
        random = new Random(0);
        
        grid = new Grid2D<CellType>[floorCount];
        for(int i = 0; i < floorCount; i++)
            grid[i] = new Grid2D<CellType>(gridSize, Vector2Int.zero);

        delaunay = new Delaunay2D[floorCount];
        selectedEdges = new HashSet<Prim.Edge>[floorCount];
        rooms = new List<Room>();
        stairs = new List<Room>();
        walls = new List<Wall>[floorCount];

        for (int i = 0; i < floorCount; i++)
            walls[i] = new List<Wall>();

        checker = new bool[gridSize.x, floorCount, gridSize.y];
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < floorCount; j++)
            {
                for (int k = 0; k < gridSize.y; k++)
                {
                    checker[i, j, k] = true;
                }
            }
        }

        for (int i = 0; i < floorCount - 1; i++)
        {
            PlaceStairs(i);
        }
        
        PlaceRooms();
        SetVertex();
        Triangulate();
        CreateHallways();
        PathfindHallways();
    }

    void PlaceRooms()
    {
        int tryCount = 0;
        for (int i = 0; i < roomCount; i++)
        {
            tryCount++;
            if (tryCount > tryMaxCount) break;
            Vector3Int location = new Vector3Int(random.Next(0, gridSize.x - 1), random.Next(0, floorCount), random.Next(0, gridSize.y - 1));
            
            int roomIndex = random.Next(0, roomPrefabs.Count - 1);

            Room room = roomPrefabs[roomIndex].GetComponent<Room>();
            Vector3Int scale = room.scale;
            RotAngle rotAngle = (RotAngle)random.Next(0, 3);
            if (rotAngle == RotAngle.d90 || rotAngle == RotAngle.d270)
            {
                //scale = new Vector3Int(scale.z, scale.y, scale.x);
            }

            bool add = true;
            
            if (location.y + scale.y > floorCount ||
                location.x + scale.x + 2 > gridSize.x ||
                location.z + scale.z + 2 > gridSize.y)
                add = false;
            
            for (int x = location.x; add && x < location.x + scale.x + 2; x++)
            {
                for (int y = location.y; add && y < location.y + scale.y; y++)
                {
                    for (int z = location.z; add && z < location.z + scale.z + 2; z++)
                    {
                        if (!checker[x, y, z])
                        {
                            add = false;
                            break;
                        }
                    }
                }
            }

            if (add == true)
            {
                for (int x = location.x + 1; x < location.x + scale.x + 1; x++)
                {
                    for (int y = location.y; y < location.y + scale.y; y++)
                    {
                        for (int z = location.z + 1; z < location.z + scale.z + 1; z++)
                        {
                            checker[x, y, z] = false;
                            grid[y][new Vector2Int(x, z)] = CellType.Room;
                        }
                    }
                }

                Debug.Log("Place Room : " + i);
                PlaceRoom(roomIndex, new Vector3Int(location.x + 1, location.y, location.z + 1), rotAngle);
            }
            else i--;
        }
    }


    void PlaceStairs(int floor)
    {
        int tryCount = 0;
        for (int i = 0; i < minStairCount[floor]; i++)
        {
            tryCount++;
            if (tryCount > tryMaxCount) break;
            Vector2Int location = new Vector2Int(random.Next(0, gridSize.x - 1), random.Next(0, gridSize.y - 1));
            
            int stairIndex = random.Next(0, stairPrefabs.Count - 1);

            
            Room stair = stairPrefabs[stairIndex].GetComponent<Room>();
            Vector3Int stairScale = stair.scale;
            RotAngle rotAngle = (RotAngle)random.Next(0, 3);
            if (rotAngle == RotAngle.d90 || rotAngle == RotAngle.d270)
            {
                //scale = new Vector3Int(scale.z, scale.y, scale.x);
            }

            bool add = true;

            if (floor + stairScale.y > floorCount ||
                location.x + stairScale.x + 2 > gridSize.x ||
                location.y + stairScale.y + 2 > gridSize.y)
                add = false;
            
            for (int x = location.x; add && x < location.x + stairScale.x + 2; x++)
            {
                for (int y = floor; add && y < floor + stairScale.y; y++)
                {
                    for (int z = location.y; add && z < location.y + stairScale.z + 2; z++)
                    {
                        if (!checker[x, y, z])
                        {
                            add = false;
                            break;
                        }
                    }
                }
            }

            if (add == true)
            {
                for (int x = location.x + 1; x < location.x + stairScale.x + 1; x++)
                {
                    for (int y = floor; y < floor + stairScale.y; y++)
                    {
                        for (int z = location.y + 1; z < location.y + stairScale.z + 1; z++)
                        {
                            checker[x, y, z] = false;
                            grid[y][new Vector2Int(x, z)] = CellType.Room;
                        }
                    }
                }

                PlaceStair(stairIndex, new Vector3Int(location.x + 1, floor, location.y + 1), rotAngle);
            }
            else i--;
        }
    }
    
    void SetVertex()
    {
        foreach (var room in rooms)
        {
            foreach (Wall wall in room.GetDoorPosition())
            {
                Vector3Int pos = wall.GetDoorPosition();
                Debug.Log("pos : " + pos);
                walls[pos.y].Add(wall);
            }
        }
        
        foreach (var room in stairs)
        {
            foreach (Wall wall in room.GetDoorPosition())
            {
                Vector3Int pos = wall.GetDoorPosition();
                Debug.Log("pos : " + pos);
                walls[pos.y].Add(wall);
            }
        }
    }

    void Triangulate() {
        for (int i = 0; i < floorCount; i++)
        {
            List<Vertex> vertices = new List<Vertex>();

            foreach (var wall in walls[i])
            {
                Vector3Int pos = wall.GetDoorPosition();
                wall.ActiveDoor();
                
                vertices.Add(new Vertex<Wall>(new Vector2(pos.x, pos.z), wall));
                Debug.Log("Vertex : " + wall.GetDoorPosition());
            }
            
            delaunay[i] = Delaunay2D.Triangulate(vertices);
        }
    }

    void CreateHallways() {
        for (int i = 0; i < floorCount; i++)
        {
            List<Prim.Edge> edges = new List<Prim.Edge>();
            
    
            Debug.Log("Create Hallways delunacy " + i +" : " + delaunay[i].Edges.Count);
            foreach (var edge in delaunay[i].Edges) {
                if(!(edge.U as Vertex<Wall>).Item.room.Equals((edge.V as Vertex<Wall>).Item.room))
                    edges.Add(new Prim.Edge(edge.U, edge.V));
            }

            List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);

            selectedEdges[i] = new HashSet<Prim.Edge>(mst);
            var remainingEdges = new HashSet<Prim.Edge>(edges);
            remainingEdges.ExceptWith(selectedEdges[i]);

            foreach (var edge in remainingEdges) {
                if (random.NextDouble() < 0.125) {
                    selectedEdges[i].Add(edge);
                }
            }
        }
    }

    void PathfindHallways() {
        for (int floor = 0; floor < floorCount; floor++)
        {
            DungeonPathfinder2D aStar = new DungeonPathfinder2D(gridSize);

            foreach (var edge in selectedEdges[floor]) {
                var startWall = (edge.U as Vertex<Wall>).Item;
                var endWall = (edge.V as Vertex<Wall>).Item;

                var startPosf = startWall.GetDoorPosition();
                var endPosf = endWall.GetDoorPosition();
                var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.z);
                var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.z);
                
                var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) => {
                    var pathCost = new DungeonPathfinder2D.PathCost();
                
                    pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic

                    if (grid[floor][b.Position] == CellType.Room) {
                        pathCost.cost += 10;
                    } else if (grid[floor][b.Position] == CellType.None) {
                        pathCost.cost += 5;
                    } else if (grid[floor][b.Position] == CellType.Hallway) {
                        pathCost.cost += 1;
                    }

                    pathCost.traversable = true;

                    return pathCost;
                });

                if (path != null) {
                    for (int i = 0; i < path.Count; i++) {
                        var current = path[i];

                        if (grid[floor][current] == CellType.None) {
                            grid[floor][current] = CellType.Hallway;
                        }

                        if (i > 0) {
                            var prev = path[i - 1];

                            var delta = current - prev;
                        }
                        
                    }
                    /*
                    foreach (var pos in path) {
                        if (grid[floor][pos] == CellType.Hallway) {
                            PlaceHallway(new Vector3Int(pos.x, floor, pos.y));
                        }
                    }*/
                }
            }
        }
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < floorCount; y++)
            {
                for (int z = 0; z < gridSize.y; z++)
                {
                    if (grid[y][new Vector2Int(x, z)] == CellType.Hallway) {
                        Hallway hallway = PlaceHallway(new Vector3Int(x, y, z));
                        if(x == 0 || grid[y][new Vector2Int(x - 1, z)] == CellType.None)
                            hallway.westHallway.SetActive(true);
                        if(x == gridSize.x - 1 || grid[y][new Vector2Int(x + 1, z)] == CellType.None)
                            hallway.eastHallway.SetActive(true);
                        if(z == 0 || grid[y][new Vector2Int(x, z - 1)] == CellType.None)
                            hallway.southHallway.SetActive(true);
                        if( z == gridSize.y - 1 || grid[y][new Vector2Int(x, z + 1)] == CellType.None)
                            hallway.northHallway.SetActive(true);
                    }
                }
            }
        }
    }


    void PlaceRoom(int roomIndex, Vector3Int position, RotAngle rotAngle)
    {
        GameObject go = Instantiate(roomPrefabs[roomIndex], Vector3.Scale(cellSize, position), Quaternion.identity);
        Room room = go.GetComponent<Room>();
        rooms.Add(room);
        room.Init(new Vector3Int(gridSize.x, floorCount, gridSize.y), position, rotAngle, seed);
    }
    
    
    void PlaceStair(int stairIndex, Vector3Int position, RotAngle rotAngle) {
        GameObject go = Instantiate(stairPrefabs[stairIndex], Vector3.Scale(cellSize, position), Quaternion.identity);
        Room stairRoom = go.GetComponent<Room>();
        stairs.Add(stairRoom);
        stairRoom.Init(new Vector3Int(gridSize.x, floorCount, gridSize.y), position, rotAngle, seed);
    }

    Hallway PlaceHallway(Vector3Int position) {
        GameObject go = Instantiate(hallwayPrefab, Vector3.Scale(cellSize, position), Quaternion.identity);
        return go.GetComponent<Hallway>();
    }
}
