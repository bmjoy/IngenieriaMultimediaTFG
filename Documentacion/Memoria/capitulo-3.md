CAPÍTULO 3. ALGORITMOS
======================

Clasificación
-------------

Clasificación: Secuencial, Ontogenia, Teleológica

Growing Tree. Creando laberintos.
---------------------------------

Vamos a estudiar un poco más a fondo algunos de los algoritmos para la generación procedural de mazmorras, pero tenemos que empezar por lo básico, un laberinto.

Existen decenas de algoritmos para la resolución y creación de mazmorras de manera procedimental, y algunos de los factores importantes que debemos tener en cuenta para escoger uno son el coste temporal, la
variedad de resultados que nos permite obtener, la complejidad del mismo algoritmo y la personalización que podamos realizar sobre este para adaptarlo a nuestro proyecto.

Antes de pasar al algoritmo conocido como Growing Tree debemos repasar rápidamente ciertos conceptos básicos. Por un lado tenemos los **vértices** o **nodos**, que en el caso que nos compete, pueden ser llamados **celdas**. Estos nodos se unen mediante **aristas**, que es básicamente una línea. Una colección de vértices y aristas es lo que llamamos **grafo**. Si desde una nodo podemos alcanzar cualquier otro nodo del grafo siguiente las aristas entonces decimos que es un **grafo conexo**.

![](img/image13.png)

Si eliminamos los ciclos del grafo, obtenemos un grafo acíclico, una cuando es grafo acíclico es conexo, lo que tenemos es llamado un **árbol**.

![](img/image43.png)

Una grafo puede estar compuesto por varios árboles, pero existe uno que comprende todos los nodos de un grafo, tenemos un **árbol de expansión (Spanning Tree)**.

![](img/image42.png)

Y los árboles de expansión son la misma esencia de la estructura de los algoritmos de generación de laberintos.

![](img/image22.png)

### Aldous-Broder

Dos investigadores llamados **D. Aldous y A. Broder**, trabajando independientemente, estaban estudiando los árboles de expansión cuando diseñaron el siguiente algoritmo:

1. Escoger un vértice cualquiera.
2. Escoger otro vértice aleatorio entre los vecinos de este. Si el nodo no ha sido visitado con anterioridad, moverse a este y agregarlo, junto a la arista, al árbol de expansión.
3. Repetir el paso 2 hasta que todos los vértices hayan sido visitados.

Un algoritmo extremadamente simple que selecciona cualquier de todos los posibles árboles de expansión del grafo con la misma probabilidad. También hay que decir que es una técnica muy ineficiente, ya que su naturaleza aleatoria a la hora de escoger el nodo hace que se puedan volver a visitar los mismos vértices una y otra vez.

![](img/image33.gif)

Posteriormente este método fue mejorado por otros como el algoritmo de Wilson, entre otros, pero vamos a pasar directamente al que nos interesa estudiar, que entra dentro de los algoritmos que hacen uso de la técnica de escoger un nodo aleatoriamente en cada paso, también conocido como **Drunken Walk**, aunque en este caso hay matices.

### Growing Tree

Entre los algoritmos de generación de laberintos, el llamado Growing Tree es quizás el más personalizable. La premisa básica es la de escoger un nodo del grafo aleatoriamente y agregarlo a una lista de "celdas activas". En cada paso posterior miramos a uno de los nodos de la lista y agregamos uno de sus vecinos **no visitados**. Si el nodo no tiene más vecinos sin visitar, lo quitamos de la lista y probamos con otro nodo. El proceso termina cuando la lista se queda vacía.

Debemos que tener en cuenta que cada celda o nodo tiene 4 bordes que tocan con otros nodos o con el exterior de la mazmorra, por lo que cada nodo deberá ser visitado 4 veces. Esto no se produce en la misma pasada, sino que al introducirlos en la lista y luego hacer el backtracking nos vamos asegurando que ese nodo tiene aún bordes libres, en caso contrario lo podemos sacar y lo damos como cerrado.

Cuando estamos comprobando un vecino debemos determinar si se ha de crear un pared según el caso. Para los bordes que den al exterior de los límites de la mazmorra simplemente creamos la pared, cuando nos movemos a una nueva celda no visitada antes, entonces simplemente creamos pasillo entre estas, pero cuando nos topamos con otro nodo que ya está en la lista de activos entonces creamos una pared en ese borde y nos movemos a otro de los vecinos. Este mismo proceso nos permite que siempre se pueda alcanzar cualquier celda desde otra.

