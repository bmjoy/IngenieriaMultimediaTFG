using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// Tipos de tiles y numero identificador base de habitaciones
public enum TileType
{
  // Vacio
  EMPTY = 0,
  // Suelo
  FLOOR = 1,
  // Items
  COIN = 10,
  CHEST,
  POTION_HEALTH,
  // Trampas
  TRAP_SPIKES_FLOOR,
  TRAP_SPIKES_WALL,
  TRAP_ARROWS,
  TRAP_ROCK,
  ENEMY_CRAB,
  ENEMY_GOBLIN,
  EXIT,
  // Habitaciones y pasillos
  ROOM_ID = 50 // A partir de este id seran habitaciones y pasillos
}

// Direcciones para trampas y otros objetos. 
// Es importante que se mantenga este orden ya que:
public enum Direction
{
  NORTH = 0,
  EAST,
  SOUTH,
  WEST,
  NONE
}

public static class Tools
{
  public static bool IsBetween( this int item, int lower, int upper, bool inclusive )
  {
    return inclusive
        ? lower <= item && item <= upper
        : lower < item && item < upper;
  }
}

public class DungeonGenerator : MonoBehaviour
{
  // Dimensiones
  public int DUNGEON_WIDTH;
  public int DUNGEON_HEIGHT;
  // Dimensiones minimas del espacio donde se va a crear la habitacion
  public static float NODE_MIN_SIZE = 12f;
  public GameObject BaseRoom; // Objeto base para generar habitacion
  private ObjectManager objectManager;
  private Coroutine routine;

  // Margen dentro del espacio del nodo. Hay que tener en cuenta que sera el doble
  private const float ROOM_MARGIN = 4f;
  // Unidad base de los tiles/celdas del mapa. Corresponde a unidades del grid de Unity
  private const float TILE_UNIT = 1f;
  // ROOM
  // Las dimensiones minimas de la habitacion seran la mitad de NODE_MIN_SIZE
  // Las dimensiones maximas seran NODE_MIN_SIZE - ROOM_MARGIN


  // Plano/rejilla sobre el que realizar las subdivisiones
  [HideInInspector]
  public Grid
    levelGrid; // Habitaciones
  [HideInInspector]
  public Grid
    objectsGrid; // Items y trampas
  [HideInInspector]
  public int
    nextId;
  [HideInInspector]
  public int
    numRooms = 0; // Total de habitaciones (no pasillos) generadas
  [HideInInspector]
  public BSPTree
    bspTree;
  private List<Room> rooms = new List<Room>(); // Lista de habitaciones (no pasillos) para acceso rapido
  private int level; // Numero de numero para escoger el numero de iteraciones

  // ESTADISTICAS DE OBJETOS, ENEMIGOS...
  private int coinProbability = 60;
  private int chestProbability = 20;
  private int potionProbability = 20;
  private int numEnemigo = 0;
  private int numPociones = 0;
  private int dificultad = 0;

  // Setters para los sliders de los test
  public void SetDungeonWidth( float value )
  {
    DUNGEON_WIDTH = (int)value;
  }

  public void SetDungeonHeight( float value )
  {
    DUNGEON_HEIGHT = (int)value;
  }


  // Genera la mazmorra usando el algoritmo de BSP Tree
  public void GenerateDungeon( int width, int height )
  {
    objectManager = GameManager.Instance.objectManager;
    level = GameManager.Instance.levelManager.level;
    DUNGEON_WIDTH = width;
    DUNGEON_HEIGHT = height;
    levelGrid = new Grid( DUNGEON_WIDTH, DUNGEON_HEIGHT );
    objectsGrid = new Grid( DUNGEON_WIDTH, DUNGEON_HEIGHT );
    nextId = 0;
    numRooms = nextId;
    // Inicializamos los tiles a vacio
    for( int i = 0; i < levelGrid.GetWidth(); i++ )
    {
      for( int j = 0; j < levelGrid.GetHeight(); j++ )
      {
        levelGrid.SetTile( i, j, (int)TileType.EMPTY );
      }
    }
    // Creamos el nodo raiz con las dimensiones totales de la mazmorra
    // lo posicionamos en el centro del mapa
    BSPNode rootNode = new BSPNode();
    rootNode.size = new Vector2( DUNGEON_WIDTH, DUNGEON_HEIGHT );
    rootNode.position = new Vector2( transform.position.x + DUNGEON_WIDTH / 2, transform.position.z + DUNGEON_HEIGHT / 2 );
    rootNode.branch = Branch.ROOT;
    rootNode.level = 0;
    bspTree = new BSPTree( rootNode );

    // Crea los nodos del arbol subdividiendo el plano inicial
    // finalmente lanza la creacion de las habitaciones, pasillos y paredes de la mazmorra
    routine = StartCoroutine( CreateDungeon() );
  }

