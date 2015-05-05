using UnityEngine;
using System.Collections;

public class RoomCreator : MonoBehaviour
{
  public GameObject digger;

  private int roomID;
  private BSPNode parentNode;
  private GameObject sibiling;
  private DungeonGenerator dungeonGenerator;

  void Awake()
  {
    dungeonGenerator = GameObject.Find("DungeonGenerator").GetComponent<DungeonGenerator>();
  }

  public void Setup()
  {
    transform.position = new Vector3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
    transform.position = new Vector3(transform.position.x - (transform.localScale.x / 2), transform.position.y, transform.position.z - (transform.localScale.z / 2));
    for (int i = (int)transform.position.x; i < (int)transform.position.x + transform.localScale.x; i++)
    {
      for (int j = (int)transform.position.z; j < (int)transform.position.z + transform.localScale.z; j++)
      {
        dungeonGenerator.SetTile(i, j, 1);
      }
    }

    for (int i = 0; i < transform.localScale.x + 1; i++)
    {
      dungeonGenerator.SetTile((int)transform.position.x + i, (int)transform.position.z, 2);
      dungeonGenerator.SetTile((int)transform.position.x + i, (int)(transform.position.z + transform.localScale.z), 2);
    }

    for (int i = 0; i < transform.localScale.z + 1; i++)
    {
      dungeonGenerator.SetTile((int)transform.position.x, (int)transform.position.z + i, 2);
      dungeonGenerator.SetTile((int)(transform.position.x + transform.localScale.x), (int)transform.position.z + i, 2);
    }

  }

  public void SetID(int _aID)
  {
    roomID = _aID;
  }

  public void SetParentNode(BSPNode _aNode)
  {
    parentNode = _aNode;
  }

  public void Connect()
  {
    GetSibiling();

    if (sibiling != null)
    {

      Vector3 startPos = new Vector3();
      Vector3 endPos = new Vector3();

      if (sibiling.transform.position.z + sibiling.transform.localScale.z < transform.position.z)
      {
        startPos = ChooseDoorPoint(0);
        endPos = sibiling.GetComponent<RoomCreator>().ChooseDoorPoint(2);
      }
      else if (sibiling.transform.position.z > transform.position.z + transform.localScale.z)
      {
        startPos = ChooseDoorPoint(2);
        endPos = sibiling.GetComponent<RoomCreator>().ChooseDoorPoint(1);
      }
      else if (sibiling.transform.position.x + sibiling.transform.localScale.x < transform.position.x)
      {
        startPos = ChooseDoorPoint(3);
        endPos = sibiling.GetComponent<RoomCreator>().ChooseDoorPoint(1);
      }
      else if (sibiling.transform.position.x > transform.position.x + transform.localScale.x)
      {
        startPos = ChooseDoorPoint(1);
        endPos = sibiling.GetComponent<RoomCreator>().ChooseDoorPoint(3);
      }


      GameObject aDigger = (GameObject)Instantiate(digger, startPos, Quaternion.identity);
      aDigger.GetComponent<Digger>().Begin(endPos);


      parentNode = FindRoomlessParent(parentNode);

      if (parentNode != null)
      {

        int aC = Random.Range(0, 2);

        if (aC == 0)
        {
          parentNode.SetRoom(this.gameObject);
        }
        else
        {
          parentNode.SetRoom(sibiling.gameObject);
        }

        sibiling.GetComponent<RoomCreator>().SetParentNode(parentNode);
      }

    }

  }

  private void GetSibiling()
  {
    if (parentNode.GetParentNode() != null)
    {
      if (parentNode.GetParentNode().GetLeftNode() != parentNode)
      {
        sibiling = parentNode.GetParentNode().GetLeftNode().GetRoom();
      }
      else
      {
        sibiling = parentNode.GetParentNode().GetRightNode().GetRoom();
      }
    }
  }

  public Vector3 ChooseDoorPoint(int _index)
  {
    switch (_index)
    {
      case 0:
        return new Vector3((int)(transform.position.x + Random.Range(1, transform.localScale.x - 2)), transform.position.y, (int)(transform.position.z));
      case 1:
        return new Vector3((int)(transform.position.x + transform.localScale.x), transform.position.y, (int)(transform.position.z + Random.Range(1, transform.localScale.z - 2)));
      case 2:
        return new Vector3((int)(transform.position.x + Random.Range(1, transform.localScale.x - 2)), transform.position.y, (int)(transform.position.z + transform.localScale.z));
      case 3:
        return new Vector3((int)(transform.position.x + 1), transform.position.y, (int)(transform.position.z + Random.Range(1, transform.localScale.z - 2)));
      default:
        return new Vector3(0, 0, 0);
    }
  }

  public BSPNode GetParent()
  {
    return parentNode;
  }

  public BSPNode FindRoomlessParent(BSPNode _aNode)
  {
    if (_aNode != null)
    {
      if (_aNode.GetRoom() == null)
      {
        return _aNode;
      }
      else
      {
        return FindRoomlessParent(_aNode.GetParentNode());
      }
    }

    return null;
  }
}
