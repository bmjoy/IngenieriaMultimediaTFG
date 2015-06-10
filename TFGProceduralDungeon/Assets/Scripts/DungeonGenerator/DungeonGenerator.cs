using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum ObjectType
{
  // Items
  COIN = 1,
  CHEST,
  POTION_HEALTH,
  // Trampas
  TRAP_SPIKES_FLOOR,
  // Otros
  EXIT
}

public class DungeonGenerator : MonoBehaviour
{
  // Dimensiones
  public int DUNGEON_WIDTH;
  public int DUNGEON_HEIGHT;
  // Dimensiones minimas del espacio donde se va a crear la habitacion
  public static float NODE_MIN_SIZE = 12f;
  // Margen dentro del espacio del nodo. Hay que tener en cuenta que sera el doble
  private float ROOM_MARGIN = 2f;
  // ROOM
  // Las dimensiones minimas de la habitacion seran la mitad de NODE_MIN_SIZE
  // Las dimensiones maximas seran NODE_MIN_SIZE - ROOM_MARGIN

  public GameObject baseRoom; // Objeto base para generar habitacion
  public GameObject floorTile; // Tile para suelo
  public GameObject wallTile; // Tile para pared
  // Plano/rejilla sobre el que realizar las subdivisiones
  public Grid levelGrid; // Habitaciones
  public Grid objectsGrid; // Items y trampas

  private int nextId;
  private BSPTree bspTree;
  private bool entranceCreated;
  private bool exitCreated;

  // Prefabs para el inspector
  public GameObject Traps_Arrows;
  public GameObject Traps_FloorSpikes;
  public GameObject Traps_WallSpikes;
  public GameObject Traps_RollingRock;
  public GameObject Portal;
  public GameObject Coin;
  public GameObject Chest;
  public GameObject PotionHealth;
  public GameObject EnemyCrab;
  public GameObject EnemyGoblin;
  public GameObject EnemyBat;

  // Setters para los sliders de los test
  public void SetDungeonWidth(float value)
  {
    DUNGEON_WIDTH = (int)value;
  }

  public void SetDungeonHeight(float value)
  {
    DUNGEON_HEIGHT = (int)value;
  }

  void Start()
  {
    GenerateDungeonBSP();
  }

  // Genera la mazmorra usando el algoritmo de BSP Tree
  public void GenerateDungeonBSP()
  {
    levelGrid = new Grid(DUNGEON_WIDTH, DUNGEON_HEIGHT);
    objectsGrid = new Grid(DUNGEON_WIDTH, DUNGEON_HEIGHT);
    // 0 --> vacio, >= 1 --> room id
    nextId = 1;
    // Inicializamos los tiles a vacio
    for (int i = 0; i < levelGrid.GetWidth(); i++)
    {
      for (int j = 0; j < levelGrid.GetHeight(); j++)
      {
        levelGrid.SetTile(i, j, 0);
      }
    }
    // Creamos el nodo raiz con las dimensiones totales de la mazmorra
    // lo posicionamos en el centro del mapa
    BSPNode rootNode = new BSPNode();
    rootNode.size = new Vector2(DUNGEON_WIDTH, DUNGEON_HEIGHT);
    rootNode.position = new Vector2(transform.position.x + DUNGEON_WIDTH / 2, transform.position.z + DUNGEON_HEIGHT / 2);
    rootNode.branch = Branch.ROOT;
    rootNode.level = 0;
    bspTree = new BSPTree(rootNode);

    // Crea los nodos del arbol subdividiendo el plano inicial
    // finalmente lanza la creacion de las habitaciones, pasillos y paredes de la mazmorra
    StartCoroutine(CreateDungeon());
  }

