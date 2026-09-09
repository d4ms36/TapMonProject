# ROADMAP DEL PROYECTO: TapMon - Mi Compi Virtual

## 📌 INFORMACIÓN GENERAL

| Campo | Valor |
|-------|-------|
| **Nombre del juego** | TapMon: Mi Compi Virtual |
| **Nombre corto** | TapMon |
| **Package Name** | com.tuempresa.tapmon |
| **Plataforma** | Android |
| **Motor** | Unity 2022.3.67f2 LTS |
| **Monetización** | AdMob (Banner + Recompensados) |
| **Almacenamiento** | Local (PlayerPrefs) - SIN SERVIDORES |
| **Público** | 18-30 años |
| **Estado actual** | CP-1 y CP-2 completados; próximo paso CP-3 |

---

## 🎯 OBJETIVO FINAL

Tener un juego de toques (clicker) funcional en Google Play Store con:
- Mecánica de toque para ganar monedas
- Sistema de mejoras (3 tipos iniciales)
- Mascota virtual que evoluciona
- Misiones diarias
- Anuncios no invasivos (Banner + Recompensados)
- Cumplimiento de políticas de Google Play

---

## 📊 FASES DEL PROYECTO

### FASE 1: CONFIGURACIÓN DEL PROYECTO UNITY
**Estado:** ✅ Completado
**Duración estimada:** 15 minutos
**Responsable:** Tú (desarrollador)

#### Objetivo
Que Unity reconozca el proyecto como válido y pueda abrirlo sin errores.

#### Tareas
1. Abrir Unity Hub
2. Crear nuevo proyecto con plantilla **"2D Mobile"**
   - Nombre: `TapMonProject`
   - Ubicación: Elegir una carpeta accesible
3. Cerrar Unity
4. Copiar scripts base a `Assets/Scripts/`:
   - `GameManager.cs`
   - `UpgradeSO.cs`
5. Eliminar temporalmente `AndroidManifest.xml` (si existe)
6. Abrir Unity y verificar que NO haya errores en consola

#### Criterio de éxito
- Unity abre el proyecto sin errores rojos
- Los scripts aparecen en el inspector de Unity

#### Documentación requerida
- Captura de pantalla de la consola de Unity (limpia)
- Lista de scripts copiados

---

### FASE 2: CREACIÓN DE LA ESCENA MÍNIMA
**Estado:** ✅ Completado
**Duración estimada:** 30 minutos
**Responsable:** Tú (desarrollador)

#### Objetivo
Tener una escena jugable con interacción básica (toque + contador).

#### Tareas
1. Crear escena `MainMenu.unity` (o usar `SampleScene`)
2. Configurar elementos de UI:
   - **Canvas** con:
     - `Text (TMP)` para mostrar monedas (parte superior)
     - `Button` grande en el centro (será la mascota)
     - `Text (TMP)` en el botón con "¡Tócame!"
3. Crear GameObject vacío llamado `GameManager`
4. Asignar script `GameManager.cs` al GameObject
5. Vincular referencias en el inspector:
   - Arrastrar el texto de monedas a `coinDisplay`
6. Configurar evento del botón:
   - OnClick → `GameManager.AddCoins(1)`
7. Guardar la escena como `MainMenu.unity`

#### Criterio de éxito
- Al hacer clic en el botón, el contador de monedas aumenta
- La UI se ve limpia y organizada

#### Documentación requerida
- Captura de pantalla de la escena en Unity
- Captura de pantalla del juego en el editor (con contador)

---

### FASE 3: GENERACIÓN DE APK SIN ANUNCIOS
**Estado:** ⬜ Pendiente (siguiente paso manual)
**Duración estimada:** 20 minutos
**Responsable:** Tú (desarrollador)

#### Objetivo
Generar un APK de prueba que se pueda instalar en un celular Android.

#### Tareas
1. Configurar plataforma Android:
   - `File > Build Settings`
   - Seleccionar "Android" → "Switch Platform"
2. Añadir escena `MainMenu` a la lista de escenas
3. Configurar Player Settings:
   - `Package Name`: `com.tuempresa.tapmon`
   - `Target API Level`: `Automatic (highest installed)`
   - `Scripting Backend`: `Mono` (para pruebas más rápidas)
   - `Internet Access`: `Auto`
4. Generar APK:
   - `Build` → Guardar como `TapMon_Test.apk`
5. Copiar APK al celular e instalar
6. Abrir el juego y probar la mecánica de toque

#### Criterio de éxito
- El APK se instala sin errores en el celular
- El juego se abre y el contador funciona al tocar

#### Documentación requerida
- Tamaño del APK generado
- Captura de pantalla del juego en el celular
- Lista de errores (si los hubo)

---

### FASE 4: INTEGRACIÓN DE ADMOB (CÓDIGO)
**Estado:** ⬜ Pendiente (Agente IA)
**Duración estimada:** 20 minutos (agente) + 10 minutos (tú)
**Responsable:** Agente IA (código) + Tú (integración)

#### Objetivo
Añadir el código necesario para mostrar anuncios de AdMob.

#### Tareas del Agente IA
1. Crear `AdManager.cs` con:
   - Inicialización de AdMob
   - Carga de Banner (parte inferior)
   - Carga de Anuncio Recompensado
   - Método `ShowRewardedAd(Action<bool> onReward)`
2. Actualizar `GameManager.cs`:
   - Añadir `WatchAdForCoins()`
   - Añadir `OnRewardEarned()`
3. Crear `Assets/Editor/BuildScript.cs` para build automático
4. Actualizar `AndroidManifest.xml` con permisos:
   ```xml
   <uses-permission android:name="android.permission.INTERNET" />
   <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />