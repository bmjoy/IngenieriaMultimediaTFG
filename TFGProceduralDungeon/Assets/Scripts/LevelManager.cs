using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
  public int MIN_DUNGEON_DIM = 50;
  public GameObject prefDungeonGenerator;
  public DungeonGenerator dungeonGenerator;

  // Numero del nivel actual
  public int level = 0;

  public void Init()
  {
    dungeonGenerator = Instantiate(prefDungeonGenerator).GetComponent<DungeonGenerator>();
    dungeonGenerator.transform.parent = this.transform;

    int width = Random.Range(MIN_DUNGEON_DIM + (level * 5), MIN_DUNGEON_DIM + (level * 5) + 5);
    int height = Random.Range(MIN_DUNGEON_DIM + (level * 5), MIN_DUNGEON_DIM + (level * 5) + 5);
    Debug.Log("Dungeon dimensions: " + width + "x" + height);
    dungeonGenerator.GenerateDungeon(width, height);
  }

  public void LoadNextLevel()
  {
    level++;
    GameManager.Instance.LoadScene(SceneName.DungeonLevel);
  }

  public void SaveToFile()
  {
    if (dungeonGenerator != null)
    {
      dungeonGenerator.SaveToFile();
    }
  }

  public void Cleanup()
  {
    if (dungeonGenerator != null)
    {
      Destroy(dungeonGenerator.gameObject);
    }
  }
}