  IEnumerator CreateDungeon()
  {
    //Random.seed = 3; Si utilizamos una semilla fija la generacion es siempre igual
    int step = 0;
    while (step < 7)
    {
      Split(bspTree.Root);
      step++;

      yield return null;
    }

    // Dibujamos el resultado final del particionado
    //DrawLeafs(bspTree.Root);

    // Crea las habitaciones
    CreateRooms(bspTree.Root);
    // Conecta las habitaciones con pasillos
    //ConnectRooms(bspTree.Root);

    //// Limpieza con automata celular
    //for (int k = 0; k < 5; k++) {
    //  for (int i = 0; i < levelGrid.GetWidth(); i++) {
    //    for (int j = 0; j < levelGrid.GetHeight(); j++) {
    //      RemoveSingles(i, j);
    //    }
    //  }
    //}

    PopulateDungeon(bspTree.Root);

    DrawLevel();

    // Guarda los resultados a fichero
    SaveToFile();
  }

  // Situa al jugador en la entrada de la mazmorra
  private void PlacePlayer(Room room)
  {
    Vector3 position = room.transform.position;
    position.y += 0.1f;
    GameObject.FindGameObjectWithTag("Player").transform.position = position;
  }

  // Coloca el objeto de salida de la mazmorra
  private void PlaceExit(Room room)
  {
    Vector2i coord = Vector2i.ToVector2i(room.transform.position);
    objectsGrid.SetTile(coord.x, coord.z, (int)ObjectType.EXIT);
  }

  private Vector2i GetRandomCoordinateInRoom(Room room)
  {
    bool found = false;
    int tries = 0;
    int rx = room.position.x + 1;
    int rz = room.position.z + 1;
    while (!found && tries < 10)
    {
      rx = Random.Range(room.position.x + 1, room.position.x + room.size.x - 1);
      rz = Random.Range(room.position.z + 1, room.position.z + room.size.z - 1);

      if (levelGrid.GetTile(rx, rz) == 0)
      {
        found = true;
      }
    }
    return new Vector2i(rx, rz);
  }

  // Situa ememigos dentro de la habitacion
  private void PlaceItems(Room room)
  {
    Vector3 position = room.transform.position;
    // Actualiza el grid de objetos
    Vector2i coord = GetRandomCoordinateInRoom(room);

    int r = Random.Range(0, 100);
    // Crea un item segun su probabilidad
    if (r >= 0 && r < 60) // Coin = 50%
    {
      objectsGrid.SetTile(coord.x, coord.z, (int)ObjectType.COIN);
    }
    else if (r >= 60 && r < 80) // Potion
    {
      objectsGrid.SetTile(coord.x, coord.z, (int)ObjectType.POTION_HEALTH);
    }
    else if (r >= 80 && r < 100) // Chest
    {
      objectsGrid.SetTile(coord.x, coord.z, (int)ObjectType.CHEST);
    }
  }

  // Coloca trampas en la habitacion
  private void PlaceTraps(Room room)
  {
    Vector2i start = room.position;
    Vector2i end = room.position + room.size;
    // Rodear los cofres con pinchos
    // Aunque la geometria de la habitacion haya sido modificada seguimos contando con el mismo
    // area para recorrer su parte del grid
    for (int i = start.x; i < end.x; i++)
    {
      for (int j = start.z; j < end.z; j++)
      {
        if (objectsGrid.GetTile(i, j) == (int)ObjectType.CHEST)
        {
          // Recorremos las celdas adyacentes colocando trampas
          for (int x = i - 1; x <= i + 1; x++)
          {
            for (int z = j - 1; z <= j + 1; z++)
            {
              if (!(x == i && z == j))
              { // No a si mismo
                if (levelGrid.GetTile(x, z) == 0)
                { // El espacio del escenario esta vacio
                  objectsGrid.SetTile(x, z, (int)ObjectType.TRAP_SPIKES_FLOOR);
                }
              }
            }
          }
        }
      }
    }
  }

