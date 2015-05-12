[System.Serializable]
public struct Vector2i
{

  public int x, z;

  public Vector2i(int x, int z)
  {
    this.x = x;
    this.z = z;
  }

  public static Vector2i operator +(Vector2i a, Vector2i b)
  {
    a.x += b.x;
    a.z += b.z;
    return a;
  }
}