  IEnumerator CreateDungeon()
  {
    // Si utilizamos una semilla fija la generacion es siempre igual
    // para un nivel concreto
    //Random.seed = 3;

    int step = 0;
    int maxSteps = 8 + level;
    while( step < maxSteps )
    {
      Split( bspTree.Root );
      step++;
      yield return null;
    }

    // Dibujamos el resultado final del particionado
    //DrawLeafs(bspTree.Root);

    // Crea las habitaciones
    CreateRooms( bspTree.Root );
    // Conecta las habitaciones con pasillos
    ConnectRooms( bspTree.Root );

    // Limpieza con automata celular
    for( int k = 0; k < 5; k++ )
    {
      for( int i = 0; i < levelGrid.GetWidth(); i++ )
      {
        for( int j = 0; j < levelGrid.GetHeight(); j++ )
        {
          RemoveSingles( i, j );
        }
      }
    }

    // Objetos, trampas y enemigos
    SetStartEndPoints();
    PopulateDungeon( bspTree.Root );
    PopulatePassages();
    DrawLevel();
    dificultad = numEnemigo * 10 - numPociones * 5;
    Debug.Log( "Dificultad: " + dificultad );
  }

  private void SetStartEndPoints()
  {
    Room entrance = null;
    Room exit = null;
    //Debug.Log(rooms.Count);
    //for (int i = 0; i < rooms.Count; i++)
    //{
    //  Debug.Log(i + " -- " + rooms[i].GetNode());
    //}
    // Entrance
    for( int i = 0; i < rooms.Count; i++ )
    {
      Debug.Log(rooms[i].GetNode());
      if( rooms[i].GetNode().branch == Branch.LEFT )
      {
        entrance = rooms[i];
        entrance.type = RoomType.ENTRANCE;
        break;
      }
    }
    // Exit
    if( entrance != null )
    {
      for( int i = 0; i < rooms.Count; i++ )
      {
        if( rooms[i].GetNode() != null && rooms[i].GetNode().branch == Branch.RIGHT )
        {
          if( exit == null )
          {
            exit = rooms[i];
          }
          else if( Vector3.Distance( entrance.position.ToVector3(), rooms[i].position.ToVector3() ) >
            Vector3.Distance( entrance.position.ToVector3(), exit.position.ToVector3() ) )
          {// Nos quedamos con la habitacion mas alejada de la entrada
            exit = rooms[i];
          }
        }
      }
      exit.type = RoomType.EXIT;
    }
    else
    {
      Debug.LogWarning( "Couldn't stablish entrance/exit" );
    }
  }

  // Limpia los objetos creados
  public void Cleanup()
  {
    if( routine != null )
    {
      StopCoroutine( routine );
    }
    bspTree = null;
    // Eliminamos los cubos base "GenSection"
    GameObject[] objectList = GameObject.FindGameObjectsWithTag( "GenSection" );
    for( int i = 0; i < objectList.Length; i++ )
    {
      Destroy( objectList[i].gameObject );
    }

    objectList = GameObject.FindGameObjectsWithTag( "Digger" );
    for( int i = 0; i < objectList.Length; i++ )
    {
      Destroy( objectList[i].gameObject );
    }
    objectList = GameObject.FindGameObjectsWithTag( "BaseRoom" );
    for( int i = 0; i < objectList.Length; i++ )
    {
      Destroy( objectList[i].gameObject );
    }
    GameObject floor = GameObject.Find( "Floor" );
    if( floor != null )
    {
      Destroy( floor.gameObject );
    }
  }

  //split the tree
  public void Split( t_BSPNode pNode )
  {
    if( pNode.GetLeftNode() != null )
    {
      Split( pNode.GetLeftNode() );
    }
    else
    {
      pNode.Cut();
      return;
    }

    if( pNode.GetLeftNode() != null )
    {
      Split( pNode.GetRightNode() );
    }
  }

