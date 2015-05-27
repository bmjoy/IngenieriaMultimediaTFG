using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
  public Grid levelGrid;

  private int roomID = 0;
  private BSPTree bspTree;
  private RoomCreator entrance;
  private BSPNode exit;

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
    bspTree = new BSPTree(rootNode);

    // Crea los nodos del arbol subdividiendo el plano inicial
    // finalmente lanza la creacion de las habitaciones, pasillos y paredes de la mazmorra
    StartCoroutine(CreateDungeon());
  }

  IEnumerator CreateDungeon()
  {
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
    ConnectRooms(bspTree.Root);

    //// Limpieza con automata celular
    for (int k = 0; k < 5; k++)
    {
      for (int i = 0; i < levelGrid.GetWidth(); i++)
      {
        for (int j = 0; j < levelGrid.GetHeight(); j++)
        {
          RemoveSingles(i, j);
        }
      }
    }
    CreateLevel();

    PopulateDungeon(bspTree.Root);
  }

  // Situa al jugador en la entrada de la mazmorra
  private void PlacePlayer(Vector3 position)
  {
    position.y += 5f;
    GameObject.FindGameObjectWithTag("Player").transform.position = position;
  }

  private void PopulateDungeon(BSPNode pNode)
  {
    // Si hay nodo izquierdo nos movemos un nivel
    if (pNode.GetLeftNode() != null)
    {
      PopulateDungeon(pNode.GetLeftNode());
    }
    else
    {
      // Hoja con habitacion
      RoomCreator room = pNode.GetRoom().GetComponent<RoomCreator>();
      if (room.type == RoomType.ENTRANCE)
      {
        PlacePlayer(room.transform.position);
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
    levelGrid = aRoom.GetComponent<RoomCreator>().Setup(levelGrid);
    aRoom.GetComponent<RoomCreator>().SetID(roomID);
    roomID++;
    aRoom.GetComponent<RoomCreator>().SetParentNode(pNode);
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

      if (entrance == null)
      {
        entrance = pNode.GetRoom().GetComponent<RoomCreator>();
        entrance.type = RoomType.ENTRANCE;
        entrance.name = "ENTRANCE";
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
        pNode.GetRoom().GetComponent<RoomCreator>().Connect();
        return;
      }
    }
    else
    {
      if (pNode.GetRoom() != null)
      {
        pNode.GetRoom().GetComponent<RoomCreator>().Connect();
        return;
      }
    }
    // Recorrido por los nodos de la parte derecha
    if (pNode.GetRightNode() != null)
    {
      ConnectRooms(pNode.GetRightNode());

      if (pNode.GetRoom() != null)
      {
        pNode.GetRoom().GetComponent<RoomCreator>().Connect();
        return;
      }
    }
    else
    {
      if (pNode.GetRoom() != null)
      {
        pNode.GetRoom().GetComponent<RoomCreator>().Connect();
        return;
      }
    }
  }

  private void CreateLevel()
  {
    int levelWidth = levelGrid.GetWidth();
    int levelHeight = levelGrid.GetHeight();
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
    floor.transform.position = new Vector3(transform.position.x + levelWidth / 2, 0f, transform.position.z + levelHeight / 2);
    floor.transform.localScale = new Vector3(levelWidth, levelHeight, 1);
    // Establecemos el tiling del material con respecto a sus dimensiones, un tile por unidad
    floor.GetComponent<Renderer>().material.mainTextureScale = new Vector2(levelWidth, levelHeight);

    for (int i = 0; i < levelWidth; i++)
    {
      for (int j = 0; j < levelHeight; j++)
      {
        switch (levelGrid.GetTile(i, j))
        {
          //case 1:
          //  Instantiate(floorTile, new Vector3(transform.position.x - (transform.localScale.x / 2) + i, transform.position.y + transform.localScale.y / 2, transform.position.z - (transform.localScale.z / 2) + j), Quaternion.identity);
          //  break;
          case 2:
            Instantiate(wallTile, new Vector3(transform.position.x - (transform.localScale.x / 2) + i, transform.position.y + transform.localScale.y / 2, transform.position.z - (transform.localScale.z / 2) + j), Quaternion.identity);
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
}
