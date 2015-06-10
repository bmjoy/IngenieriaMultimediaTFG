TODO del videojuego
===================
## Tests
+ Usar el DungeonGenerator para los tests en vez del GeneratorBSP que ya esta antiguo.

## Dungeon Generator
+ Centrar habitaciones dentro del espacio del nodo
+ Colocar la salida lejos de la entrada y de manera que no exista un camino demasiado directo. Utilizar distancias usando directamente el arbol.
+ Agregar items de manera aleatoria con probabilidad
+ Agregar items con probabilidad variable. Afecta el nivel, otros items, etc.

## Enemigos
+ Terminar de implementar las diferentes IA de patrulla, etc.
+ Colocar enemigos en mazmorras de manera aleatoria basándose en probabilidad y afectado por items y trampas.

## Visual/Animaciones
+ Efectos ítems
  * Arreglar animación de cofre
  * Efecto moneda al cogerla. Hace un brinco y mientras se hace pequeña y gira se introduce en la mochila del jugador.
  * 

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
