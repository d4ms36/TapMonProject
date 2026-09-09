# Resumen ejecutivo de TapMon

## Identidad

TapMon: Mi Compi Virtual es un juego clicker para Android. La mecánica base entrega monedas al pulsar el botón principal y guarda el progreso localmente.

## Estado

CP-1 y CP-2 están completados. La escena principal es `Assets/Scenes/MainMenu.unity` y contiene un `GameManager`, un contador `CoinDisplay` basado en TextMeshPro y el botón `TapButton` conectado a `GameManager.AddCoins(1)`.

El siguiente hito es CP-3: generar y probar manualmente un APK de Android sin anuncios. El proyecto se trabaja con Unity 2022.3.67f2 LTS.

## Arquitectura actual

- `GameManager.cs`: monedas, persistencia con PlayerPrefs y eventos de UI.
- `UpgradeSO.cs`: definición de mejoras como ScriptableObjects.
- `AdManager.cs`: integración opcional de AdMob; usa IDs de prueba durante el desarrollo.
- `MainMenu.unity`: cámara, Canvas, contador, botón y EventSystem.

## Alcance pendiente

Después de CP-3 se prevén la integración de AdMob, el sistema de mejoras, las misiones diarias, el pulido, la preparación de una build firmada y la publicación.

## Restricciones operativas

Los agentes deben editar archivos de texto y configuración sin abrir Unity ni generar APKs. Cada cambio relevante debe quedar reflejado en `README.md` y validarse estáticamente cuando sea posible.
