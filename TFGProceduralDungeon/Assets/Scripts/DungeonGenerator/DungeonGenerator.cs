using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{
  // Los distintos generadores para los test

  public int DUNGEON_WIDTH = 50;
  public int DUNGEON_HEIGHT = 50;
  public static float ROOM_SIZE = 10f;
  public GameObject baseRoom;
  public GameObject floorTile;
  public GameObject wallTile;
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
    // lo posicionamos en el centro segun las dimensiones
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

    // Limpieza con automata celular
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

    // Instanciamos el jugador en la entrada
    if (entrance != null)
    {
      Vector3 playerPosition = new Vector3(entrance.transform.position.x + (entrance.transform.localScale.x / 2), 10f, entrance.transform.position.z + (entrance.transform.localScale.z / 2));
      Debug.Log("position: " + entrance.transform.position + ", size: " + entrance.transform.localScale);
      Debug.Log(playerPosition);
      //Vector3 playerPosition = new Vector3(2f, 2f, 2f);
      GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;
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
    // Si hay 
    if (pNode.GetLeftNode() != null)
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

  private void AddRoom(BSPNode pNode)
  {
    Vector3 roomPosition = new Vector3(pNode.position.x, 0f, pNode.position.y);
    GameObject aRoom = (GameObject)Instantiate(baseRoom, roomPosition, Quaternion.identity);
    aRoom.transform.localScale = new Vector3((int)(Random.Range(10, pNode.size.x - 5)),
                                             aRoom.transform.localScale.y,
                                             (int)(Random.Range(10, pNode.size.y - 5)));
    aRoom.GetComponent<RoomCreator>().Setup();
    aRoom.GetComponent<RoomCreator>().SetID(roomID);
    aRoom.GetComponent<RoomCreator>().SetParentNode(pNode);
    pNode.SetRoom(aRoom);
    roomID++;
  }

  public void DrawNode(BSPNode pNode)
  {
    GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    cube1.transform.localScale = new Vector3(pNode.size.x, 0.1f, pNode.size.y);
    cube1.transform.position = new Vector3(
      pNode.position.x,
      0f,
      pNode.position.y);
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