  private Vector2i GetRandomCoordinateInRoom( Room room )
  {
    bool found = false;
    int tries = 0;
    int rx = room.position.x + 1;
    int rz = room.position.z + 1;
    int depth = 2;
    while( !found && tries < 20 )
    {
      // No queremos posiciones en pared o adyacentes a pared
      rx = Random.Range( room.position.x + depth, room.position.x + room.size.x - depth );
      rz = Random.Range( room.position.z + depth, room.position.z + room.size.z - depth );

      if( levelGrid.GetTile( rx, rz ) == (int)TileType.FLOOR &&
        objectsGrid.GetTile( rx, rz ) == (int)TileType.EMPTY )
      {
        found = true;
      }
      tries++;
    }
    Vector2i position = new Vector2i( rx, rz );
    if( !found )
    {
      position = new Vector2i( -1, -1 );
    }
    return position;
  }

  // Situa al jugador en la entrada de la mazmorra
  private void PlacePlayer( Room room )
  {
    Vector3 position = room.transform.position;
    position.y += 0.1f;
    GameObject.FindGameObjectWithTag( "Player" ).transform.position = position;
  }

  // Coloca el objeto de salida de la mazmorra
  private void PlaceExit( Room room )
  {
    int roomId = room.GetId();
    Vector2i coord = Vector2i.ToVector2i( room.transform.position ); // Centro habitacion
    Vector2i searchLimits = new Vector2i( room.size.x / 2, room.size.z / 2 );

    // Posicion en pared +z
    if( levelGrid.GetTile( coord.x, coord.z + searchLimits.z ) == roomId )
    {
      objectsGrid.SetTile( coord.x, coord.z + searchLimits.z, (int)TileType.EXIT );
    }// Posicion en pared -z
    else if( levelGrid.GetTile( coord.x, coord.z - searchLimits.z ) == roomId )
    {
      objectsGrid.SetTile( coord.x, coord.z - searchLimits.z, (int)TileType.EXIT );
    }// Posicion en pared +x
    else if( levelGrid.GetTile( coord.x + searchLimits.x, coord.z ) == roomId )
    {
      objectsGrid.SetTile( coord.x + searchLimits.x, coord.z, (int)TileType.EXIT );
    }// Posicion en pared -x
    else if( levelGrid.GetTile( coord.x - searchLimits.x, coord.z ) == roomId )
    {
      objectsGrid.SetTile( coord.x - searchLimits.x, coord.z, (int)TileType.EXIT );
    }
    else
    {
      objectsGrid.SetTile( coord.x, coord.z, (int)TileType.EXIT );
    }
  }

  // Situa ememigos dentro de la habitacion
  private void PlaceItems( Room room )
  {
    Vector3 position = room.transform.position;
    // Actualiza el grid de objetos
    Vector2i coord = GetRandomCoordinateInRoom( room );
    if( coord.x == -1 )
    {
      return;
    }

    int r = Random.Range( 0, 100 );
    // Crea un item segun su probabilidad
    if( r >= 0 && r < 60 ) // Coin = 50%
    {
      int coins = Random.Range( 1, 6 );
      // Pueden aparecer mas de una moneda
      for( int i = 0; i < coins; i++ )
      { // Probabilidad igual
        objectsGrid.SetTile( coord.x, coord.z, (int)TileType.COIN );
        coord = GetRandomCoordinateInRoom( room );
      }
    }
    r = Random.Range( 0, 100 );
    if( r >= 60 && r < 80 ) // Potion
    {
      objectsGrid.SetTile( coord.x, coord.z, (int)TileType.POTION_HEALTH );
      numPociones++;
    }
    else if( r >= 80 && r < 100 ) // Chest
    {
      objectsGrid.SetTile( coord.x, coord.z, (int)TileType.CHEST );
    }
  }

  // Coloca trampas en la habitacion
  private void PlaceTraps( Room room )
  {
    Vector2i start = room.position;
    Vector2i end = room.position + room.size;
    // Rodear los cofres con pinchos
    // Aunque la geometria de la habitacion haya sido modificada seguimos contando con el mismo
    // area para recorrer su parte del grid
    for( int i = start.x; i < end.x; i++ )
    {
      for( int j = start.z; j < end.z; j++ )
      {
        switch( objectsGrid.GetTile( i, j ) )
        {
          case (int)TileType.CHEST:
            // Recorremos las celdas adyacentes colocando trampas
            for( int x = i - 1; x <= i + 1; x++ )
            {
              for( int z = j - 1; z <= j + 1; z++ )
              {
                if( !(x == i && z == j) )
                { // No a si mismo
                  if( levelGrid.GetTile( x, z ) == (int)TileType.FLOOR )
                  { // El espacio del escenario es suelo
                    objectsGrid.SetTile( x, z, (int)TileType.TRAP_SPIKES_FLOOR );
                  }
                }
              }
            }
            break;
          case (int)TileType.COIN:
            // Si la moneda esta cerca de una pared, colocamos una trampa en ese tile de pared
            Vector2i xLimits = new Vector2i( i - 2, i + 2 );
            Vector2i zLimits = new Vector2i( j - 2, j + 2 );
            for( int x = xLimits.x; x <= xLimits.z; x++ )
            {
              for( int z = zLimits.x; z <= zLimits.z; z++ )
              {
                if( !(x == xLimits.x && z == zLimits.x) && !(x == xLimits.x && z == zLimits.z) &&
                  !(x == xLimits.z && z == zLimits.x) && !(x == xLimits.z && z == zLimits.z) )
                {
                  if( levelGrid.GetTile( x, z ) >= (int)TileType.ROOM_ID ) // Pared
                  {
                    objectsGrid.SetTile( x, z, (int)TileType.TRAP_SPIKES_WALL );
                  }
                }
              }
            }
            break;
        }
      }
    }
  }

  private void PopulatePassages()
  {
    // Segunda pasada para rellenar pasillos
    List<int> visitedPassages = new List<int>();
    int cellCount = 0;
    for( int i = 0; i < DUNGEON_WIDTH; i++ )
    {
      for( int j = 0; j < DUNGEON_HEIGHT; j++ )
      {
        int tile = levelGrid.GetTile( i, j );
        int currentTile = tile;
        // Encuentra tile de pasillo no visitado
        if( tile >= numRooms && visitedPassages.FindIndex( item => item == tile ) == -1 )
        {
          // Recorrido horizontal
          for( int x = j; tile == currentTile && x < DUNGEON_HEIGHT; x++ )
          {
            cellCount++;
            tile = levelGrid.GetTile( i, x );
          }

          visitedPassages.Add( currentTile );
        }
        if( cellCount >= 10 ) // Se puede poner una roca rodante
        {
          objectsGrid.SetTile( i + 2, j, (int)TileType.TRAP_ROCK );
        }
        cellCount = 0;
      }
    }
  }

  // Coloca enemigos en la habitacion
  private void PlaceEnemies( Room room )
  {
    Vector3 position = room.transform.position;
    Vector2i coord = GetRandomCoordinateInRoom( room );
    if( coord.x == -1 )
    {
      return;
    }
    int r = Random.Range( 0, 2 );

    switch( r )
    {
      case 0: // Crab
        objectsGrid.SetTile( coord.x, coord.z, (int)TileType.ENEMY_CRAB );
        break;
      case 1: // Goblin
        objectsGrid.SetTile( coord.x, coord.z, (int)TileType.ENEMY_GOBLIN );
        break;
    }
    numEnemigo++;
  }

  private void PopulateDungeon( BSPNode node )
  {
    // Si hay nodo izquierdo nos movemos un nivel
    if( node.GetLeftNode() != null )
    {
      PopulateDungeon( node.GetLeftNode() );
    }
    else
    {// Hoja con habitacion
      Room room = node.GetRoom().GetComponent<Room>();

      switch( room.type )
      {
        case RoomType.DEFAULT:
          PlaceItems( room );
          PlaceTraps( room );
          PlaceEnemies( room );
          break;
        case RoomType.PASSAGE:
          break;
        case RoomType.ENTRANCE:
          PlacePlayer( room );
          break;
        case RoomType.EXIT:
          PlaceExit( room );
          break;
        default:
          break;
      }
      return;
    }
    if( node.GetRightNode() != null )
    {
      PopulateDungeon( node.GetRightNode() );
    }
  }

  //split the tree
  public void Split( BSPNode node )
  {
    // Si hay nodo izquierdo nos movemos un nivel
    if( node.GetLeftNode() != null )
    {
      Split( node.GetLeftNode() );
    }
    else
    {
      // Cuando no hay nodos hijos, intenta crearlos
      node.Cut();
      return;
    }
    if( node.GetRightNode() != null )
    {
      Split( node.GetRightNode() );
    }
  }

  public Grid GetGrid()
  {
    return levelGrid;
  }

  public void SetTile( int x, int y, int value )
  {
    levelGrid.SetTile( x, y, value );
  }

