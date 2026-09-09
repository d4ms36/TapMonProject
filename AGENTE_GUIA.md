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
- CP-3: [ ] Pendiente (manual del desarrollador)
- MP-1: [ ] Pendiente (código de AdMob)
- MP-2: [ ] Pendiente
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