  // Coloca enemigos en la habitacion
  private void PlaceEnemies(Room room)
  {
    //Vector3 position = room.transform.position;
    //int r = Random.Range(0, 2);

    //switch (r) {
    //  case 0: // Arrows
    //    position.y = EnemyCrab.transform.position.y;
    //    Instantiate(EnemyCrab, position, Traps_Arrows.transform.rotation);
    //    break;
    //  case 1: // Floor
    //    position.y = EnemyGoblin.transform.position.y;
    //    Instantiate(EnemyGoblin, position, Traps_FloorSpikes.transform.rotation);
    //    break;
    //  case 2: // Wall
    //    position.y = EnemyBat.transform.position.y;
    //    Instantiate(EnemyBat, position, Traps_WallSpikes.transform.rotation);
    //    break;
    //}
  }

  private void PopulateDungeon(BSPNode pNode)
  {
    // Si hay nodo izquierdo nos movemos un nivel
    if (pNode.GetLeftNode() != null)
    {
      PopulateDungeon(pNode.GetLeftNode());
    }
    else
    {// Hoja con habitacion
      Room room = pNode.GetRoom().GetComponent<Room>();

      switch (room.type)
      {
        case RoomType.DEFAULT:
          PlaceItems(room);
          PlaceTraps(room);
          break;
        case RoomType.PASSAGE:
          break;
        case RoomType.ENTRANCE:
          PlacePlayer(room);
          break;
        case RoomType.EXIT:
          PlaceExit(room);
          break;
        default:
          break;
      }
      return;
    }
    if (pNode.GetRightNode() != null)
    {
      PopulateDungeon(pNode.GetRightNode());
    }
  }

  //split the tree
  public void Split(BSPNode pNode)
  {
    // Si hay nodo izquierdo nos movemos un nivel
    if (pNode.GetLeftNode() != null)
    {
      Split(pNode.GetLeftNode());
    }
    else
    {
      // Cuando no hay nodos hijos, intenta crearlos
      pNode.Cut();
      return;
    }
    if (pNode.GetRightNode() != null)
    {
      Split(pNode.GetRightNode());
    }
  }

  public Grid GetGrid()
  {
    return levelGrid;
  }

  public void SetTile(int x, int y, int value)
  {
    levelGrid.SetTile(x, y, value);
  }

  // Agrega una habitacion dentro de los limites del espacio del nodo
  private void AddRoom(BSPNode pNode)
  {
    // Las dimensiones minimas de la habitacion seran la mitad de NODE_MIN_SIZE
    // Las dimensiones maximas seran NODE_MIN_SIZE - ROOM_MARGIN
    Vector3 roomPosition = new Vector3(pNode.position.x, 0f, pNode.position.y);
    GameObject aRoom = (GameObject)Instantiate(baseRoom, roomPosition, Quaternion.identity);
    aRoom.transform.localScale = new Vector3((int)(Random.Range(NODE_MIN_SIZE / 2, pNode.size.x - ROOM_MARGIN)),
                                             aRoom.transform.localScale.y,
                                             (int)(Random.Range(NODE_MIN_SIZE / 2, pNode.size.y - ROOM_MARGIN)));
    // Configuracion de la habitacion
    Room roomScript = aRoom.GetComponent<Room>();
    // Id unico
    roomScript.SetID(nextId);
    nextId++;
    roomScript.type = RoomType.DEFAULT;
    aRoom.name = pNode.GetBranchName() + "-" + pNode.level.ToString();
    // Escribe en el grid la estructura de la habitacion
    levelGrid = roomScript.Setup(levelGrid);
    roomScript.SetParentNode(pNode);
    pNode.SetRoom(aRoom);
  }

