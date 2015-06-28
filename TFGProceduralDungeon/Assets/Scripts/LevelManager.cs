using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
  public int MIN_DUNGEON_DIM = 50;
  public GameObject prefDungeonGenerator;
  public GameObject dungeonGenerator;
  public Hud hud;
  
  // Timer para los minutos:segundos:milisegundos
  public float timer;

  public void Init()
  {
    int level = GameManager.Instance.level;
    dungeonGenerator = Instantiate(prefDungeonGenerator);

    int width = Random.Range(MIN_DUNGEON_DIM + (level * 5), MIN_DUNGEON_DIM + (level * 5) + 5);
    int height = Random.Range(MIN_DUNGEON_DIM + (level * 5), MIN_DUNGEON_DIM + (level * 5) + 5);
    dungeonGenerator.GetComponent<DungeonGenerator>().GenerateDungeon(width, height);
    // Comienza el temporizador
    timer = 0f;
  }

  void Update()
  {
    timer += Time.deltaTime;
  }

  // Muestra la pantalla final del nivel
  public void FinishLevel()
  {
    GameObject.Find("UIManager").GetComponent<UIManager>().OnLevelFinish();
  }

  // Carga el siguiente nivel
  public void LoadNextLevel()
  {
    GameManager.Instance.level++;
    GameManager.Instance.LoadScene((int)SceneName.DungeonLevel);
  }

  public void SaveToFile()
  {
    if(dungeonGenerator != null)
    {
      dungeonGenerator.GetComponent<DungeonGenerator>().SaveToFile();
    }
  }

  public void Cleanup()
  {
    DestroyImmediate(dungeonGenerator);
  }
}
