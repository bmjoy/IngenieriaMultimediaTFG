using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class DebugTools : MonoBehaviour
{
  private class LineObject
  {
    public GameObject parent;
    public LineRenderer line;

    public LineObject(GameObject p, LineRenderer l)
    {
      parent = p;
      line = l;
    }
  }

  // Instancia publica del singleton
  public static DebugTools instance = null;
  private List<LineObject> lineRendererList;

  public Material lineMaterial;

  void Awake()
  {
    // Inicializacion de la instancia del singleton
    if(instance == null)
    {
      instance = this;
    }
    else if(instance != this)
    {
      Destroy(gameObject);
    }
    DontDestroyOnLoad(gameObject);
  }

  void Start()
  {
    lineRendererList = new List<LineObject>();
  }

  void Update()
  {
    if(Input.GetKeyDown(KeyCode.R))
    {
      Application.LoadLevel(Application.loadedLevelName);
    }

//    for (int i = 0; i < lineRendererList.Count; i++)
//    {
//      LineObject l = lineRendererList[i];
//      l.line.SetPosition(0, l.parent.transform.position);
//    }
  }
  
  // Dibuja una linea 3D pero solo sobre un eje indicado
  // axis = ['x', 'y', 'z']
  public void AddCoordinateLines(GameObject parent, Vector3 start, Vector3 direction, char axis)
  {
    Vector3 end = start + direction;
    Color lineColor;
    // Cancela los otros ejes
    if(axis == 'x')
    {
      end = Vector3.Scale(end, new Vector3(1, 0, 0));
      lineColor = Color.red;
    }
    else if(axis == 'y')
    {
      end = Vector3.Scale(end, new Vector3(0, 1, 0));
      lineColor = Color.green;
    }
    else
    {
      end = Vector3.Scale(end, new Vector3(0, 0, 1));
      lineColor = Color.blue;
    }
    GameObject goLineRenderer = new GameObject();
    goLineRenderer.name = "Debug Line";
    LineRenderer coLineRenderer = goLineRenderer.AddComponent<LineRenderer>();
    // Configuracion de la linea a dibujar
    goLineRenderer.transform.position = start;
    coLineRenderer.SetPosition(0, start);
    coLineRenderer.SetPosition(1, end);
    coLineRenderer.shadowCastingMode = ShadowCastingMode.Off;
    coLineRenderer.receiveShadows = false;
    coLineRenderer.material = lineMaterial;
    coLineRenderer.SetColors(lineColor, lineColor);
    coLineRenderer.useLightProbes = false;
    coLineRenderer.SetWidth(0.05f, 0.05f);
    // Creamos el objeto con referencia a su parent y lo guardamos en la lista
    // para su futura actualizacion
    LineObject lineObject = new LineObject(parent, coLineRenderer);
    lineRendererList.Add(lineObject);
  }
}
