using UnityEngine;
using System.Collections;

public class TestGenerators : MonoBehaviour
{
  // Generadores
  public GeneratorGrowingTree prefabGeneratorGT; // Growing Tree
  private GeneratorGrowingTree instanceGeneratorGT;
  public GeneratorCA prefabGeneratorCA; // Cellular Automata
  private GeneratorCA instanceGeneratorCA;
  public GeneratorBSP prefabGeneratorBSP; // BSP Tree
  private GeneratorBSP instanceGeneratorBSP;

  // OPCIONES DE LOS TEST
  private int selectedAlg = 0;
  // Los distintos generadores para los test
  public GameObject settingsSize;
  public int DUNGEON_WIDTH = 100;
  public int DUNGEON_HEIGHT = 100;

  // Growing Tree
  public GameObject settingsGT;
  private float delay = 0f;
  private bool random = false;

  // Automata celular
  public GameObject settingsCA; // Instancia del panel de opciones
  private int cellularAutomataPasses = 1;
  private int wallProbability = 35;

  // BSP Tree
  public GameObject settingsBSP;

  // Setters para los sliders de los test
  public void SetDelay(float value)
  {
    delay = value;
  }

  public void SetRandom(bool value)
  {
    random = value;
  }

  public void SetDungeonWidth(float value)
  {
    DUNGEON_WIDTH = (int)value;
  }

  public void SetDungeonHeight(float value)
  {
    DUNGEON_HEIGHT = (int)value;
  }

  public void SetCAPasses(float value)
  {
    cellularAutomataPasses = (int)value;
  }

  public void SetCAProbability(float value)
  {
    wallProbability = (int)value;
  }

  public void SetBSPRoomSize(float value)
  {
    GeneratorBSP.ROOM_SIZE = value;
  }

  // Activa las opciones del algoritmo seleccionado
  public void OnAlgSelected(float selection)
  {
    selectedAlg = (int)selection;

    settingsSize.SetActive(false);
    settingsGT.SetActive(false);
    settingsCA.SetActive(false);
    settingsBSP.SetActive(false);
    switch (selectedAlg)
    {
      case 0: // Growing Tree
        settingsGT.SetActive(true);
        break;
      case 1: // Cellular Automata3
        settingsCA.SetActive(true);
        settingsSize.SetActive(true);
        break;
      case 2: // BSP Tree
        settingsBSP.SetActive(true);
        settingsSize.SetActive(true);
        break;
    }
  }
  // Boton de comenzar a ejecutar el algoritmo
  public void OnRun()
  {
    switch (selectedAlg)
    {
      case 0: // Growing Tree
        GenerateDungeonGrowingTree();
        break;
      case 1: // Cellular Automata3
        GenerateDungeonCA();
        break;
      case 2: // BSP Tree
        GenerateDungeonBSP();
        break;
    }
  }

  public void Cleanup()
  {
    if (instanceGeneratorGT != null)
    {
      instanceGeneratorGT.Cleanup();
      Destroy(instanceGeneratorGT.gameObject);
    }

    if (instanceGeneratorCA != null)
    {
      instanceGeneratorCA.Cleanup();
      Destroy(instanceGeneratorCA.gameObject);
    }

    if (instanceGeneratorBSP != null)
    {
      instanceGeneratorBSP.Cleanup();
      Destroy(instanceGeneratorBSP.gameObject);
    }
  }

  // Genera la mazmorra usando el algoritmo de Growing Tree
  private void GenerateDungeonGrowingTree()
  {
    Cleanup();
    instanceGeneratorGT = Instantiate(prefabGeneratorGT);
    instanceGeneratorGT.name = "GeneratorGrowingTree";
    instanceGeneratorGT.Generate(delay, random);
  }

  // Genera la mazmorra usando el algoritmo de Automata Celular
  private void GenerateDungeonCA()
  {
    Cleanup();
    instanceGeneratorCA = Instantiate(prefabGeneratorCA);
    instanceGeneratorCA.name = "GeneratorCA";
    instanceGeneratorCA.Generate(DUNGEON_WIDTH, DUNGEON_HEIGHT, cellularAutomataPasses, wallProbability);
  }

  // Genera la mazmorra usando el algoritmo de BSP Tree
  private void GenerateDungeonBSP()
  {
    Cleanup();
    instanceGeneratorBSP = Instantiate(prefabGeneratorBSP);
    instanceGeneratorBSP.name = "GeneratorBSP";
    instanceGeneratorBSP.Generate(DUNGEON_WIDTH, DUNGEON_HEIGHT);
  }


}
