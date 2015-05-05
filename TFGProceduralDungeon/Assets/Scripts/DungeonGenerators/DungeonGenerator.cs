using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{
  public int DUNGEON_WIDTH = 20;
  public int DUNGEON_HEIGHT = 20;
  public GameObject baseRoom;
  public GameObject floorTile;
  public GameObject wallTile;
  // Plano/rejilla sobre el que realizar las subdivisiones
  public Grid levelGrid;

  private int roomID = 0;
  private BSPTree bspTree;

  // Setters para los sliders de los test
  public void SetDungeonWidth(float value)
  {
    DUNGEON_WIDTH = (int)value;
  }

  public void SetDungeonHeight(float value)
  {
    DUNGEON_HEIGHT = (int)value;
  }

  // Genera la mazmorra usando el algoritmo de BSP Tree
  public void GenerateDungeonBSP()
  {
    // Creamos el cubo inicial
    GameObject rootArea = GameObject.CreatePrimitive(PrimitiveType.Cube);
    rootArea.transform.localScale = new Vector3(DUNGEON_WIDTH, 1, DUNGEON_HEIGHT);
    //startPlane.tag = "GenSection";
    rootArea.transform.position = new Vector3(transform.position.x + rootArea.transform.localScale.x / 2,
            transform.position.y,
            transform.position.z + rootArea.transform.localScale.z / 2);

    levelGrid = new Grid((int)rootArea.transform.localScale.x, (int)rootArea.transform.localScale.z);

    for(int i = 0; i < levelGrid.GetWidth(); i++)
    {
      for(int j = 0; j < levelGrid.GetHeight(); j++)
      {
        levelGrid.SetTile(i, j, 0);
      }
    }
    // Crea el arbol y agrega el primer nodo raiz
    bspTree = new BSPTree(rootArea);

    // Crea los nodos del arbol subdividiendo el plano inicial
    // finalmente lanza la creacion de las habitaciones, pasillos y paredes de la mazmorra
    StartCoroutine(CreateDungeon());
  }

  IEnumerator CreateDungeon()
  {
    int step = 0;
    while(step < 7)
    {
      if(Input.GetKeyDown(KeyCode.P))
      {
        Split(bspTree.Root);
        step++;
      }
      yield return null;
    }
    while(!Input.GetKeyDown(KeyCode.P))
    {
      yield return null;
    }
    // Crea las habitaciones
    CreateRooms(bspTree.Root);

    // Conecta las habitaciones con pasillos
    ConnectRooms(bspTree.Root);

    // Limpieza
    for(int k = 0; k < 5; k++)
    {
      for(int i = 0; i < levelGrid.GetWidth(); i++)
      {
        for(int j = 0; j < levelGrid.GetHeight(); j++)
        {
          RemoveSingles(i, j);
        }
      }
    }
    CreateLevel();
  }

  //split the tree
  public void Split(BSPNode pNode)
  {
    if(pNode.GetLeftNode() != null)
    {
      Split(pNode.GetLeftNode());
    }
    else
    {
      pNode.Cut();
      return;
    }

    if(pNode.GetLeftNode() != null)
    {
      Split(pNode.GetRightNode());
    }

  }

  public Grid GetGrid()
  {
    return levelGrid;
  }

  public void SetTile(int _x, int _y, int _value)
  {
    levelGrid.SetTile(_x, _y, _value);
  }

  private void AddRoom(BSPNode pNode)
  {

    GameObject aObj = pNode.GetCube();

    GameObject aRoom = (GameObject)Instantiate(baseRoom, aObj.transform.position, Quaternion.identity);
    aRoom.transform.localScale = new Vector3(
            (int)(Random.Range(10, aObj.transform.localScale.x - 5)),
            aRoom.transform.localScale.y,
            (int)(Random.Range(10, aObj.transform.localScale.z - 5)));
    aRoom.GetComponent<RoomCreator>().Setup();
    aRoom.GetComponent<RoomCreator>().SetID(roomID);
    aRoom.GetComponent<RoomCreator>().SetParentNode(pNode);
    pNode.SetRoom(aRoom);
    roomID++;
  }

  private void CreateRooms(BSPNode pNode)
  {
    if(pNode.GetLeftNode() != null)
    {
      CreateRooms(pNode.GetLeftNode());
    }
    else
    {
      AddRoom(pNode);
      return;
    }

    if(pNode.GetRightNode() != null)
    {
      CreateRooms(pNode.GetRightNode());
    }
  }

  private void ConnectRooms(BSPNode pNode)
  {
    if(pNode.GetLeftNode() != null)
    {
      ConnectRooms(pNode.GetLeftNode());

      if(pNode.GetRoom() != null)
      {
        pNode.GetRoom().GetComponent<RoomCreator>().Connect();

        return;
      }

    }
    else
    {
      if(pNode.GetRoom() != null)
      {
        pNode.GetRoom().GetComponent<RoomCreator>().Connect();

        return;
      }
    }

    if(pNode.GetRightNode() != null)
    {
      ConnectRooms(pNode.GetRightNode());

      if(pNode.GetRoom() != null)
      {
        pNode.GetRoom().GetComponent<RoomCreator>().Connect();

        return;
      }
    }
    else
    {
      if(pNode.GetRoom() != null)
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
    GameObject rootArea = GameObject.CreatePrimitive(PrimitiveType.Quad);
    // Escalamos en x, y (no z) ya que el quad esta rotado 90º
    rootArea.transform.localScale = new Vector3(levelWidth, levelHeight, 1);
    rootArea.transform.Rotate(new Vector3(90, 0, 0));
    rootArea.transform.position = new Vector3(transform.position.x + levelWidth / 2,
                                              1,
                                              transform.position.z + levelHeight / 2);

    for(int i = 0; i < levelWidth; i++)
    {
      for(int j = 0; j < levelHeight; j++)
      {
        switch(levelGrid.GetTile(i, j))
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
  private void RemoveSingles(int _x, int _y)
  {
    int count = 0;

    if(_x < levelGrid.GetWidth() - 1 && _x > 1 && _y > 1 && _y < levelGrid.GetHeight() - 1)
    {
      if(levelGrid.GetTile(_x + 1, _y) == 1)
      {
        count++;
      }

      if(levelGrid.GetTile(_x - 1, _y) == 0)
      {
        return;
      }

      if(levelGrid.GetTile(_x + 1, _y) == 0)
      {
        return;
      }

      if(levelGrid.GetTile(_x, _y + 1) == 0)
      {
        return;
      }

      if(levelGrid.GetTile(_x, _y - 1) == 0)
      {
        return;
      }


      //

      if(levelGrid.GetTile(_x - 1, _y) == 1)
      {
        count++;
      }

      if(levelGrid.GetTile(_x, _y + 1) == 1)
      {
        count++;
      }

      if(levelGrid.GetTile(_x, _y - 1) == 1)
      {
        count++;
      }

      if(levelGrid.GetTile(_x - 1, _y) == 1)
      {
        count++;
      }

      if(levelGrid.GetTile(_x - 1, _y - 1) == 1)
      {
        count++;
      }

      if(levelGrid.GetTile(_x + 1, _y - 1) == 1)
      {
        count++;
      }

      if(levelGrid.GetTile(_x - 1, _y + 1) == 1)
      {
        count++;
      }

      if(levelGrid.GetTile(_x + 1, _y + 1) == 1)
      {
        count++;
      }

      if(count >= 5)
      {
        levelGrid.SetTile(_x, _y, 1);
      }
    }
  }
}
