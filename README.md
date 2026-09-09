# TapMon: Mi Compi Virtual - Configuración del Proyecto

> Esta sección es la fuente única de verdad del estado actual del proyecto. Los procedimientos históricos que aparecen más abajo se conservan como referencia y deben interpretarse según este estado.

**Repositorio:** https://github.com/d4ms36/TapMonProject

## Estado actual

- **Unity:** 2022.3.67f2 LTS.
- **Escena principal:** `Assets/Scenes/MainMenu.unity`.
- **Scripts principales:** `Assets/Scripts/GameManager.cs` y `Assets/Scripts/UpgradeSO.cs`.
- **UI:** `Canvas` con `CoinDisplay` (TextMeshPro) y `TapButton`.
- **Interacción:** `TapButton` ejecuta `GameManager.AddCoins(1)` y `GameManager.coinDisplay` apunta a `CoinDisplay`.
- **Errores conocidos:** Ninguno; la escena está configurada para compilar sin errores.

## Estado CP-3

CP-3 está completado: el APK de prueba fue generado y probado manualmente en Android.

## Documentación para Google Play

- Política de privacidad: `Docs/PrivacyPolicy.md`.
- Términos y condiciones: `Docs/TermsOfService.md`.
- Material de ficha: `Docs/GooglePlay/description.txt` e `icon.png`.
- Capturas pendientes de incorporar: `Docs/GooglePlay/screenshots/`.

Antes de publicar, sustituir los datos de contacto, confirmar la URL pública de las políticas y revisar la declaración de datos de Google Play.

## Buenas prácticas de AdMob

- Usar únicamente IDs de prueba durante desarrollo y pruebas internas.
- No hacer clic en anuncios propios ni incentivar clics artificiales.
- Mostrar anuncios respetando las políticas de AdMob y Google Play.
- Solicitar consentimiento para anuncios personalizados cuando corresponda, especialmente para usuarios del EEE/Reino Unido, antes de inicializar la publicidad personalizada.
- Mantener los anuncios recompensados como una acción voluntaria y entregar la recompensa solo después de una señal válida del SDK.
- No incluir AdMob en el APK CP-3; activarlo solo después de configurar consentimiento, privacidad y los identificadores de producción.

## Configuración de AdMob

`Assets/Scripts/AdManager.cs` usa la API moderna de Google Mobile Ads 9.x para banner adaptable inferior, interstitial y anuncio recompensado. Los IDs reales de TapMon están separados de los IDs oficiales de prueba mediante `useTestAds`.

La carga de interstitial y rewarded usa `InterstitialAd.Load(...)` y `RewardedAd.Load(...)`; no deben reintroducirse constructores `new InterstitialAd(...)`, `LoadAd()` sobre esos formatos ni `OnAdRewarded`, porque pertenecen a APIs antiguas.

### Instalación y configuración

1. Instalar el plugin oficial Google Mobile Ads Unity en el proyecto.
2. Definir `ADMOB_ENABLED` en `Project Settings > Player > Scripting Define Symbols` después de instalar el SDK.
3. Crear un GameObject `AdManager` y asignarle `Assets/Scripts/AdManager.cs`.
4. Mantener `useTestAds = true` durante desarrollo, pruebas internas y pruebas en dispositivos.
5. Antes de publicar, configurar consentimiento y cambiar `useTestAds` a `false` en el componente `AdManager`.
6. Configurar el App ID de Android en el SDK/manifest; el App ID de TapMon está documentado en el script.

### IDs de TapMon

- App ID: `ca-app-pub-9771091826001795~9872406537`
- Banner: `ca-app-pub-9771091826001795/2154181115`
- Interstitial: `ca-app-pub-9771091826001795/2971956266`
- Rewarded: `ca-app-pub-9771091826001795/1609783455` (500 monedas)

Los IDs reales no deben usarse en pruebas que generen impresiones o clics artificiales. Los IDs `ca-app-pub-3940256099942544/...` del script son los IDs oficiales de prueba de Google.

## MP-2: APK con anuncios de prueba

Objetivo: instalar el SDK de Google Mobile Ads, activar `AdManager` y generar una build Android que use exclusivamente anuncios de prueba.

### Preparación en Unity