  // Agrega una habitacion dentro de los limites del espacio del nodo
  private Room AddRoom( BSPNode node )
  {
    // Las dimensiones minimas de la habitacion seran la mitad de NODE_MIN_SIZE
    // Las dimensiones maximas seran NODE_MIN_SIZE - ROOM_MARGIN
    Vector3 roomPosition = new Vector3( node.position.x, 0f, node.position.y );
    GameObject aRoom = (GameObject)Instantiate( BaseRoom, roomPosition, Quaternion.identity );
    aRoom.transform.localScale = new Vector3( (int)(Random.Range( NODE_MIN_SIZE / 2, node.size.x - ROOM_MARGIN )),
                                             aRoom.transform.localScale.y,
                                             (int)(Random.Range( NODE_MIN_SIZE / 2, node.size.y - ROOM_MARGIN )) );
    // Configuracion de la habitacion
    Room roomScript = aRoom.GetComponent<Room>();
    // Id unico
    roomScript.SetID( (int)TileType.ROOM_ID + nextId );
    nextId++;
    numRooms = (int)TileType.ROOM_ID + nextId;
    roomScript.type = RoomType.DEFAULT;
    aRoom.name = node.GetBranchName() + "-" + node.level.ToString();
    // Escribe en el grid la estructura de la habitacion
    levelGrid = roomScript.Setup( levelGrid );
    roomScript.SetParentNode( node );
    node.SetRoom( aRoom );

    return roomScript;
  }

  public void DrawNode( BSPNode node )
  {
    GameObject cube1 = GameObject.CreatePrimitive( PrimitiveType.Cube );
    cube1.transform.localScale = new Vector3( node.size.x, 0.1f, node.size.y );
    cube1.transform.position = new Vector3( node.position.x, 0f, node.position.y );
    cube1.GetComponent<Renderer>().material.color = new Color( Random.Range( 0.0f, 1.0f ),
                                                              Random.Range( 0.0f, 1.0f ),
                                                              Random.Range( 0.0f, 1.0f ) );
    cube1.GetComponent<Renderer>().material.shader = Shader.Find( "Unlit/Color" );
  }

  // Dibuja en la escena las ultimas subdivisiones en la creacion de la mazmorra
  private void DrawLeafs( BSPNode node )
  {
    if( node.GetLeftNode() != null )
    {
      DrawLeafs( node.GetLeftNode() );
    }
    else
    {
      DrawNode( node );
      return;
    }

    if( node.GetRightNode() != null )
    {
      DrawLeafs( node.GetRightNode() );
    }
  }

  private void CreateRooms( BSPNode node )
  {
    if( node.GetLeftNode() != null )
    {
      CreateRooms( node.GetLeftNode() );
    }
    else
    {
      rooms.Add( AddRoom( node ) );
      return;
    }

    if( node.GetRightNode() != null )
    {
      CreateRooms( node.GetRightNode() );
    }
  }

  // Crea los pasillos para conectar las habitaciones
  // Recorre recursivamente el arbol
  private void ConnectRooms( BSPNode node )
  {
    // Recorrido por los nodos de la parte izquierda
    if( node.GetLeftNode() != null )
    {
      ConnectRooms( node.GetLeftNode() );

      if( node.GetRoom() != null )
      {
        node.GetRoom().GetComponent<Room>().Connect();
        return;
      }
    }
    else
    {
      if( node.GetRoom() != null )
      {
        node.GetRoom().GetComponent<Room>().Connect();
        return;
      }
    }
    // Recorrido por los nodos de la parte derecha
    if( node.GetRightNode() != null )
    {
      ConnectRooms( node.GetRightNode() );

      if( node.GetRoom() != null )
      {
        node.GetRoom().GetComponent<Room>().Connect();
        return;
      }
    }
    else
    {
      if( node.GetRoom() != null )
      {
        node.GetRoom().GetComponent<Room>().Connect();
        return;
      }
    }
  }

  private Vector2i GetAdyacentWallDirection( int x, int z, int depth )
  {
    Vector2i direction = new Vector2i( 0, 0 );
    if( levelGrid.GetTile( x - depth, z ) != 0 )
    {
      direction.x = -1;
    }
    else if( levelGrid.GetTile( x + depth, z ) != 0 )
    {
      direction.x = 1;
    }
    else if( levelGrid.GetTile( x, z - depth ) != 0 )
    {
      direction.z = -1;
    }
    else if( levelGrid.GetTile( x, z + depth ) != 0 )
    {
      direction.z = 1;
    }
    return direction;
  }

