TODO del videojuego
===================
## General
+ Limpiar codigo. Usar correctamente properties o variables publicas. Revisar nombres de funciones y agregar comentarios.
+ Mantener en fichero el sistema de probilidades de aparicion y leer desde ahi.

## Tests
+ Usar el DungeonGenerator para los tests en vez del GeneratorBSP que ya esta antiguo.

## Dungeon Generator
+ Centrar habitaciones dentro del espacio del nodo
+ Colocar la salida lejos de la entrada y de manera que no exista un camino demasiado directo. Utilizar distancias usando directamente el arbol.
+ Agregar items con probabilidad variable. Afecta el nivel, otros items, etc.
+ Clasificar las habitaciones y pasillos en el grid de alguna manera y distinguiendo todo el area que ocupan. Hace un enum con los tipos de tiles de escenario o algo asi.
+ BUG: A veces al buscar una salida no la encuentra y da error porque es null. En la funcion de calcular las distancias.

## Enemigos
+ Colocar enemigos en mazmorras de manera aleatoria basándose en probabilidad y afectado por items y trampas.

## Visual/Animaciones
+ Efectos ítems
  * Arreglar animación de cofre
  * Efecto moneda al cogerla. Hace un brinco y mientras se hace pequeña y gira se introduce en la mochila del jugador.

## Texturas
+ Cofre
+ Moneda
+ Cambiar dispensador de flechas
+ Texturas para trampas de pinchos

## Sonido y música
+ Música de menús
+ Música de juego
+ Sonido de botones
+ Sonido de items
  * Moneda
  * Poción
  * Cofre
+ Trampas
  * Dispensador de flechas
  * Pinchos
  * Trigger y roca

## Extra
+ Cambiar semilla de int a String y usar GetHash... para obtener el int32
+ Cargar nivel cargando con semilla
+ Pantalla de controles y explicación de objetivos al iniciar un nuevo juego
+ Manual del juego
