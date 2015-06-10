using UnityEngine;
using System.Collections;

// Tipos de habitaciones
public enum RoomType
{
  DEFAULT,
  ENTRANCE, // Habitacion inicial donde comienza el jugador
  EXIT,     // Habitacion final de la mazmorra
  PASSAGE   // Pasillo que une dos habitaciones
}

public class Room : MonoBehaviour
{
  public GameObject digger;

  public RoomType type;
  // Posicion y dimensiones en enteros. Son unidades de grid.
  public Vector2i position;
  public Vector2i size;
  private int roomId;
  private BSPNode parentNode;
  private GameObject sibling;

  // Establece la geometria de la habitacion sobre el grid
  public Grid Setup(Grid grid)
  {
    // Pasamos la posicion y escala a unidades de grid
    // La posicion se situa en la esquina superior izquierda, la primera casilla de la habitacion
    this.size = new Vector2i((int)transform.localScale.x, (int)transform.localScale.z);
    this.position = new Vector2i((int)(transform.position.x - (size.x / 2)), (int)(transform.position.z - (size.z / 2)));

    // Obtenemos la posicion sobre el grid
    //Vector2i startPosition = new Vector2i((int)(transform.position.x - (transform.localScale.x / 2)),
    //                                (int)(transform.position.z - (transform.localScale.z / 2)));

    // Inicializa todos los tiles de la habitacion
    for (int i = position.x; i < position.x + size.x; i++)
    {
      for (int j = position.z; j < position.z + size.z; j++)
      {
        grid.SetTile(i, j, 0);
      }
    }

    // Paredes de la habitacion
    for (int i = 0; i <= size.x; i++)
    {
      grid.SetTile(position.x + i, position.z, roomId); // Arriba
      grid.SetTile(position.x + i, position.z + size.z, roomId); // Abajo
    }
    for (int i = 0; i <= size.z; i++)
    {
      grid.SetTile(position.x, position.z + i, roomId); // Izquierda
      grid.SetTile(position.x + size.x, position.z + i, roomId); // Derecha
    }

    return grid;
  }

  public void SetID(int aID)
  {
    roomId = aID;
  }

  public void SetParentNode(BSPNode aNode)
  {
    parentNode = aNode;
  }

  public void Connect()
  {
    // Recoge el nodo hermano
    GetSibiling();
    if (sibling != null)
    {
      Room siblingRoom = sibling.GetComponent<Room>();

      Vector3 startPos = new Vector3();
      Vector3 endPos = new Vector3();

      if (siblingRoom.position.z + siblingRoom.size.z < position.z) // Habitacion (norte) y hermana (sur)
      {
        startPos = ChooseDoorPoint(0); // Punto sur en habitacion
        endPos = sibling.GetComponent<Room>().ChooseDoorPoint(2); // Punto norte en hermana
      }
      else if (siblingRoom.position.z > position.z + size.z) // Habitacion (sur) y hermana (norte)
      {
        startPos = ChooseDoorPoint(2);
        endPos = sibling.GetComponent<Room>().ChooseDoorPoint(1);
      }
      else if (siblingRoom.position.x + siblingRoom.size.x < position.x) // Habitacion (este) y hermana (oeste)
      {
        startPos = ChooseDoorPoint(3);
        endPos = sibling.GetComponent<Room>().ChooseDoorPoint(1);
      }
      else if (siblingRoom.position.x > position.x + size.x) // Habitacion (oeste) y hermana (este)
      {
        startPos = ChooseDoorPoint(1);
        endPos = sibling.GetComponent<Room>().ChooseDoorPoint(3);
      }

      GameObject aDigger = (GameObject)Instantiate(digger, startPos, Quaternion.identity);
      aDigger.GetComponent<Digger>().Dig(endPos);

      // Buscamos un padre sin habitacion (normalmente el padre inmediato)
      parentNode = FindRoomlessParent(parentNode);
      if (parentNode != null)
      {
        // Decidimos aleatoriamente 50% que habitacion se le asigna al padre
        // Si el nodo derecho o izquierdo. Asi luego conectamos mas zonas
        int aC = Random.Range(0, 2);
        if (aC == 0)
        {
          parentNode.SetRoom(this.gameObject);
        }
        else
        {
          parentNode.SetRoom(sibling.gameObject);
        }
        sibling.GetComponent<Room>().SetParentNode(parentNode);
      }

      Destroy(aDigger);
    }
  }

  // Obtiene el nodo hermano
  private void GetSibiling()
  {
    if (parentNode.GetParentNode() != null)
    {
      if (parentNode.GetParentNode().GetLeftNode() != parentNode)
      {
        sibling = parentNode.GetParentNode().GetLeftNode().GetRoom();
      }
      else
      {
        sibling = parentNode.GetParentNode().GetRightNode().GetRoom();
      }
    }
  }

  public Vector3 ChooseDoorPoint(int index)
  {
    switch (index)
    {
      case 0: // Sur, se crea puerta en X, z
        return new Vector3((int)(position.x + Random.Range(1, size.x - 2)), transform.position.y, (int)(position.z));
      case 1: // Este, se crea puerta en x + size.x, Z
        return new Vector3((int)(position.x + size.x), transform.position.y, (int)(position.z + Random.Range(1, size.z - 2)));
      case 2: // Norte, se crea puerta en X, z + size.z
        return new Vector3((int)(position.x + Random.Range(1, size.x - 2)), transform.position.y, (int)(position.z + size.z));
      case 3: // Oeste, se crea puerta en x, Z
        return new Vector3((int)(position.x + 1), transform.position.y, (int)(position.z + Random.Range(1, size.z - 2)));
      default:
        return new Vector3(0, 0, 0);
    }
  }

  public BSPNode GetParent()
  {
    return parentNode;
  }

  // Sube por los padres hasta encontrar uno que no tenga habitacion
  public BSPNode FindRoomlessParent(BSPNode aNode)
  {
    if (aNode != null)
    {
      if (aNode.GetRoom() == null)
      {
        return aNode;
      }
      else
      {
        return FindRoomlessParent(aNode.GetParentNode());
      }
    }

    return null;
  }
}