  private GameObject CreateWall( List<Vector2i> coordinates, char axis )
  {
    if( coordinates.Count <= 0 )
    {
      return null;
    }
    GameObject wall = null;
    float halfUnit = TILE_UNIT / 2;
    float gap = 0.02f;
    Vector3 position = new Vector3( coordinates[0].x + halfUnit, 0f, coordinates[0].z + halfUnit ); // En el centro de la celda
    if( coordinates.Count == 1 )// Celdas solitarias
    {
      if( axis == 'x' )
      {
        wall = objectManager.Create( ObjectName.TileWall, new Vector3( position.x, 1f, position.z ) );
        Vector3 wallScale = wall.transform.localScale;
        wallScale.x += gap;
        wallScale.z += gap;
        wall.transform.localScale = wallScale;
        wall.GetComponent<TextureTiling>().ReTiling();
      }
    }
    else if( coordinates.Count > 1 )
    { // Agrupar tiles en un pared
      Vector3 midPoint = coordinates[coordinates.Count / 2].ToVector3();
      int wallWidth = coordinates.Count;
      wall = objectManager.Create( ObjectName.TileWall, new Vector3( midPoint.x, 1f, midPoint.z ) );
      // Al escalar tenemo en cuenta que vamos a hacer tiling de la textura
      // por lo que en vez de escalar en z (columnas, c) que es por donde estamos uniendo tiles
      // vamos a escalar en x y luego rotar, así el retiling de la textura se aplica sobre la coordenada correcta
      Vector3 wallScale = wall.transform.localScale;
      wallScale.x = wallWidth + gap;
      wallScale.z += gap;
      wall.transform.localScale = wallScale;
      wall.GetComponent<TextureTiling>().ReTiling(); // Establece el tiling de las texturas segun la escala
      // Ajustes para centrar la pared dentro de las celdas
      if( axis == 'x' )
      {
        wall.transform.Translate( 0f, 0f, halfUnit );
        if( wallWidth % 2 != 0 )
        {
          wall.transform.Translate( halfUnit, 0f, 0f );
        }
      }
      if( axis == 'z' )
      {
        wall.transform.Translate( halfUnit, 0f, 0f );
        if( wallWidth % 2 != 0 )
        {
          wall.transform.Translate( 0f, 0f, halfUnit );
        }
        wall.transform.Rotate( Vector3.up, 90f );
      }
    }

    return wall;
  }