Un aspecto interesante es como el uso de distintas heurísticas para seleccionar un nodo de la lista de activos cambia el comportamiento de este algoritmo. Por ejemplo, si escogemos el nodo más reciente, el último que se agregó a la lista, obtenemos un comportamiento de pila recursiva. Este comportamiento es el mismo que encontramos en otro algoritmo llamado Recursive Backtracker. Si escogemos un nodo aleatoriamente, entonces tenemos un comportamiento del estilo del algoritmo de Prim.

En la implementación que he realizado sobre Unity podemos ver el comportamiento al usar el nodo más reciente agregado a la lista.

Sustituir GIFs por tiras de imágenes

![](img/image59.gif)

Y también cuando escogemos un nodo aleatorio cada vez, como en el algoritmo de Prim.

![](img/image12.gif)

Modificando este algoritmo incluso podríamos hacer que se generen habitaciones en vez de pasillos. El uso de este método similar al Recursive Backtracker nos permite determinar y marcar cuando una habitación se ha completado, momento en el que se comienzan a desapilar nodos y retoma el camino hacia la creación de otra habitación. Pero la cosa se complica si queremos tener habitaciones y pasillos en es mismo mapa, por ello este algoritmo no es de mucha utilidad más allá de la generación de laberintos.

### Conclusión

El algoritmo de Growing Tree es interesante en el sentido en que se puede personalizar y ampliarlo para generar algo más que laberintos, ensanchando los pasillos o uniendo los adyacentes para generar habitaciones y obtener una estructura más similar a lo que busco para el videojuego que quiero crear, pero como veremos hay otros algoritmos que son más adecuados y rápidos para generar mapas grandes.

Podemos decir finalmente que este algoritmo tiene sus **ventajas**:  
- Esta basado en técnicas muy simples.  
- Es flexible y puede comportarse como otros algoritmos según su implementación.

Pero también hay algunas **desventajas**:  
- No es el algoritmo más rápido de por sí, y habría que utilizar una mezcla con árboles de partición (BSP, Quadtree) para generar laberintos realmente grandes.  
- Generalmente solo se puede utilizar para la creación laberintos, otro tipo de estructuras como mazmorras o cuevas están fuera del alcance.

El autómata celular
-------------------
Más información para rellenar contenido en esta sección:
[*http://jeremykun.com/2012/07/29/the-cellular-automaton-method-for-cave-generation/*](http://jeremykun.com/2012/07/29/the-cellular-automaton-method-for-cave-generation/)

El segundo tipo de algoritmo que vamos a estudiar entra dentro de los algoritmos utilizados para representar sistemas naturales, como cuevas, bosques o manadas de animales.

El concepto de **autómata celular** fue presentado originalmente en los **años 40** por Stanislaw Ulam y John Von Neumann cuando trabajaban en el laboratorio nacional de Los Alamos. Fue estudiado ocasionalmente durante las siguientes dos décadas pero no fue hasta los 70 en que **John Horton Conway** creó el “**Juego de la vida” (**[***Conway’s Game Of Life***](http://en.wikipedia.org/wiki/Conway%27s_Game_of_Life)**)** que despertó el interés en el entorno académico.

### Conway’s Game Of Life  
Este juego consiste en una serie de puntos llamados células que evolucionan con el tiempo basándose en la interacción con sus vecinas. Si una célula tiene **menos de** **dos** vecinas, entonces **muere**, debido a la falta de población, con **dos o tres** vecinas se mantiene viva durante esa generación, si tiene **más de tres** entonces **muere** por sobrepoblación. Si una célula muerta (un espacio vacío) tiene exactamente tres vecinos, entonces esta se convierte en una célula viva, representando un proceso de reproducción.

Antes de comenzar el juego se establecen ciertas condiciones para la situación inicial, pero a partir de ahí el jugador no tiene más interacción con este, el juego evoluciona por sí solo.

![](img/image36.gif)

Durante los **años 80 Stephen Wolfram** se ocupó del estudio del autómata celular sobre una sola dimensión, y en 2002 publicó su libro “**Un nuevo tipo de ciencia**”, donde argumenta que este tipo de métodos pueden ser utilizados en otros campos de la ciencia como la criptografía.

[*http://en.wikipedia.org/wiki/Elementary\_cellular\_automaton*](http://en.wikipedia.org/wiki/Elementary_cellular_automaton)
[*http://es.wikipedia.org/wiki/Un\_nuevo\_tipo\_de\_ciencia*](http://es.wikipedia.org/wiki/Un_nuevo_tipo_de_ciencia)

### Aplicación para generación procedural de cuevas

**Jim Badcock** publicó hace unos años su aplicación de los [*autómatas celulares en la creación procedimental de cuevas*](http://www.jimrandomh.org/misc/caves.html), mostrando su potencial para la representación de sistemas naturales, al menos como base, ya que el proceso requiere un refinamiento posterior.

El **proceso** consiste en lo siguiente: 

1.  Generamos un lienzo inicial utilizando la técnica de **ruido blanco**, es decir, dividimos el mapa en celdas y las recorremos decidiendo aleatoriamente si esta se rellena o se queda vacía. Para evitar situaciones no deseables o extrañas establecemos un porcentaje que indique la probabilidad de que esta se convierta en pared. Por ejemplo, decimos que la probabilidad es del 40%, generamos un número aleatorio entre 0 y 100 y si este es inferior o igual al 40 entonces generamos una pared.  

2.  Recorremos el mapa de nuevo ahora aplicando las reglas del autómata celular:  
    a.  Si la celda es pared y tiene menos de 3 vecinas de tipo pared, entonces se convierte en espacio vacío.  
    b.  Si la celda está vacía y tiene 5 o más vecinas de tipo pared, entonces esta también se convierte en pared.
  
3.  Cuando hemos recorrido todas las celdas el algoritmo termina, aunque es posible realizar más iteraciones, dependiendo de los resultados a obtener.  

Vamos a ver ejemplos generados a partir de la implementación que he realizado sobre Unity:

![](img/image57.png)

Tablero inicial generado aleatoriamente con una probabilidad de paredes del 35%.

![](img/image30.png)

Este es el resultado al terminar el algoritmo con **un solo pase**, podemos ver que se generan zonas o espacios abiertos distinguibles unos de otros. También observamos uno de los problemas del algoritmo, y es que es poco consistente, y muy frecuentemente obtenemos zonas inconexas que deberemos procesar posteriormente para unirlas al sistema principal de habitaciones. A veces estas zonas desconectadas están separadas simplemente por distancias de 1 o 2 celdas, pero como podemos ver en este mismo ejemplo, en la zona superior derecha tenemos una zona relativamente grande con una separación de la zona principal de al menos 3 celdas, por lo que resultaría difícil determinar programáticamente como esa zona debería unirse.

Por otro lado esto puede ser interesante para juego como **Minecraft**, donde podemos encontrar en cuevas, con mucha frecuencia, zonas totalmente aisladas a las que se debe acceder usando el pico o la pala. En ese caso específico este algoritmo parece actuar correctamente.
![](img/image64.png)

Aquí tenemos otro ejemplo, esta vez con **dos pases**. Ahora hemos conseguido que todos los espacios estén conectados de alguna manera. También se han eliminado celdas sueltas de una unidad al realizar este segundo pase, lo cual puede ser beneficioso para eliminar ruido innecesario pero al mismo tiempo perjudicial porque le quita un poco de la aleatoriedad que podemos encontrar en las cuevas naturales. Otro problema de realizar este segundo pase es que perdemos la distinción entre las distintas secciones o habitaciones de la cueva, por lo que nos queda un espacio abierto con algunos rincones más cerrados.

![](img/image60.png)

Aquí otro ejemplo con los mismo parámetros pero con diferentes dimensiones. Conseguimos conectar todos los espacios pero el centro está demasiado despoblado.

![](img/image58.png)

Si subimos la probabilidad de paredes al 40% y al realizar dos pases obtenemos un resultado mejor.

### Conclusión

Haciendo varias pruebas puedo llegar a la conclusión de que los mejores resultados los obtengo con una probabilidad de obtener paredes del 35% al 40% y uno o dos pases según este porcentaje, cuanto más se acerca al 40% mejores resultados obtendremos con dos pases que solo con uno.

Uno de los principales problemas con este algoritmo, como ya hemos visto, es el poco control sobre el resultado final, demasiadas aleatoriedad en la creación de los distintos espacios que componen la cueva. Esto dificulta también la decoración de esta, ya que es más difícil identificar cada parte de la cueva como esquinas, cámaras aisladas, tipo de materiales de roca, etc., por lo que tendríamos que determinar el tipo de material o bloque según la profundidad de la cueva, ya sea en vertical o en horizontal. Un ejemplo de esto es Minecraft, que distribuye los bloques basándose en la profundidad de bloques desde una capa base y también en la distribución horizontal en zonas que dan al exterior:

![](img/image35.png)

En cualquier caso este tipo de estructuras no son las que quiero para juego que voy a desarrollar, ya que busco una mejor distinción entre habitaciones y pasillo, estructuras del estilo de templos, hechos en parte por humanos. Pero no voy a descartar completamente esta técnica, ya que veremos más adelante que es útil para refinar y darle un toque más natural a los resultados del siguiente algoritmo que vamos a estudiar.

Particionado BSP
----------------

Esta técnica utiliza el particionado binario para subdividir un espacio utilizando hiperplanos. Las subdivisiones obtenidas son representadas mediante una estructura de datos de tipo árbol, conocida como **BSP Tree o Árbol BSP**.

### ¿Por qué utilizar un árbol BSP?

Cuando vamos a generar un mapa de mazmorra, hay muchas maneras de hacerlo, simplemente podríamos generar rectángulos de tamaños y en posiciones aleatorias y crear una habitación en cada uno, pero esto puede llevar a muchos problemas, como la superposición de habitaciones, espacios entre estas demasiado arbitrarios y extraños, como obtener algunas habitaciones muy juntas y otras muy separadas. Si entonces queremos pulir el algoritmo y arreglar estos problemas la complejidad de la solución se vuelve grande y es más difícil de depurar nuevos problemas.

La misma estructura de un **árbol binario** nos permite dividir el espacio de manera más o menos regular, manteniendo la consistencia entre el espacio entre habitaciones, los tamaños de estas y permitiéndonos unir mediante pasillos basándonos directamente en la unión de los mismos nodos del árbol.

El método procedería de la siguiente manera:

**1.** Creamos un plano completo sobre el que vamos a generar las habitaciones de la mazmorra. Este espacio completo es la raíz del árbol.

![](img/image14.png)

**2.** Escogemos una orientación aleatoria, horizontal o vertical, sobre la que vamos a partir.

**3.** Escogemos un punto en x (horizontal) o en y (vertical) según la orientación escogida y partimos el espacio en dos sub-mazmorras.

![](img/image40.png)

**4.** Seguimos subdividiendo esas sub-mazmorras generadas pero teniendo cuidado que las divisiones no sean demasiado cerca del borde, ya que debemos ser capaces de poder incluir una habitación en cada una de estas divisiones.

![](img/image18.png)

Aspecto en la segunda iteración.

![](img/image23.png)

Aspecto en la última iteración. Cada uno de estos espacios incluirá una habitación.

**¿Y cuándo nos detenemos?**

Tenemos varias opciones:
- En cada iteración comprobamos si quedan espacios que puedan ser divididos y en las nuevas áreas se puedan crear al menos una habitación, seguimos dividiendo hasta que no queden espacios que cumplan esta condición. El problema con este método es que al final obtenemos áreas con muy dimensiones similares, por lo que las habitaciones también serán prácticamente iguales.
- Otra solución es establecer un número de iteraciones fijo dependiendo del tamaño de la mazmorra, con lo que obtendremos áreas divididas al máximo posible, pero otras que se podrían dividir al menos una vez más, pero se quedan enteras y de esta manera se puede crear un habitación más grande y alargada o una habitación pequeña pero un pasillo largo que la une con otra sección.

**5.** Cuando tenemos las divisiones para incluir una sola habitación en cada una de estas, comenzamos a construir la mazmorra. Creamos una habitación de tamaño aleatorio dentro de cada división, es decir, cada hoja del árbol, teniendo en cuenta los límites del espacio.

**6.** Para construir los pasillos que unen las habitaciones recorremos el árbol conectando cada nodo hoja con su hermana. Los nodos hoja son los nodos que no tiene más hijos, al final de cada rama del árbol.

El resultado final para el ejemplo que hemos estado viendo sería como se ve en la siguiente imagen.

![](img/image10.png)

Como vemos las habitaciones dentro de cada espacio se generan en una posición y con dimensiones aleatorias dentro de sus límites, por lo que podemos encontrar habitaciones pequeñas en espacio relativamente grande. Evidentemente estos parámetros se pueden ajustar si se desea que cada habitación rellene lo más en su espacio.

El utilizar un árbol binario nos asegura que después de realizar la primera partición vamos a obtener al menos dos hojas en dos ramas distintas que podemos utilizar como habitaciones de inicio y de final, estando siempre separadas por la raíz, por lo que nunca estarán conectadas directamente y se deberá recorrer primero la mayor parte del resto de habitaciones, algo que es esencial para el tipo de juego que queremos realizar en este caso.

Un segundo ejemplo generado mediante este método.

![](img/image32.png)

Como alternativa podemos utilizar un **QuadTree** para generar la mazmorra usando este tipo de técnica. En ese caso tendríamos cuatro nodos por cada partición y puede acelerar el proceso de creación, pero la diferencia es realmente mínima en estos casos.

En la siguiente sección sobre desarrollo del juego se explicará en detalle qué algoritmos se han seleccionado, como se han combinado y cómo se pueden crear las habitaciones y unirlas mediante pasillos.

Desarrollo del videojuego
=========================

Vamos a entrar en detalles sobre el desarrollo del videojuego, la selección de algoritmos para generar las mazmorras, la colocación de objetos, enemigos y trampas, el diseño de la IA y el estilo visual, etc.

Generación de mazmorra basada en árbol BSP
------------------------------------------

Para generar la estructura base de la mazmorra he escogido el algoritmo de generación mediante árbol BSP. La implementación de este algoritmo es algo más compleja que otros vistos, pero nos proporciona ciertas ventajas para la siguiente colocación de objetos.

Por un lado nos permite identificar fácilmente que nodos serán habitaciones, pero además podemos utilizar la división inicial de la raíz para establecer una dificultad distinta entre un lado del árbol y otro. Esto también nos permite escoger la entrada y salida de mazmorra, simplemente basándonos en el lado en el que se sitúa el nodo.

Si queremos adentrarnos incluso más, podemos establecer una dificultad según la profundidad del nodo en el árbol, los de niveles más altos tendrán probabilidades mayores de aparición de ciertos enemigos y trampas. Esto agregar una curva de dificultad progresiva según vamos avanzando dentro de la misma mazmorra.

Vamos a definir ciertos valores para acortar la descripción del
algoritmo:

- **ROOM\_SIZE**: tamaño mínimo del espacio que ocupará una habitación.

- **NODE\_SIZE**: tamaño del espacio/nodo.

- **MARGIN**: Margen entre el borde de los espacios de los nodos y la habitación contenida en estos.

### GENERANDO EL ÁRBOL

**1.** Creamos un grid de enteros del tamaño indicado en las dimensiones de la mazmorra. Una casilla de este grid equivale a una casilla del grid de Unity, de una unidad, el tamaño por defecto de un cubo.

Inicializamos toda la rejilla al valor 0.

**2.** Creamos el árbol BSP comenzando con el nodo raíz. Posicionamos el nodo raíz en el centro del mapa.

**3.** Nos movemos al siguiente nodo por la izquierda. Si tienes hijos, vamos al siguiente por la izquierda, llamando al mismo método de forma recursiva pasando este nodo izquierdo. Cuando no se encuentre hijo, partimos (Cut).

**3.1** **Cut**: Decidimos con un 50% de probabilidad, si se divide (Split) por el eje 'x' o el 'z'.

**3.2 SplitX/SplitZ**:

**a.** Comprobamos que se puede hacer un corte, dejando sitio en ambos nodos resultante para introducir una habitación en cada uno. Para ello comprobamos si el tamaño del nodo es al menos ROOM\_SIZE \* 2.

**b.** Realizamos el corte, escogiendo un valor entre [ROOM\_SIZE, NODE\_SIZE.x/z - ROOM\_SIZE]. De esta manera dejamos al menos un mínimo del tamaño de la habitacion. Restamos el valor obtenido al NODE\_SIZE y obtenemos así el tamaño de la partición.

**c.** Del valor anterior creamos un nodo a la izquierda del nodo actual. Obtenemos el espacio restante y creamos el nodo derecho.

**4.** Al volver pasamos al nodo derecho y realizamos los mismos pasos.

**6.** ITERAR 3-5 durante x ciclos. El número de ciclos dependerá del
tamaño de la mazmorra habitaciones.

### CREANDO HABITACIONES

**7.** Recorremos el árbol de forma recursiva y cuando llegamos a un nodo hoja, sin hijos, creamos una habitación en este, AddRoom(nodo).

**7.1.** **AddRoom**(nodo): Obtenemos la posicion de la habitación a partir de la del nodo, será la misma puesto que vamos a centrar la habitación en el espacio particionado.

**7.2.** Instanciamos el objeto base para la habitación en esta posición y le damos unas dimensiones aleatorias entre el tamaño mínimo de la habitación (ROOM\_SIZE) y el tamaño máximo de espacio disponible menos un margen que dejamos para que dos habitaciones adyacentes no aparezcan demasiado juntas.

Size x = Random(ROOM\_SIZE, NODE\_SIZE - MARGIN)

Size z = Random(ROOM\_SIZE, NODE\_SIZE - MARGIN)

**7.3.** Con la posicion y dimensiones de la habitación ahora recorremos el grid estableciendo los tiles de suelo(1) y paredes(2).

### CONECTANDO HABITACIONES

**8.** Recorremos el árbol hasta las hojas, uniendo sus habitaciones con pasillos.

El proceso se realiza bajando hasta un nodo hoja con habitación, entonces se busca su hermano, que contendrá una habitación adyacente.

Entonces creamos un pasillo entre estas dos habitaciones mirando los lados adyacentes y seleccionando un punto de puerta aleatorio en la primera de las habitaciones. Desde este punto "cavamos" en línea recta hasta la otra habitación.

Un vez creado el pasillo subimos al nodo padre, que no tendrá asignada una habitación y le asignamos aleatoriamente una de entre los nodos hijo. De esta manera el nodo hermano de este nodo padre se conectará con una de los nodos hijos y realizando esto por todo el árbol mantenemos la conectividad en toda la mazmorra.

### LIMPIANDO MEDIANTE AUTOMATAS CELULARES

**9.** Un paso adicional es hacer una limpieza de **celdas** "**solitarias**" que puedan haber quedado en la creación de los pasillos. Aplicamos una iteración usando el algoritmo del autómata celular de manera ligera.

Esto elimina columnas sueltas y agrega más variedad en las formas de las habitaciones, ya que crea curvas hacia los pasillos en algunas de las
habitaciones.

![](img/image34.png)

Implementación del videojuego
=============================

Detalles sobre la implementación del videojuego de los aspectos mencionados en el documento/apartado de diseño.

El mapa del mundo
-----------------

Sobre la pantalla de mapa donde se selecciona cada mazmorra.

Generación de las mazmorras
---------------------------

### Estructura principal / Generación de habitaciones

Después de haber estudiado las técnicas
- Generación de objetos. Ítems de objetivo (Llaves, mapas, objetos especiales)
- Generación de trampas.
- Enemigos.


Ítems
-----
- Objetos comunes
- Objetos de mazmorra
**…**

Aspectos técnicos de diseño
---------------------------
*Guía de estilo*
*Patrones de diseño de videojuegos utilizados*

Metodología de trabajo
======================
*Sobre el propio método de gestión del proyecto, cómo se fijan los
objetivos y se llega a estos, gestión de riesgos, etc.*  
Objetivos semanales  
Todoist  
RescueTime  

Conclusiones
============
*Reflexión sobre el desarrollo del trabajo y futura continuación del
proyecto.*

Sobre este documento
====================

Software ofimático
------------------

Para escribir este documento he utilizado la herramienta de oficina online **Google Docs**. Los motivos que me han llevado a utilizar esto en vez de una suite como Microsoft Office:
- Simpleza, con la funcionalidad mínima que necesito y buenos parámetros por defecto, me ayuda a concentrarme.
- Mejor renderizado de fuentes, más parecido a lo que finalmente se exporta como PDF, probablemente por el suavizado que realiza el navegador.
- Rápido acceso a nuevas tipografías desde el servicio de Google.
- El corrector ortográfico de Google es superior en algunos aspectos a otros como el utilizado por Libreoffice.

Tipografía
----------
Para la tipografía he escogido la familia de fuentes **Roboto Slab** de Google. Se trata de la tipografía que podemos ver en los sistemas Android pero en su variante Serif. Un tipo Serif no tan marcado como Times New Roman o Georgia, por lo que es más agradable a la vista.

También he escogido un tamaño de 11 píxeles, similar a lo que se suele ver en sitios online con gran contenido de texto.
