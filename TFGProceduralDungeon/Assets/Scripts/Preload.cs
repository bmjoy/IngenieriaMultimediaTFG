using UnityEngine;
using System.Collections;

// Simplemente carga la primera escena
// Esto se pone en un escena Preload con un GameManager (Singleton)
// de esta manera el singleton se crea solo una vez y no hay que destruir los duplicados
// que pueden dar problemas porque no mueren hasta final de frame y pueden ejecutar cosas
public class Preload : MonoBehaviour
{
  public SceneName sceneToLoad;
  void Start()
  {
    Application.LoadLevel((int)sceneToLoad);
  }
}