  public void DrawNode(BSPNode pNode)
  {
    GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    cube1.transform.localScale = new Vector3(pNode.size.x, 0.1f, pNode.size.y);
    cube1.transform.position = new Vector3(pNode.position.x, 0f, pNode.position.y);
    cube1.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f),
                                                              Random.Range(0.0f, 1.0f),
                                                              Random.Range(0.0f, 1.0f));
    cube1.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Color");
  }

  // Dibuja en la escena las ultimas subdivisiones en la creacion de la mazmorra
  private void DrawLeafs(BSPNode pNode)
  {
    if (pNode.GetLeftNode() != null)
    {
      DrawLeafs(pNode.GetLeftNode());
    }
    else
    {
      DrawNode(pNode);
      return;
    }

    if (pNode.GetRightNode() != null)
    {
      DrawLeafs(pNode.GetRightNode());
    }
  }

  private void CreateRooms(BSPNode pNode)
  {
    if (pNode.GetLeftNode() != null)
    {
      CreateRooms(pNode.GetLeftNode());
    }
    else
    {
      AddRoom(pNode);

      if (!entranceCreated && pNode.branch == Branch.LEFT)
      {
        pNode.GetRoom().GetComponent<Room>().type = RoomType.ENTRANCE;
        entranceCreated = true;
      }

      if (!exitCreated && pNode.branch == Branch.RIGHT)
      {
        pNode.GetRoom().GetComponent<Room>().type = RoomType.EXIT;
        exitCreated = true;
      }
      return;
    }

    if (pNode.GetRightNode() != null)
    {
      CreateRooms(pNode.GetRightNode());
    }
  }

  // Crea los pasillos para conectar las habitaciones
  // Recorre recursivamente el arbol
  private void ConnectRooms(BSPNode pNode)
  {
    // Recorrido por los nodos de la parte izquierda
    if (pNode.GetLeftNode() != null)
    {
      ConnectRooms(pNode.GetLeftNode());

      if (pNode.GetRoom() != null)
      {
        pNode.GetRoom().GetComponent<Room>().Connect();
        return;
      }
    }
    else
    {
      if (pNode.GetRoom() != null)
      {
        pNode.GetRoom().GetComponent<Room>().Connect();
        return;
      }
    }
    // Recorrido por los nodos de la parte derecha
    if (pNode.GetRightNode() != null)
    {
      ConnectRooms(pNode.GetRightNode());

      if (pNode.GetRoom() != null)
      {
        pNode.GetRoom().GetComponent<Room>().Connect();
        return;
      }
    }
    else
    {
      if (pNode.GetRoom() != null)
      {
        pNode.GetRoom().GetComponent<Room>().Connect();
        return;
      }
    }
  }

  private void DrawLevel()
  {
    // Creamos el suelo con un quad escalado
    //    GameObject rootArea = GameObject.CreatePrimitive(PrimitiveType.Quad);
    //    // Escalamos en x, y (no z) ya que el quad esta rotado 90º
    //    rootArea.transform.localScale = new Vector3(levelWidth, levelHeight, 1);
    //    rootArea.transform.Rotate(new Vector3(90, 0, 0));
    //    rootArea.transform.position = new Vector3(transform.position.x + levelWidth / 2,
    //                                              1,
    //                                              transform.position.z + levelHeight / 2);

    // Instanciamos el prefab del suelo y lo escalamos
    GameObject floor = Instantiate(floorTile);
    floor.transform.position = new Vector3(transform.position.x + DUNGEON_WIDTH / 2, 0f, transform.position.z + DUNGEON_HEIGHT / 2);
    floor.transform.localScale = new Vector3(DUNGEON_WIDTH, DUNGEON_HEIGHT, 1);
    // Establecemos el tiling del material con respecto a sus dimensiones, un tile por unidad
    floor.GetComponent<Renderer>().material.mainTextureScale = new Vector2(DUNGEON_WIDTH, DUNGEON_HEIGHT);

    for (int i = 0; i < DUNGEON_WIDTH; i++)
    {
      for (int j = 0; j < DUNGEON_HEIGHT; j++)
      {
        // Escenario
        if (levelGrid.GetTile(i, j) > 0)
        {
          //Instantiate(wallTile, new Vector3(transform.position.x - (transform.localScale.x / 2) + i, transform.position.y + transform.localScale.y / 2, transform.position.z - (transform.localScale.z / 2) + j), Quaternion.identity);
          Instantiate(wallTile, new Vector3(transform.position.x + 0.5f + i, transform.position.y + transform.localScale.y / 2, transform.position.z + 0.5f + j), Quaternion.identity);
        }
        // Objetos
        Vector3 position = new Vector3(i + 0.5f, 0f, j + 0.5f); // En el centro de la celda
        switch (objectsGrid.GetTile(i, j))
        {
          case (int)ObjectType.COIN:
            Instantiate(Coin, position, Coin.transform.rotation);
            break;
          case (int)ObjectType.CHEST:
            Instantiate(Chest, position, Chest.transform.rotation);
            break;
          case (int)ObjectType.POTION_HEALTH:
            Instantiate(PotionHealth, position, PotionHealth.transform.rotation);
            break;
          case (int)ObjectType.TRAP_SPIKES_FLOOR:
            Instantiate(Traps_FloorSpikes, position, Traps_FloorSpikes.transform.rotation);
            break;
          case (int)ObjectType.EXIT:
            Instantiate(Portal, position, Portal.transform.rotation);
            break;
          default:
            break;
        }
      }
    }
  }

  // Automata celular para limpiar
  private void RemoveSingles(int x, int y)
  {
    int count = 0;

    if (x < levelGrid.GetWidth() - 1 && x > 1 && y > 1 && y < levelGrid.GetHeight() - 1)
    {
      if (levelGrid.GetTile(x + 1, y) == 1)
        count++;
      if (levelGrid.GetTile(x - 1, y) == 0)
        return;
      if (levelGrid.GetTile(x + 1, y) == 0)
        return;
      if (levelGrid.GetTile(x, y + 1) == 0)
        return;
      if (levelGrid.GetTile(x, y - 1) == 0)
        return;

      if (levelGrid.GetTile(x - 1, y) == 1)
        count++;
      if (levelGrid.GetTile(x, y + 1) == 1)
        count++;
      if (levelGrid.GetTile(x, y - 1) == 1)
        count++;
      if (levelGrid.GetTile(x - 1, y) == 1)
        count++;
      if (levelGrid.GetTile(x - 1, y - 1) == 1)
        count++;
      if (levelGrid.GetTile(x + 1, y - 1) == 1)
        count++;
      if (levelGrid.GetTile(x - 1, y + 1) == 1)
        count++;
      if (levelGrid.GetTile(x + 1, y + 1) == 1)
        count++;

      if (count >= 5)
        levelGrid.SetTile(x, y, 1);
    }
  }

  // Vuelca a fichero el resultado de la generación
  private void SaveToFile()
  {
    string fileName = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
    fileName = "bsp_" + fileName;
    string content = "Dungeon Generator (BSP)";
    //content += "\nSeed: " + seed;
    content += "\nDimensions: " + DUNGEON_WIDTH + "x" + DUNGEON_HEIGHT;
    content += "\nRoom min size: " + NODE_MIN_SIZE / 2;
    content += "\n\n";

    // Layout de las habitaciones y pasillos
    for (int x = 0; x < DUNGEON_WIDTH; x++) // alto = filas
    {
      for (int z = 0; z < DUNGEON_HEIGHT; z++) // ancho = columnas
      {
        if (levelGrid.GetTile(x, z) == 0)
        {
          content += "-";
        }
        else
        {
          content += levelGrid.GetTile(x, z);
        }
      }
      content += "\n";
    }
    content += "\n\n";

    // Habitaciones + objetos/trampas
    for (int x = 0; x < DUNGEON_WIDTH; x++) // alto = filas
    {
      for (int z = 0; z < DUNGEON_HEIGHT; z++) // ancho = columnas
      {
        if (objectsGrid.GetTile(x, z) == 0)
        {
          if (levelGrid.GetTile(x, z) == 0)
          {
            content += "-";
          }
          else
          {
            content += levelGrid.GetTile(x, z);
          }
        }
        else
        {
          content += objectsGrid.GetTile(x, z);
        }
      }
      content += "\n";
    }

    // Escribe a fichero
    fileName = DebugTools.instance.METRICS_PATH + "BSP/" + fileName + @".txt";
    DebugTools.instance.WriteToFile(fileName, content);
  }
}
