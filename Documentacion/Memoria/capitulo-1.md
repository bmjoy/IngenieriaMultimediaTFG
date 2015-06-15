CAPÍTULO 1. SOBRE EL PROYECTO
=============================

1.1 Motivación
--------------
Desde el lanzamiento de Minecraft los videojuegos basados en la
generación de niveles o mundos de manera semi-aleatoria ha explotado en
popularidad entre las creaciones de corte más independiente, donde los
recursos son bastante limitados y hay que recurrir a técnicas que puedan
aportar la mayor cantidad de contenido sin que esto robe más tiempo del
proyecto de lo que debería ser necesario. Con esto me refiero a pequeños
estudios donde no se pueden permitir tener a personas dedicadas
únicamente a un aspecto del diseño como pueden ser los escenarios. En
este aspecto cabe destacar a Minecraft debido a que desde su lanzamiento
han proliferado una gran cantidad de clones y variaciones desde todos
los rincones del ambiente académico, pudiendo encontrar en internet
cientos de repositorios con alguna pequeña implementación de generación
de un mundo utilizando algún algoritmo de los que vamos a estudiar y
cualquier lenguaje de programación imaginable.

Ahora incluso estamos presenciando lo que parece ser una nueva
generación en este sector con el prometedor No Man’s Sky, que pretende
recrear un enorme universo lleno de vida y actividades para el jugador.
En este caso ya no estamos hablando solamente de utilizar algoritmos y
parámetros para generar el terreno, estructuras y las estadísticas de
los objetos, sino que incluso las especies animales, plantas y demás
vida que podamos encontrar en este universo.

Mi interés por este tipo de técnicas comenzó hace unos años cuando
ejecuté por primera vez el videojuego Minecraft y, aunque reticente al
principio, debido principalmente a su aspecto simple y un poco amateur,
en cuanto me puse a explorar me quedé fascinado al ver como debajo de
una jugabilidad y aspecto simple se escondían mundos enormes que se
generaban delante de mis ojos, donde se podían encontrar rincones
escondidos con formaciones extrañas y nada habituales dentro de ese
mismo mundo. Disfrutaba solo con el hecho de comenzar una nueva partida,
introduciendo un nombre de semilla a mi gusto y viendo el resultado, o
adentrándome en algún foro o hilo de Reddit para buscar semillas
curiosas que otros usuarios habían descubierto, aportando coordenadas
donde podíamos encontrar formaciones de minerales extraños y poco
frecuentes.

A pesar de mi interés por esos mundos aleatorios, no ha sido hasta ahora
que he encontrado el momento perfecto para estudiar estas técnicas de
generación, ahora que mis conocimientos de programación me dan la
confianza suficiente para hacerlo y tengo la oportunidad perfecta con
este trabajo de fin de grado.

En este trabajo propongo un **estudio de técnicas de generación
procedimental** como las utilizadas en Minecraft, Spelunky o Faster than
light, así como la realización de un videojuego de tipo **dungeon
crawler** o **rogue-like** que genere el contenido, desde la estructura
general de habitaciones hasta los objetos, de esta manera.

1.2 Objetivos
-------------

- **Estudio** de la definición de **generación procedimental** de contenido dentro del sector de los videojuegos como herramienta que contribuye a la jugabilidad de este.

- Estudio de distintos videojuegos que utilizan técnicas de generación procedimental de contenido.

- Estudio de distintos algoritmos de generación de mazmorras y del contenido de estas. Implementación visual de algunos de estos algoritmos que sean más orientados a la generación de mazmorras, pudiendo modificar algunos parámetros desde una interfaz gráfica.

- Diseño y desarrollo de un videojuego sencillo de estilo rogue-like, haciendo uso justificado de alguno de los algoritmos estudiados para la generación del contenido.

1.3 Estructura de los documentos
--------------------------------

En este documento vamos a explorar las distintas definiciones de
generación procedural aplicada a videojuegos, veremos el funcionamiento
de algunos algoritmos para la generación de mazmorras y el desarrollo
del videojuego que acompaña al proyecto.

El documento consta de X[Numero de capítulos] capítulos con el siguiente contenido:

- **Capítulo 1**: Motivación y objetivos del proyecto. Introducción al documento.

- **Capítulo 2**: Definición de generación procedimental. Estudio de técnicas utilizadas en videojuegos.

- **Capítulo 3**: Algoritmos de generación de mazmorras. Clasificación. Descripción detallada y demostración.

- **Capítulo 4**:

<!--Explicar que se incluye en este documento y en el GDD, que no profundiza en la implementación final. Dejarlo bien claro.-->
