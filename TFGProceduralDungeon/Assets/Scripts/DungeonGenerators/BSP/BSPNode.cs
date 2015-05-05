using UnityEngine;
using System.Collections;

public class BSPNode
{
	
	
  GameObject cube;
  BSPNode parentNode;
  BSPNode leftNode;
  BSPNode rightNode;
  Color myColor;
	
  private bool isConnected;
	
  GameObject room;
	
  public BSPNode()
  {
    isConnected = false;
    myColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
  }
	
  public void SetLeftNode(BSPNode _aNode)
  {
    leftNode = _aNode;
  }
	
  public void SetRightNode(BSPNode _aNode)
  {
    rightNode = _aNode;
  }
	
  public void SetParentNode(BSPNode _aNode)
  {
    parentNode = _aNode;	
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
	
  void SplitX(GameObject _aSection)
  {
		
    float xSplit = Random.Range(20, _aSection.transform.localScale.x - 20);
		
    if(xSplit > 20)
    {
      GameObject cube0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
      cube0.transform.localScale = new Vector3(xSplit, _aSection.transform.localScale.y, _aSection.transform.localScale.z);
      cube0.transform.position = new Vector3(
				_aSection.transform.position.x - ((xSplit - _aSection.transform.localScale.x) / 2),
				_aSection.transform.position.y,
				_aSection.transform.position.z);
      cube0.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
      cube0.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Color");
      //cube0.tag = "GenSection";
      leftNode = new BSPNode();
      leftNode.SetCube(cube0);
      leftNode.SetParentNode(this);
			
      GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
      float split1 = _aSection.transform.localScale.x - xSplit;
      cube1.transform.localScale = new Vector3(split1, _aSection.transform.localScale.y, _aSection.transform.localScale.z);
      cube1.transform.position = new Vector3(
				_aSection.transform.position.x + ((split1 - _aSection.transform.localScale.x) / 2),
				_aSection.transform.position.y,
				_aSection.transform.position.z);
      cube1.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
      cube1.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Color");
      //cube1.tag = "GenSection";
      rightNode = new BSPNode();
      rightNode.SetCube(cube1);
      rightNode.SetParentNode(this);
			
      GameObject.DestroyImmediate(_aSection);
    }		
  }
	
  void SplitZ(GameObject _aSection)
  {
    float zSplit = Random.Range(20, _aSection.transform.localScale.z - 20);
    float zSplit1 = _aSection.transform.localScale.z - zSplit;
		
    if(zSplit > 20)
    {
		
      GameObject cube0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
      cube0.transform.localScale = new Vector3(_aSection.transform.localScale.x, _aSection.transform.localScale.y, zSplit);
      cube0.transform.position = new Vector3(
				_aSection.transform.position.x,
				_aSection.transform.position.y,
				_aSection.transform.position.z - ((zSplit - _aSection.transform.localScale.z) / 2));
      cube0.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
      cube0.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Color");
      //cube0.tag = "GenSection";
      leftNode = new BSPNode();
      leftNode.SetCube(cube0);	
      leftNode.SetParentNode(this);
			
      GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
      cube1.transform.localScale = new Vector3(_aSection.transform.localScale.x, _aSection.transform.localScale.y, zSplit1);
      cube1.transform.position = new Vector3(
				_aSection.transform.position.x,
				_aSection.transform.position.y,
				_aSection.transform.position.z + ((zSplit1 - _aSection.transform.localScale.z) / 2));
      cube1.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
      cube1.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Color");
      //cube1.tag = "GenSection";
      rightNode = new BSPNode();
      rightNode.SetCube(cube1);
      rightNode.SetParentNode(this);
			
      GameObject.DestroyImmediate(_aSection);
    }
  }
	
  public void SetCube(GameObject _aCube)
  {
    cube = _aCube;	
  }
	
  public GameObject GetCube()
  {
    return cube;	
  }
	
  public void Cut()
  {
    float choice = Random.Range(0, 2);
    if(choice <= 0.5)
    {
      SplitX(cube);	
    }
    else
    {
      SplitZ(cube);	
    }
  }
	
  public Color GetColor()
  {
    return myColor;	
  }
	
  public void SetRoom(GameObject _aRoom)
  {
    room = _aRoom;	
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
