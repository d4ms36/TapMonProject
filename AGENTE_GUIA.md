# GUÍA PARA AGENTES DE VSCode

## Contexto del proyecto

- **Nombre:** TapMon: Mi Compi Virtual
- **Motor:** Unity 2022.3.67f2 LTS
- **Plataforma:** Android
- **Monetización:** AdMob (Banner + anuncios recompensados)
- **Almacenamiento:** PlayerPrefs local, sin servidores

## Estado actual

- CP-1: [x] Completado
- CP-2: [x] Completado
- CP-3: [x] Completado (APK generado y probado)
- MP-1: [x] Estructura de AdMob creada
- MP-2: [ ] Pendiente (instalación del SDK y APK de prueba)
- MP-3: [ ] Pendiente (mejoras)
- MP-4: [ ] Pendiente (misiones)
- LP-1: [ ] Pendiente
- LP-2: [ ] Pendiente
- LP-3: [ ] Pendiente
- LP-4: [ ] Pendiente

## Reglas para el agente

1. No abrir Unity.
2. Actualizar siempre `README.md` después de cada cambio relevante.
3. Usar IDs de prueba de AdMob durante el desarrollo.
4. Comentar el código nuevo en español cuando el comentario aporte contexto.
5. No generar APK; el build y las pruebas en dispositivo corresponden al desarrollador.
6. Validar los cambios con comprobaciones estáticas o herramientas disponibles.

## Estructura de archivos

- Scripts: `Assets/Scripts/`
- ScriptableObjects: `Assets/ScriptableObjects/`
- Escenas: `Assets/Scenes/`
- Documentación principal: `README.md`, `Docs/`
- Guía operativa: `AGENTE_GUIA.md`

## Cómo trabajar

1. Leer `README.md` y esta guía.
2. Confirmar el estado actual y localizar el archivo dueño del cambio.
3. Hacer cambios específicos y mínimos.
4. Actualizar `README.md` y la documentación relacionada.
5. Ejecutar una validación estática sin abrir Unity.
6. Reportar al PM los archivos modificados, la validación y los pendientes manuales.

## Políticas y publicación

- Mantener `Docs/PrivacyPolicy.md` y `Docs/TermsOfService.md` sincronizados con las funciones reales de la app.
- No dejar placeholders de contacto o empresa antes de publicar en Google Play.
- Documentar cualquier dato recopilado por AdMob y completar el formulario de seguridad de datos de Play Console.
- Implementar y probar consentimiento GDPR para usuarios del EEE/Reino Unido antes de mostrar anuncios personalizados.
- Usar IDs de prueba de AdMob hasta que la build de producción esté configurada y revisada.

## Integración de AdMob

- `Assets/Scripts/AdManager.cs` contiene Banner, Interstitial y Rewarded.
- `AdManager.cs` está adaptado a la API moderna 9.x: usar `InterstitialAd.Load`, `RewardedAd.Load`, `CanShowAd` y `Show(Action<Reward>)`.
- No volver a usar `new InterstitialAd`, `new RewardedAd`, `IsLoaded`, `OnAdRewarded` ni `LoadAd` para interstitial/rewarded.
- Los IDs reales de TapMon están documentados en el script; los IDs de prueba se seleccionan con `useTestAds`.
- Mantener `useTestAds` en `true` durante desarrollo y pruebas; cambiarlo a `false` solo antes de una build de producción revisada.
- Instalar el plugin oficial, definir `ADMOB_ENABLED` y configurar el App ID de Android en el SDK.
- Probar consentimiento GDPR antes de inicializar anuncios personalizados.
- El Rewarded entrega 500 monedas mediante `GameManager.OnRewardEarned()` solo después de `OnUserEarnedReward`.

## Error de PlayServicesResolver

- Si aparece `XmlException: Data at the root level is invalid`, validar todos los XML antes de cambiar código.
- `Assets/Plugins/Android/AndroidManifest.xml` debe empezar con la declaración XML y no puede contener `using UnityEngine` ni otro código C#.
- No borrar manifiestos válidos del SDK; comparar primero con `GoogleMobileAdsPlugin.androidlib/AndroidManifest.xml`.
- Con Unity abierto por el desarrollador, revisar `Assets > External Dependency Manager > Android Resolver > Settings`, desactivar la resolución automática si está causando regeneraciones problemáticas y ejecutar `Force Resolve` manualmente.
- No abrir Unity desde el agente; documentar el diagnóstico y pedir al desarrollador que ejecute Force Resolve.

## Generación de APK desde terminal

- No abrir Unity manualmente ni generar APK desde el agente sin una instrucción explícita del PM.
- La build automatizada usa `Assets/Editor/BuildScript.cs` y el método `BuildScript.BuildAndroidAdMob`.
- Ejecutar Unity `2022.3.67f2` con `-batchmode -quit -nographics` y guardar la salida en `build.log`.
- El resultado esperado es `Builds/TapMon_AdMob.apk`; verificar que existe y registrar su tamaño.
- Confirmar que `useTestAds = true` y que el SDK de AdMob está instalado antes de compilar.
- No ejecutar la build con otra versión de Unity ni sustituir los IDs de prueba durante MP-2.
- Si la build falla, revisar primero `build.log` y reportar el código de salida sin incluir carpetas generadas en Git.
- Si no existe `C:\Program Files\Unity\Hub\Editor\2022.3.67f2\Editor\Unity.exe` o el SDK de AdMob no está instalado, detenerse y reportarlo; no usar otra versión de Unity ni simular `Builds/TapMon_AdMob.apk`.

## MP-2: SDK y APK de prueba

- Consultar `README.md` y `Docs/MP2_CHECKLIST.md` antes de cambiar la configuración del SDK.
- Instalar el plugin oficial Google Mobile Ads Unity desde Package Manager; no inventar dependencias ni modificar `Packages/manifest.json` sin verificar el paquete.
- Activar `ADMOB_ENABLED` únicamente después de instalar el SDK y confirmar que sus namespaces están disponibles.
- Añadir `AdManager` a `Assets/Scenes/MainMenu.unity` como GameObject y asignar `Assets/Scripts/AdManager.cs`.
- Mantener `useTestAds = true` durante todo MP-2, incluso en el APK instalado en el celular.
- Probar banner, interstitial y rewarded con los IDs oficiales de prueba `ca-app-pub-3940256099942544/...`.
- No cambiar a IDs reales, no generar clics artificiales y no marcar MP-2 como completado sin evidencia del dispositivo.
- No abrir Unity ni generar APK desde el agente; esas acciones corresponden al desarrollador.
