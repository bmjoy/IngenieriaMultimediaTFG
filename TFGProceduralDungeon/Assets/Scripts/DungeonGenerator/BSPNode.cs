using UnityEngine;
using System.Collections;

public class BSPNode
{
  private BSPNode parentNode;
  private BSPNode leftNode;
  private BSPNode rightNode;
  private Color myColor;

  private bool isConnected;

  GameObject room;

  public Vector2 position;
  public Vector2 size;

  public BSPNode()
  {
    isConnected = false;
    myColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
  }

  public void SetLeftNode(BSPNode node)
  {
    leftNode = node;
  }

  public void SetRightNode(BSPNode node)
  {
    rightNode = node;
  }

  public void SetParentNode(BSPNode node)
  {
    parentNode = node;
  }

  public BSPNode GetLeftNode()
  {
    return leftNode;
  }

  public BSPNode GetRightNode()
  {
    return rightNode;
  }

  public BSPNode GetParentNode()
  {
    return parentNode;
  }

  void SplitX()
  {
    float roomSize = DungeonGenerator.ROOM_SIZE;
    float xSplit = Random.Range(roomSize, this.size.x - roomSize);
    float xSplit1 = this.size.x - xSplit;

    if (xSplit > roomSize)
    {
      // Creamos el nodo izquierdo con las dimensiones del nuevo corte
      leftNode = new BSPNode();
      leftNode.size = new Vector2(xSplit, this.size.y);
      leftNode.position = new Vector2(this.position.x - ((xSplit - this.size.x) / 2), this.position.y);
      leftNode.SetParentNode(this);

      // Creamos el nodo derecho con las dimensiones del nuevo corte
      rightNode = new BSPNode();
      // Espacio restante
      rightNode.size = new Vector2(xSplit1, this.size.y);
      rightNode.position = new Vector2(this.position.x + ((xSplit1 - this.size.x) / 2), this.position.y);
      rightNode.SetParentNode(this);
    }
  }

  void SplitZ()
  {
    float roomSize = DungeonGenerator.ROOM_SIZE;
    float zSplit = Random.Range(roomSize, this.size.y - roomSize);
    float zSplit1 = this.size.y - zSplit;

    if (zSplit > roomSize)
    {
      leftNode = new BSPNode();
      leftNode.size = new Vector2(this.size.x, zSplit);
      leftNode.position = new Vector2(this.position.x, this.position.y - ((zSplit - this.size.y) / 2));
      leftNode.SetParentNode(this);

      rightNode = new BSPNode();
      rightNode.size = new Vector2(this.size.x, zSplit1);
      rightNode.position = new Vector2(this.position.x, this.position.y + ((zSplit1 - this.size.y) / 2));
      rightNode.SetParentNode(this);
    }
  }

  public void Cut()
  {
    float choice = Random.Range(0, 2);
    if (choice <= 0.5)
    {
      SplitX();
    }
    else
    {
      SplitZ();
    }
  }

  public Color GetColor()
  {
    return myColor;
  }

  public void SetRoom(GameObject room)
  {
    this.room = room;
  }

  public GameObject GetRoom()
  {
    return room;
  }

  public void SetConnected()
  {
    isConnected = true;
  }

  public bool GetIsConnected()
  {
    return isConnected;
  }

}