  private void DrawLevel()
  {
    GameObject tempObject;
    // Instanciamos el prefab del suelo y lo escalamos
    Vector3 position = new Vector3( transform.position.x + DUNGEON_WIDTH / 2, 0f, transform.position.z + DUNGEON_HEIGHT / 2 );
    GameObject floor = objectManager.Create( ObjectName.TileFloor, position );
    floor.transform.localScale = new Vector3( DUNGEON_WIDTH, DUNGEON_HEIGHT, 1 );
    // Establecemos el tiling del material con respecto a sus dimensiones, un tile por unidad
    floor.GetComponent<Renderer>().material.mainTextureScale = new Vector2( DUNGEON_WIDTH, DUNGEON_HEIGHT );

    // PAREDES
    List<Vector2i> wallCoordinates = new List<Vector2i>();

    Grid tempGrid = new Grid( DUNGEON_WIDTH, DUNGEON_HEIGHT );
    tempGrid.SetGrid( levelGrid.GetGrid() );

    // Crear el escenario, uniendo paredes
    // Paredes Eje z
    for( int r = 0; r < DUNGEON_WIDTH; r++ )
    { //x
      for( int c = 0; c < DUNGEON_HEIGHT; c++ )
      { //z
        if( tempGrid.GetTile( r, c ) >= (int)TileType.ROOM_ID )
        {
          wallCoordinates.Add( new Vector2i( r, c ) );
        }
        else
        {
          tempObject = CreateWall( wallCoordinates, 'z' );
          if( tempObject != null )
          {
            tempObject.transform.parent = floor.transform.parent;
          }
          if( wallCoordinates.Count > 1 )
          {
            for( int i = 0; i < wallCoordinates.Count; i++ ) // Borra la pared del mapa temporal
            {
              tempGrid.SetTile( wallCoordinates[i].x, wallCoordinates[i].z, 0 );
            }
          }
          wallCoordinates = new List<Vector2i>();
        }
      }
    }

    // Paredes Eje x y celdas solitarias restantes
    for( int c = 0; c < DUNGEON_HEIGHT; c++ )
    { //z
      for( int r = 0; r < DUNGEON_WIDTH; r++ )
      { //x
        if( tempGrid.GetTile( r, c ) >= (int)TileType.ROOM_ID )
        {
          wallCoordinates.Add( new Vector2i( r, c ) );
        }
        else
        {
          tempObject = CreateWall( wallCoordinates, 'x' );
          if( tempObject != null )
          {
            tempObject.transform.parent = floor.transform.parent;
          }
          for( int i = 0; i < wallCoordinates.Count; i++ ) // Borra la pared del mapa temporal
          {
            tempGrid.SetTile( wallCoordinates[i].x, wallCoordinates[i].z, 0 );
          }
          wallCoordinates = new List<Vector2i>();
        }
      }
    }

    // OBJETOS
    for( int r = 0; r < DUNGEON_WIDTH; r++ )
    { //x
      for( int c = 0; c < DUNGEON_HEIGHT; c++ )
      { //z
        position = new Vector3( r + TILE_UNIT / 2, 0f, c + TILE_UNIT / 2 ); // En el centro de la celda
        int cell = objectsGrid.GetTile( r, c );
        // Objetos
        switch( cell )
        {
          case (int)TileType.COIN:
            objectManager.Create( ObjectName.Coin, position );
            break;
          case (int)TileType.CHEST:
            // Los cofres cerca de una pared se giran sobre esta
            GameObject chest = objectManager.Create( ObjectName.Chest, position );
            Vector3 direction = GetAdyacentWallDirection( r, c, 1 ).ToVector3();
            direction.x *= -1f;
            chest.transform.LookAt( position + direction );
            break;
          case (int)TileType.POTION_HEALTH:
            objectManager.Create( ObjectName.PotionHealth, position );
            break;
          case (int)TileType.TRAP_SPIKES_WALL:
          case (int)TileType.TRAP_ARROWS:
            position = new Vector3( r, TILE_UNIT, c );
            Direction orientation = CalculateTrapDirection( new Vector2i( r, c ) );
            if( orientation != Direction.NONE )
            {
              Vector3 rotation = new Vector3( 90f, 0f, 0f );
              if( orientation == Direction.NORTH )
              {
                position.x += TILE_UNIT / 2;
                position.z += TILE_UNIT;
                rotation = new Vector3( 0f, 0f, 0f );
              }
              else if( orientation == Direction.EAST )
              {
                position.x += TILE_UNIT;
                position.z += TILE_UNIT / 2;
                rotation = new Vector3( 0f, 90f, 0f );
              }
              else if( orientation == Direction.SOUTH )
              {
                position.x += TILE_UNIT / 2;
                rotation = new Vector3( 0f, 180f, 0f );
              }
              else if( orientation == Direction.WEST )
              {
                position.z += TILE_UNIT / 2;
                rotation = new Vector3( 0f, 270f, 0f );
              }
            
              ObjectName objectName = (cell == (int)TileType.TRAP_SPIKES_WALL) ? ObjectName.TrapSpikeWall : ObjectName.TrapArrows;

              GameObject trap = objectManager.Create( objectName, position );
              trap.transform.eulerAngles = rotation;
            }
            else
            {
              // Restauramos el tile para que no aparezca en las metricas
              objectsGrid.SetTile( r, c, (int)TileType.EMPTY );
            }

            break;
          case (int)TileType.TRAP_SPIKES_FLOOR:
            objectManager.Create( ObjectName.TrapSpikeFloor, position );
            break;
          case (int)TileType.TRAP_ROCK:
            objectManager.Create( ObjectName.TrapRock, position );
            break;
          case (int)TileType.ENEMY_CRAB:
            position.y += TILE_UNIT;
            tempObject = objectManager.Create( ObjectName.EnemyCrab, position );
            tempObject.GetComponent<Enemy>().SetPatrolCenter( position );
            break;
          case (int)TileType.ENEMY_GOBLIN:
            position.y += TILE_UNIT;
            tempObject = objectManager.Create( ObjectName.EnemyGobling, position );
            tempObject.GetComponent<Enemy>().SetPatrolCenter( position );
            break;
          case (int)TileType.EXIT:
            objectManager.Create( ObjectName.Portal, position );
            break;
        }
      }
    }
  }

  // Mira los alrededores de la posicion de la trampa para orientarla hacia el lado correcto
  private Direction CalculateTrapDirection( Vector2i pos )
  {
    List<Direction> candidates = new List<Direction>();
    Direction orientation = Direction.NONE;

    //// Arriba, izquierda, derecha, abajo (casillas impares)
    int c = 0;
    for( int i = pos.x - 1; i <= pos.x + 1; i++ )
    {
      for( int j = pos.z - 1; j <= pos.z + 1; j++ )
      {
        if( c % 2 != 0 && levelGrid.GetTile( i, j ) == (int)TileType.FLOOR )
        {
          if( c == 1 )
          {
            candidates.Add( Direction.WEST );
          }
          else if( c == 3 )
          {
            candidates.Add( Direction.SOUTH );
          }
          else if( c == 5 )
          {
            candidates.Add( Direction.NORTH );
          }
          else if( c == 7 )
          {
            candidates.Add( Direction.EAST );
          }
        }
        c++;
      }
    }

    if( candidates.Count == 1 )
    {
      orientation = candidates[0];
    }
    else if( candidates.Count > 1 ) // Escoge una direccion aleatoriamente
    {
      orientation = candidates[Random.Range( 0, candidates.Count )];
    }

    return orientation;
  }

