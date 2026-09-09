# Checklist MP-2: APK con anuncios de prueba

**Proyecto:** TapMon: Mi Compi Virtual  
**Unity:** 2022.3.67f2 LTS  
**Escena:** `Assets/Scenes/MainMenu.unity`  
**APK:** `Builds/TapMon_MP2_Test.apk`

> Esta checklist se completa manualmente por el desarrollador. El agente no abre Unity ni genera APK.

## 1. Preparación del SDK

- [ ] Confirmar que el proyecto abre con Unity 2022.3.67f2 LTS sin errores rojos.
- [ ] Instalar el plugin oficial Google Mobile Ads Unity desde Package Manager.
- [ ] Instalar o resolver External Dependency Manager si el plugin lo solicita.
- [ ] Confirmar que el SDK de Google Mobile Ads está disponible en el proyecto.
- [ ] Configurar el App ID de prueba de Android en el plugin.
- [ ] Añadir `ADMOB_ENABLED` a `Project Settings > Player > Scripting Define Symbols` para Android.
- [ ] Confirmar que la escena compila con el SDK activado.

**Notas de instalación:**

- Fecha: ____________________
- Versión del plugin: ____________________
- App ID de prueba utilizado: ____________________
- Errores encontrados: ________________________________________________

## 2. Configuración de la escena

- [ ] Abrir `Assets/Scenes/MainMenu.unity`.
- [ ] Crear o localizar un GameObject llamado `AdManager`.
- [ ] Asignar `Assets/Scripts/AdManager.cs` al GameObject.
- [ ] Confirmar que el componente `AdManager` está habilitado.
- [ ] Confirmar que `useTestAds` está en `true`.
- [ ] Confirmar que el banner, interstitial y rewarded usan IDs oficiales de prueba.
- [ ] Confirmar que `GameManager` existe y que el rewarded puede llamar a `GameManager.OnRewardEarned()`.
- [ ] Guardar la escena.

## 3. Build Android

- [ ] Abrir `File > Build Settings`.
- [ ] Seleccionar Android y pulsar `Switch Platform` si es necesario.
- [ ] Añadir `Assets/Scenes/MainMenu.unity` a `Scenes In Build`.
- [ ] Confirmar que `ADMOB_ENABLED` está activo para Android.
- [ ] Confirmar que `useTestAds` continúa en `true` antes de compilar.
- [ ] Generar el APK como `Builds/TapMon_MP2_Test.apk`.
- [ ] Registrar el tamaño del APK: ____________________
- [ ] Registrar el hash del APK, si se desea: ____________________

## 4. Instalación en dispositivo

- [ ] Usar un dispositivo Android con conexión a Internet.
- [ ] Desinstalar una versión anterior si puede conservar una configuración incompatible.
- [ ] Instalar `TapMon_MP2_Test.apk`.
- [ ] La aplicación inicia sin cerrarse.
- [ ] No aparecen errores de App ID o inicialización del SDK.

**Dispositivo:** ____________________  
**Versión de Android:** ____________________  
**Fecha de prueba:** ____________________

## 5. Pruebas funcionales de anuncios

### Banner

- [ ] El banner aparece en la parte inferior.
- [ ] El banner no tapa `CoinDisplay`, `TapButton` ni controles importantes.
- [ ] El banner carga usando el ID oficial de prueba.
- [ ] La aplicación sigue funcionando si el banner no está disponible.

Resultado/notas: ______________________________________________________

### Interstitial

- [ ] Existe un botón o flujo de prueba que llama a `ShowInterstitial()`.
- [ ] El interstitial aparece después de una acción apropiada, como completar una misión.
- [ ] El interstitial no aparece al iniciar repetidamente ni bloquea el juego.
- [ ] Al cerrar el anuncio, se carga el siguiente interstitial.
- [ ] La aplicación continúa funcionando si el anuncio no está cargado.

Resultado/notas: ______________________________________________________

### Rewarded

- [ ] Existe un botón o flujo de prueba que llama a `ShowRewardedAd(...)`.
- [ ] El anuncio se muestra solo después de una acción voluntaria.
- [ ] Al completar el anuncio, se entregan exactamente 500 monedas.
- [ ] La recompensa no se entrega si el anuncio se cierra antes de completarse.
- [ ] La recompensa no se entrega dos veces por una sola visualización.
- [ ] Se carga un rewarded nuevo después de cerrar el anterior.

Monedas antes: __________  Monedas después: __________  
Resultado/notas: ______________________________________________________

## 6. Criterios de aceptación

- [ ] Banner visible y no intrusivo.
- [ ] Interstitial funcional en el flujo de prueba.
- [ ] Rewarded funcional con recompensa de 500 monedas.
- [ ] Sin errores rojos relacionados con AdMob.
- [ ] `useTestAds` sigue en `true`.
- [ ] APK probado en al menos un dispositivo real.
- [ ] Evidencias guardadas: capturas, versión del APK y notas de errores.

## 7. Antes de producción

- [ ] Implementar y probar consentimiento GDPR donde corresponda.
- [ ] Completar la declaración de seguridad de datos de Google Play.
- [ ] Verificar la política de privacidad y términos publicados.
- [ ] Revisar los IDs reales de producción.
- [ ] Cambiar `useTestAds` a `false` únicamente en la build de producción revisada.
- [ ] Nunca probar IDs reales con clics o impresiones artificiales.
