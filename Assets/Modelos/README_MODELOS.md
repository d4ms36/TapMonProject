# Biblioteca de modelos y arte

## Propósito

`Assets/Modelos/` centraliza los sprites, animaciones, iconos, fondos y prefabs visuales de TapMon. La carpeta mantiene separado el contenido artístico de la lógica del juego y permite actualizar la apariencia sin cambiar el código.

## Estructura

- `Mascota/Sprites/`: sprites de las etapas de la mascota.
- `Mascota/Animaciones/`: clips y controladores de animación de la mascota.
- `Mascota/Mascota.prefab`: placeholder reutilizable con `Image` y animación `Idle`.
- `UI/Botones/`: sprites y recursos visuales de botones.
- `UI/Iconos/`: iconos de mejoras, misiones y acciones.
- `UI/Fondos/`: fondos de pantallas y paneles.

## Convenciones de nombres

Usar nombres descriptivos en PascalCase, sin espacios ni caracteres especiales. Las etapas de la mascota siguen esta convención:

- `Huevo.png`
- `Bebe.png`
- `Adolescente.png`
- `Adulto.png`
- `Legendario.png`

Para variantes, añadir un sufijo claro, por ejemplo `Adulto_Feliz.png` o `Boton_Comprar.png`.

## Resolución y formato

- Resolución recomendada: `512x512` para sprites de mascota e iconos principales.
- Formato: `PNG` con transparencia RGBA.
- Mantener el fondo transparente y evitar bordes semitransparentes innecesarios.
- Preparar sprites cuadrados cuando se usen en el botón central para conservar la proporción.

## Actualizar un sprite existente

1. Reemplazar el archivo manteniendo exactamente el mismo nombre y ruta.
2. En Unity, seleccionar el asset y usar `Assets > Reimport`.
3. Comprobar que `Texture Type` sea `Sprite (2D and UI)` y que la transparencia se conserve.
4. Verificar el resultado en `Mascota.prefab` y en la escena principal.

## Añadir una nueva etapa

1. Crear el sprite en `Mascota/Sprites/` con resolución y formato recomendados.
2. Añadirlo al array de etapas en `GameManager` o `MascotaManager`, según el sistema que controle la mascota.
3. Actualizar el `UpgradeSO` correspondiente para que su campo `icono` o sprite apunte al nuevo recurso.
4. Si la etapa tiene movimiento propio, crear su clip en `Mascota/Animaciones/` y añadirlo al `Animator Controller`.
5. Probar la transición y el orden de desbloqueo sin renombrar assets ya publicados.

## Animación base

`Mascota.prefab` incluye un `Animator` con el estado `Idle`. El clip placeholder aplica una respiración suave mediante una escala vertical periódica. Sustituir el clip o sus curvas cuando exista arte final, manteniendo el estado `Idle` como estado por defecto.