  // Automata celular para limpiar
  private void RemoveSingles( int x, int y )
  {
    int count = 0;

    if( x < levelGrid.GetWidth() - 1 && x > 1 && y > 1 && y < levelGrid.GetHeight() - 1 )
    {
      if( levelGrid.GetTile( x + 1, y ) == (int)TileType.FLOOR )
        count++;
      if( levelGrid.GetTile( x - 1, y ) == (int)TileType.EMPTY )
        return;
      if( levelGrid.GetTile( x + 1, y ) == (int)TileType.EMPTY )
        return;
      if( levelGrid.GetTile( x, y + 1 ) == (int)TileType.EMPTY )
        return;
      if( levelGrid.GetTile( x, y - 1 ) == (int)TileType.EMPTY )
        return;

      if( levelGrid.GetTile( x - 1, y ) == (int)TileType.FLOOR )
        count++;
      if( levelGrid.GetTile( x, y + 1 ) == (int)TileType.FLOOR )
        count++;
      if( levelGrid.GetTile( x, y - 1 ) == (int)TileType.FLOOR )
        count++;
      if( levelGrid.GetTile( x - 1, y ) == (int)TileType.FLOOR )
        count++;
      if( levelGrid.GetTile( x - 1, y - 1 ) == (int)TileType.FLOOR )
        count++;
      if( levelGrid.GetTile( x + 1, y - 1 ) == (int)TileType.FLOOR )
        count++;
      if( levelGrid.GetTile( x - 1, y + 1 ) == (int)TileType.FLOOR )
        count++;
      if( levelGrid.GetTile( x + 1, y + 1 ) == (int)TileType.FLOOR )
        count++;

      if( count >= 5 )
        levelGrid.SetTile( x, y, (int)TileType.FLOOR );
    }
  }

  // Vuelca a fichero el resultado de la generación
  public void SaveToFile()
  {
    string fileName = System.DateTime.Now.ToString( "yyyyMMdd_HHmmss" );
    fileName = "bsp_" + fileName;
    string content = "Dungeon Generator (BSP)";
    //content += "\nSeed: " + seed;
    content += "\nDimensions: " + DUNGEON_WIDTH + "x" + DUNGEON_HEIGHT;
    content += "\nRoom min size: " + NODE_MIN_SIZE / 2;
    content += "\n2 caracteres = 1 celda";
    content += "\n\n";

    string temp;
    int tile;
    // Layout de las habitaciones y pasillos
    for( int x = 0; x < DUNGEON_WIDTH; x++ ) // alto = filas
    {
      for( int z = 0; z < DUNGEON_HEIGHT; z++ ) // ancho = columnas
      {
        if( levelGrid.GetTile( x, z ) == 0 )
        {
          content += "--";
        }
        else
        {
          tile = levelGrid.GetTile( x, z );
          if( tile < 10 )
          {
            temp = "0" + tile.ToString();
          }
          else
          {
            temp = tile.ToString();
          }
          content += temp;
        }
      }
      content += "\n";
    }
    content += "\n\n";

    // Habitaciones + objetos/trampas
    for( int x = 0; x < DUNGEON_WIDTH; x++ ) // alto = filas
    {
      for( int z = 0; z < DUNGEON_HEIGHT; z++ ) // ancho = columnas
      {
        if( objectsGrid.GetTile( x, z ) == 0 )
        {
          if( levelGrid.GetTile( x, z ) == 0 )
          {
            content += "--";
          }
          else
          {
            tile = levelGrid.GetTile( x, z );
            if( tile < 10 )
            {
              temp = "0" + tile.ToString();
            }
            else
            {
              temp = tile.ToString();
            }
            content += temp;
          }
        }
        else
        {
          tile = objectsGrid.GetTile( x, z );
          if( tile < 10 )
          {
            temp = "0" + tile.ToString();
          }
          else
          {
            temp = tile.ToString();
          }
          content += temp;
        }
      }
      content += "\n";
    }

    // Escribe a fichero
    fileName = DebugTools.instance.METRICS_PATH + "BSP/" + fileName + @".txt";
    DebugTools.instance.WriteToFile( fileName, content );
  }
}