1. Abrir el proyecto con Unity 2022.3.67f2 LTS.
2. Instalar el plugin Google Mobile Ads Unity desde Package Manager siguiendo la [guía oficial de Google](https://developers.google.com/admob/unity/quick-start).
3. Resolver la instalación del External Dependency Manager si el plugin lo solicita y dejar que el SDK termine su configuración.
4. En `Project Settings > Player > Scripting Define Symbols`, añadir `ADMOB_ENABLED` para Android.
5. Abrir `Assets/Scenes/MainMenu.unity` y crear un GameObject vacío llamado `AdManager`.
6. Asignar `Assets/Scripts/AdManager.cs` al GameObject `AdManager`.
7. Confirmar que `useTestAds` permanece en `true`. No usar los IDs reales durante esta prueba.
8. Configurar el App ID de prueba de Android en la configuración del plugin; los IDs de unidad de prueba ya están definidos en `AdManager.cs`.

### Generación del APK

1. Abrir `File > Build Settings` y seleccionar `Android`.
2. Pulsar `Switch Platform` si es necesario.
3. Añadir `Assets/Scenes/MainMenu.unity` a `Scenes In Build`.
4. Verificar que `ADMOB_ENABLED` está activo para la plataforma Android.
5. Generar un APK de prueba, por ejemplo `Builds/TapMon_MP2_Test.apk`.

### Prueba en el celular

Instalar el APK en un dispositivo Android con conexión a Internet y comprobar:

- El banner aparece en la parte inferior sin bloquear la UI.
- El interstitial se muestra al completar una misión o desde un botón de prueba; no debe mostrarse de forma abusiva.
- El rewarded se puede abrir voluntariamente y entrega exactamente 500 monedas después de completarse.
- Si un anuncio no está disponible, la aplicación continúa funcionando y permite reintentar.
- La consola no muestra errores de App ID, SDK o carga de anuncios.

El procedimiento detallado y el espacio para evidencias están en `Docs/MP2_CHECKLIST.md`.

## Generación automática de APK

`Assets/Editor/BuildScript.cs` permite generar el APK desde una terminal sin abrir Unity manualmente. Requiere Unity `2022.3.67f2`, Android Build Support, el SDK oficial de Google Mobile Ads y `useTestAds = true`. En este entorno el APK queda pendiente porque esa versión exacta de Unity y el SDK de AdMob aún no están instalados.

Desde la raíz del proyecto, ejecutar PowerShell con la ruta de Unity instalada:

```powershell
& "C:\Program Files\Unity\Hub\Editor\2022.3.67f2\Editor\Unity.exe" `
   -batchmode -quit -nographics `
   -projectPath (Get-Location) `
   -executeMethod BuildScript.BuildAndroidAdMob `
   -logFile (Join-Path (Get-Location) "build.log")
```

El script activa Android, registra `MainMenu.unity`, asegura `ADMOB_ENABLED` y genera `Builds/TapMon_AdMob.apk`. Tras la ejecución:

```powershell
$apk = Get-Item "Builds/TapMon_AdMob.apk"
Write-Host "APK: $($apk.FullName)"
Write-Host "Tamaño: $($apk.Length) bytes"
```

Revisar `build.log` si Unity devuelve un código distinto de cero. No usar `2022.3.62f3` u otra versión para este hito: la build debe ejecutarse con `2022.3.67f2`.

## Checklist de publicación en Google Play

- [x] Repositorio GitHub configurado y tag `v0.1.0-cp3` creado.
- [x] APK CP-3 generado y probado.
- [ ] Política de privacidad publicada en una URL accesible.
- [ ] Términos y condiciones revisados y publicados.
- [ ] Formulario de seguridad de datos completado.
- [ ] Consentimiento GDPR implementado y probado donde corresponda.
- [ ] Icono final de 512x512 preparado.
- [ ] Capturas de pantalla finales incorporadas.
- [ ] Descripción, categoría, clasificación de contenido y datos de contacto completados.
- [ ] Build de producción firmada y subida a Play Console.

## Procedimiento CP-3 archivado

El procedimiento manual usado para generar y probar el APK se conserva debajo como referencia.

1. Abrir el proyecto con Unity 2022.3.67f2 LTS y cambiar la plataforma a Android.
2. Añadir `Assets/Scenes/MainMenu.unity` a Build Settings.
3. Configurar el identificador de paquete y mantener `ADMOB_ENABLED` desactivado.
4. Generar `Builds/TapMon_Test.apk`.
5. Instalarlo en un dispositivo Android y probar el contador, la persistencia y el arranque.

### Checklist CP-3 archivado

- [ ] Android seleccionado como plataforma.
- [ ] `MainMenu.unity` incluida en la compilación.
- [ ] APK generado sin anuncios.
- [ ] APK instalado en un dispositivo.
- [ ] `TapButton` incrementa el contador.
- [ ] Progreso conservado tras cerrar y abrir la aplicación.
- [ ] Capturas y tamaño del APK registrados.

## Historial resumido

- **CP-1:** Proyecto creado con scripts base y estructura inicial.
- **CP-2:** Escena principal creada con Canvas, contador, botón funcional, GameManager y EventSystem.

## Instrucciones para nuevos agentes

1. No abrir Unity; editar únicamente archivos de texto y configuración.
2. Leer esta sección y `AGENTE_GUIA.md` antes de modificar el proyecto.
3. Actualizar `README.md` después de cada cambio relevante.
4. Usar únicamente IDs de prueba de AdMob durante el desarrollo.
5. Comentar el código nuevo en español cuando el comentario aporte contexto.

## Estado de implementación

### Dependencias de Unity

Corregido el error de apertura causado por la dependencia inexistente `com.unity.modules.vestings`. La entrada fue eliminada de `Packages/manifest.json` y el archivo conserva un JSON válido.

### Scripts faltantes en MainMenu

Corregido el aviso `The referenced script (Unknown) on this Behaviour is missing!`. La escena contenía GUID ficticios reutilizados para componentes UI diferentes, por lo que Unity no podía resolverlos como scripts válidos. Se eliminaron esas referencias rotas de `Assets/Scenes/MainMenu.unity`; `GameManager.cs` y `UpgradeSO.cs` existen en `Assets/Scripts/` y sus nombres de clase coinciden con sus archivos.

Al abrir la escena, Unity puede reserializar los componentes del paquete UI/TMP; no deberían aparecer referencias `Unknown`.

Verificación adicional: `Canvas` conserva únicamente su `RectTransform`, `Canvas`, `CanvasScaler` y `GraphicRaycaster`, y no quedan componentes `MonoBehaviour` con GUID nulo o referencias a scripts inexistentes en `MainMenu.unity`.

También se corrigió el GUID corrupto de `CanvasScaler` en `Assets/Scenes/MainMenu.unity`: tenía 40 caracteres y ahora usa el GUID válido de 32 caracteres.

El botón `TapButton` quedó conectado a `GameManager.AddCoins(1)`: el evento usa el objeto `GameManager` (`fileID: 1202`), el modo de argumento entero y el valor `1`; `GameManager.coinDisplay` referencia `CoinDisplay` (`fileID: 2202`).

### Configuración de Unity

El proyecto requiere **Unity 2022.3.67f2 LTS**, definida en `ProjectSettings/ProjectVersion.txt`. Esta versión incorpora los parches de seguridad necesarios para el problema reportado en versiones anteriores y es la versión base recomendada antes de publicar en Google Play.

Para instalarla desde Unity Hub:

1. Abrir **Unity Hub** y entrar en **Installs**.
2. Pulsar **Install Editor** y seleccionar **Unity 2022.3.67f2 LTS**.
3. Durante la instalación, incluir estos módulos:
   - **Android Build Support**.
   - **Android SDK & NDK Tools**.
   - **OpenJDK**.
4. En **Projects**, abrir este proyecto con la instalación `2022.3.67f2`.
5. Aceptar la actualización de archivos sólo después de confirmar que existe una copia de seguridad o control de versiones.

### CP-1: Estructura base

Completado. El proyecto contiene las carpetas base, `GameManager.cs`, `UpgradeSO.cs` y `.gitignore` para archivos generados por Unity.

### CP-2: Escena jugable y UI

La UI de `Assets/Scenes/MainMenu.unity` fue regenerada con referencias oficiales de Unity UI y TextMeshPro: Canvas 1920x1080, fondo, contador, botón central y EventSystem. `GameManager.coinDisplay` apunta a `CoinDisplay` y el botón ejecuta `GameManager.AddCoins(1)`.

Para probarlo en Unity:

1. Abrir la carpeta en Unity Hub con Unity 2022.3.67f2 LTS.
2. Abrir `Assets/Scenes/MainMenu.unity`.
3. Verificar que `GameManager.coinDisplay` apunta a `CoinDisplay`.
4. Ejecutar la escena y pulsar `¡Tócame!`; el contador debe aumentar y aparecer el log de toque.
5. Probar la persistencia cerrando y reabriendo la escena.

La integración de AdMob es opcional y no es necesaria para completar CP-2.

### CP-3: Primer APK instalable sin anuncios

La documentación de CP-3 está preparada. El APK y las capturas deben generarse manualmente desde Unity y un dispositivo Android.

#### 1. Abrir y probar el proyecto

1. Abrir Unity Hub y seleccionar el proyecto ubicado en `C:\Users\Orial\Desktop\Apps\Toctoc`.
2. Usar Unity 2022.3.67f2 LTS con `Android Build Support`, `Android SDK & NDK Tools` y `OpenJDK` instalados.
3. Abrir `Assets/Scenes/MainMenu.unity`.
4. Revisar la consola y resolver cualquier error rojo antes de continuar.
5. Presionar **Play** y pulsar `¡Tócame!`; el contador debe aumentar y la consola debe mostrar el mensaje de toque.
6. Detener **Play**.

#### 2. Configurar Android

1. Abrir `File > Build Settings`.
2. Seleccionar `Android` y pulsar `Switch Platform`; esperar a que finalice.
3. Abrir `Assets/Scenes/MainMenu.unity` y pulsar `Add Open Scenes`.
4. Confirmar que la escena aparece en la lista y está marcada para compilar.

#### 3. Configurar Player Settings

En `File > Build Settings > Player Settings...`, pestaña **Android**:

- **Package Name:** `com.tuempresa.tapmon`; sustituir `tuempresa` por el identificador del equipo.
- **Target API Level:** `Automatic (highest installed)`.
- **Scripting Backend:** `Mono` para builds de prueba más rápidas.
- **Internet Access:** `Auto`.
- **Target Architectures:** `ARM64` si está disponible.

No activar `ADMOB_ENABLED` ni instalar AdMob para este primer APK. La prueba no necesita anuncios ni conexión a Internet.

#### 4. Generar el APK

1. En `Build Settings`, pulsar **Build**.
2. Crear o seleccionar la carpeta `Builds/`.
3. Guardar el archivo como `Builds/TapMon_Test.apk`.
4. Esperar a que Unity finalice la compilación.
5. Registrar el tamaño del archivo generado en la evidencia de CP-3.

#### 5. Instalar en Android

1. Conectar el dispositivo por USB o copiar `TapMon_Test.apk` al teléfono.
2. Activar **Opciones de desarrollador** y **Depuración USB** si se usa USB.
3. Abrir el APK desde el administrador de archivos.
4. Autorizar la instalación desde fuentes desconocidas cuando Android lo solicite.

#### 6. Validar en el dispositivo

1. Abrir la aplicación **TapMon**.
2. Pulsar el botón central y confirmar que el contador aumenta.
3. Cerrar y volver a abrir la aplicación.
4. Confirmar que el progreso se conserva mediante `PlayerPrefs`.
5. Guardar una captura del juego en el editor y otra del juego en el teléfono.
6. Anotar cualquier error de Unity, instalación o ejecución; si no hay errores, registrar `Ninguno`.

#### Evidencias de CP-3

- [ ] Captura del juego en modo Play del editor.
- [ ] Captura del juego ejecutándose en el celular.
- [ ] Tamaño registrado de `TapMon_Test.apk`.
- [x] README actualizado con el procedimiento de CP-3.
- [ ] Lista de errores encontrados o confirmación `Ninguno`.
- [ ] APK instalado y contador funcional en el dispositivo.

## 📋 Requisitos Previos
- Unity Editor 2022.3.67f2 LTS
- Módulos instalados: Android Build Support, Android SDK & NDK Tools, OpenJDK

## 🚀 Configuración del Proyecto

1. **Crear Proyecto:**
   - Abre Unity Hub → Nuevo Proyecto → Plantilla "2D Mobile"
   - Nombre: `TapMonProject`
   - Ubicación: `C:\Users\Orial\Desktop\Apps\Toctoc`

2. **Cambiar Plataforma:**
   - Ve a `File > Build Settings`
   - Selecciona `Android` y haz clic en `Switch Platform`

3. **Configurar Player Settings:**
   - `File > Build Settings > Player Settings`
   - Package Name: `com.tuempresa.tapmon`
   - Target API Level: `Automatic (highest installed)`
   - Other Settings: Verificar Scripting Backend (IL2CPP)

## 📁 Estructura del Proyecto

```
Assets/
├── Scripts/
│   ├── GameManager.cs       - Gestor principal del juego
│   ├── AdManager.cs         - Integración opcional con AdMob
│   └── UpgradeSO.cs         - ScriptableObject para mejoras
├── ScriptableObjects/
│   └── (Crear aquí las instancias UpgradeSO desde el menú de Unity)
├── Scenes/
│   └── MainMenu.unity       - Escena principal
├── UI/
│   └── (Prefabs de UI)
└── Art/
    └── (Sprites de mascotas)
```

## 🎮 Mecánica de Toque (Core)

### GameManager.cs
- **`AddCoins(int amount)`**: Añade monedas al contador y actualiza UI.
- **`PurchaseUpgrade(UpgradeSO upgrade)`**: Verifica monedas, aplica efecto y guarda progreso.
- **`HandleTap(Vector2 position)`**: Detecta entrada del usuario (tactil/click).
- **Persistencia**: Usa `PlayerPrefs` para guardar/cargar monedas.

### Implementación en Update():
```csharp
if (Input.GetMouseButtonDown(0))
{
    HandleTap(Input.mousePosition);
}
```

## 📊 Sistema de Mejoras

### Crear ScriptableObjects:
1. En la carpeta `Assets/ScriptableObjects`:
   - Clic derecho → Create → TapMon → UpgradeSO
   - **Mejora_Daño**: Aumenta monedas por toque (effectValue = 1)
   - **Mejora_Auto**: Auto-clicker (effectValue = 2)
   - **Mejora_Mascota**: Evolución visual (effectValue = 3)

### Configuración de Costos:
- Mejora_Daño: baseCost = 100, costMultiplier = 2
- Mejora_Auto: baseCost = 500, costMultiplier = 1.5
- Mejora_Mascota: baseCost = 1000, costMultiplier = 1

## 🔧 UI Básica

### Configurar Canvas:
1. Crear Canvas en la escena
2. Añadir `TMP_Text` para mostrar contador de monedas (asignar en GameManager)
3. Crear área táctil grande con componente `Button`
4. Asignar `GameManager.Instance.AddCoins(1)` en el evento OnClick

### Configurar Texto de Monedas:
- Asignar el objeto `coinDisplay` en el Inspector del GameManager
- Formato: `"{CurrentCoins}"`

## 📱 Configuración de Android

### Build Settings:
- **File > Build Settings > Switch Platform** → Android
- **Target API Level**: Automatic (highest installed)
- **Scripting Backend**: IL2CPP
- **Target Architectures**: ARM64

### Keystore (Para Build Final):
1. `File > Build Settings > Player Settings > Publishing Settings`
2. Crear Keystore con `Keystore Manager`
3. Generar `.aab` para Google Play
4. Firma obligatoria para publicación en Google Play Store

### Nota importante:
Los archivos `.asset` anteriores contenían código C# con una extensión incorrecta y no eran assets válidos de Unity. Las mejoras deben crearse con `Create > TapMon > UpgradeSO` y configurarse en el Inspector.

AdMob está desactivado por defecto porque el SDK no forma parte de este repositorio. Después de instalarlo, añade `ADMOB_ENABLED` en `Project Settings > Player > Scripting Define Symbols`.

### Publicación:
El agente no generará el Keystore manualmente. Para crear el `.aab` o `.apk` firmado:
1. Abre el Keystore Manager en Unity
2. Crea un nuevo Keystore (.keystore) con contraseña
3. Genera el Alias con clave privada
4. Exporta el .aab desde Build Settings

## 🎯 Checklist de MVP

- [ ] Proyecto creado con plantilla 2D Mobile
- [x] GameManager.cs implementado con AddCoins
- [x] UpgradeSO.cs creado como ScriptableObject
- [x] MainMenu.unity creada con cámara, luz y Canvas
- [x] Contador TMP conectado a GameManager.coinDisplay
- [x] Botón central conectado a GameManager.AddCoins(1)
- [ ] 3 instancias de mejoras creadas
- [x] UI con TMP_Text para contador
- [x] Botón/área táctil para generar monedas
- [ ] PlayerPrefs configurado para persistencia
- [ ] Player Settings configurado para Android
- [ ] Build Settings cambiado a Android
- [ ] Keystore preparado para publicación final

## 🔗 Dependencias

El archivo `Packages/manifest.json` incluye `com.unity.textmeshpro` requerido para el sistema de UI.

---
*Última actualización: Septiembre 2026*
