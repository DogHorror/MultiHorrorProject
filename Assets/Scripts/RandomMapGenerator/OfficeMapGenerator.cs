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
    
    [Header("Room Settings")] 
    [SerializeField] private Vector3 cellSize;
    [Header("Generator Settings")] 
    [SerializeField] private int seed;
    [SerializeField] private int floorCount;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private int roomCount;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material blueMaterial;

    [SerializeField] private int[] minStairCount;
    
    Random random;
    Grid2D<CellType>[] grid;
    List<Room> rooms;
    List<Room> stairs;
    Delaunay2D delaunay;
    HashSet<Prim.Edge> selectedEdges;

    private bool[,,] checker;
    void Start() {
        Generate();
    }

    void Generate() {
        random = new Random(0);
        
        grid = new Grid2D<CellType>[floorCount];
        for(int i = 0; i < floorCount; i++)
            grid[i] = new Grid2D<CellType>(gridSize, Vector2Int.zero);
        
        rooms = new List<Room>();
        stairs = new List<Room>();

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
        Triangulate();
        CreateHallways();
        PathfindHallways();
    }

    void PlaceRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            Vector3Int location = new Vector3Int(random.Next(1, gridSize.x - 1), random.Next(0, floorCount), random.Next(1, gridSize.y - 1));
            
            int roomIndex = random.Next(0, roomPrefabs.Count);

            Room room = roomPrefabs[roomIndex].GetComponent<Room>();
            Vector3Int scale = room.scale;
            RotAngle rotAngle = (RotAngle)random.Next(0, 3);
            if (rotAngle == RotAngle.d90 || rotAngle == RotAngle.d270)
            {
                scale = new Vector3Int(scale.z, scale.y, scale.x);
            }

            bool add = true;
            
            for (int x = 0; x < scale.x; x++)
            {
                for (int y = 0; y < scale.y; y++)
                {
                    for (int z = 0; z < scale.z; z++)
                    {
                        if (!checker[x, y, z])
                        {
                            add = false;
                            break;
                        }
                    }
                }
            }

            if (location.y + scale.y > floorCount ||
                location.x + scale.x > gridSize.x ||
                location.z + scale.z > gridSize.y)
            {
                add = false;
            }

            if (add == true)
            {
                for (int x = 0; x < scale.x; x++)
                {
                    for (int y = 0; y < scale.y; y++)
                    {
                        for (int z = 0; z < scale.z; z++)
                        {
                            checker[x, y, z] = false;
                            grid[y][new Vector2Int(x, z)] = CellType.Room;
                        }
                    }
                }
                PlaceRoom(roomIndex, location, rotAngle);
            }
        }
    }


    void PlaceStairs(int floor)
    {
        for (int i = 0; i < minStairCount[floor]; i++)
        {
            Vector2Int location = new Vector2Int(random.Next(1, gridSize.x - 1), random.Next(1, gridSize.y - 1));
            
            int stairIndex = random.Next(0, stairPrefabs.Count);

            Room stair = stairPrefabs[stairIndex].GetComponent<Room>();
            Vector3Int scale = stair.scale;
            RotAngle rotAngle = (RotAngle)random.Next(0, 3);
            if (rotAngle == RotAngle.d90 || rotAngle == RotAngle.d270)
            {
                scale = new Vector3Int(scale.z, scale.y, scale.x);
            }
            
            
            
            bool add = true;
            
            for (int x = 0; x < scale.x; x++)
            {
                for (int y = 0; y < scale.y; y++)
                {
                    for (int z = 0; z < scale.z; z++)
                    {
                        if (!checker[x, y, z])
                        {
                            add = false;
                            break;
                        }
                    }
                }
            }
            
            if (floor + scale.y > floorCount ||
                location.x + scale.x > gridSize.x ||
                location.y + scale.y > gridSize.y)
            {
                add = false;
            }

            if (add == true)
            {
                for (int x = 0; x < scale.x; x++)
                {
                    for (int y = 0; y < scale.y; y++)
                    {
                        for (int z = 0; z < scale.z; z++)
                        {
                            checker[x, y, z] = false;
                            grid[y][new Vector2Int(x, z)] = CellType.Room;
                        }
                    }
                }

                PlaceStair(stairIndex, new Vector3Int(location.x, floor, location.y), rotAngle);
            }
        }
    }
    
    /*
    void PlaceRooms() {
        for (int i = 0; i < roomCount; i++) {
            Vector2Int location = new Vector2Int(
                random.Next(0, size.x),
                random.Next(0, size.y)
            );

            Vector2Int roomSize = new Vector2Int(
                random.Next(1, roomMaxSize.x + 1),
                random.Next(1, roomMaxSize.y + 1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));

            foreach (var room in rooms) {
                if (Room.Intersect(room, buffer)) {
                    add = false;
                    break;
                }
            }
        
            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y) {
                add = false;
            }

            if (add) {
                rooms.Add(newRoom);
                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size);

                foreach (var pos in newRoom.bounds.allPositionsWithin) {
                    grid[pos] = CellType.Room;
                }
            }
        }
    }*/

    void Triangulate() {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms) {
            foreach(Wall wall in room.GetDoorPosition())
                vertices.Add(new Vertex<Wall>(wall.GetDoorPosition(), wall));
        }

        delaunay = Delaunay2D.Triangulate(vertices);
    }

    void CreateHallways() {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges) {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges) {
            if (random.NextDouble() < 0.125) {
                selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways() {
        for (int floor = 0; floor < floorCount; floor++)
        {
            DungeonPathfinder2D aStar = new DungeonPathfinder2D(gridSize);

            foreach (var edge in selectedEdges) {
                var startWall = (edge.U as Vertex<Wall>).Item;
                var endWall = (edge.V as Vertex<Wall>).Item;

                var startPosf = startWall.GetDoorPosition();
                var endPosf = endWall.GetDoorPosition();
                var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
                var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);

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

                    foreach (var pos in path) {
                        if (grid[floor][pos] == CellType.Hallway) {
                            PlaceHallway(pos);
                        }
                    }
                }
            }
        }
    }

    void PlaceCube(Vector2Int location, Vector2Int size, Material material) {
        GameObject go = Instantiate(cubePrefab, new Vector3(location.x, 0, location.y), Quaternion.identity);
        go.GetComponent<Transform>().localScale = new Vector3(size.x, 1, size.y);
        go.GetComponent<MeshRenderer>().material = material;
    }

    void PlaceRoom(int roomIndex, Vector3Int position, RotAngle rotAngle) {
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

    void PlaceHallway(Vector2Int location) {
        PlaceCube(location, new Vector2Int(1, 1), blueMaterial);
    }
}
