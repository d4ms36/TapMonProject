📋 PLAN DE IMPLEMENTACIÓN - TAPMON
🎯 VISIÓN GENERAL
Tipo	Horizonte	Objetivo Principal
Corto plazo	Tú decides	Tener un APK funcional instalable en celular con mecánica de toque
Mediano plazo	Tú decides	Juego completo con mejoras, misiones y anuncios funcionales
Largo plazo	Tú decides	Publicación en Google Play con todos los requisitos cumplidos
📊 OBJETIVOS A CORTO PLAZO
Objetivo CP-1: Proyecto Unity funcional
Estado: ✅ Completado
Tareas:

□ Crear proyecto 2D Mobile en Unity Hub
□ Copiar scripts base (GameManager, UpgradeSO)
□ Verificar que Unity abre sin errores
Criterio de éxito: Consola de Unity limpia, scripts visibles en el inspector.

Objetivo CP-2: Escena jugable mínima
Estado: ✅ Completado
Tareas:

□ Crear Canvas con contador de monedas
□ Crear botón central (mascota) con evento de toque
□ Vincular GameManager con la UI
Criterio de éxito: Al tocar el botón, el contador aumenta.

Objetivo CP-3: Primer APK instalable
Estado: ⬜ Pendiente
Tareas:

□ Configurar Android en Build Settings
□ Configurar Player Settings (Package Name, API Level)
□ Generar APK de prueba
□ Instalar en celular y probar
Criterio de éxito: El APK se instala y el juego funciona en el celular.

Objetivo CP-4: Documentación y control de versiones
Estado: ⬜ Pendiente
Tareas:

□ Crear repositorio en GitHub
□ Subir el proyecto con .gitignore
□ Documentar los pasos de las fases 1-3
Criterio de éxito: Repositorio GitHub con el código base y documentación inicial.

📊 OBJETIVOS A MEDIANO PLAZO
Objetivo MP-1: Integración de AdMob
Estado: ⬜ Pendiente (Agente IA)
Tareas:

□ Generar AdManager.cs con lógica de anuncios
□ Actualizar GameManager con WatchAdForCoins()
□ Instalar SDK de AdMob desde Package Manager
□ Probar anuncios de prueba en el editor
Criterio de éxito: El banner se muestra en la parte inferior. Los anuncios recompensados cargan y entregan monedas.

Objetivo MP-2: APK con anuncios de prueba
Estado: ⬜ Pendiente
Tareas:

□ Generar APK con AdMob integrado
□ Instalar en celular
□ Probar banner y anuncios recompensados
Criterio de éxito: Los anuncios de prueba funcionan en el celular.

Objetivo MP-3: Sistema de mejoras funcional
Estado: ⬜ Pendiente (Agente IA)
Tareas:

□ Crear 3 ScriptableObjects (Daño, Auto, Mascota)
□ Implementar lógica de compra en GameManager
□ Crear UI de tienda con botones y precios
□ Probar que las mejoras afectan al juego
Criterio de éxito: Se pueden comprar mejoras y afectan al gameplay (más monedas, auto-clicker, evolución visual).

Objetivo MP-4: Sistema de misiones diarias
Estado: ⬜ Pendiente (Agente IA)
Tareas:

□ Crear DailyMission (clase o ScriptableObject)
□ Implementar lógica de misiones en GameManager
□ Guardar progreso en PlayerPrefs
□ Crear UI de misiones con progreso
Criterio de éxito: Las misiones se completan, entregan recompensas y se reinician diariamente.

📊 OBJETIVOS A LARGO PLAZO
Objetivo LP-1: Pulido y optimización
Estado: ⬜ Pendiente
Tareas:

□ Mejorar la UI (colores, animaciones, feedback visual)
□ Optimizar rendimiento (reducir tamaño del APK)
□ Probar en múltiples dispositivos (si es posible)
□ Corregir bugs detectados
Criterio de éxito: El juego se ve profesional y funciona sin errores.

Objetivo LP-2: Documentación legal completa
Estado: ⬜ Pendiente
Tareas:

□ Crear PrivacyPolicy.md y TermsOfService.md
□ Subir a GitHub Pages
□ Configurar URL en Unity Player Settings
□ Verificar que los enlaces sean públicos
Criterio de éxito: Documentos legales accesibles públicamente desde la app.

Objetivo LP-3: Generación de keystore y build final
Estado: ⬜ Pendiente
Tareas:

□ Crear keystore personal en Unity
□ Guardar contraseñas de forma segura
□ Generar APK firmado (release)
□ Probar la build final en el celular
Criterio de éxito: Build firmada lista para subir a Google Play.

Objetivo LP-4: Publicación en Google Play
Estado: ⬜ Pendiente
Tareas:

□ Crear cuenta de desarrollador en Google Play (si no tienes)
□ Preparar assets (icono, capturas de pantalla, descripción)
□ Subir APK/AAB a Google Play Console
□ Completar ficha de la app
□ Enviar para revisión
Criterio de éxito: La app está disponible en Google Play Store.

📋 TABLA RESUMEN DE OBJETIVOS
ID	Objetivo	Horizonte	Estado	Responsable
CP-1	Proyecto Unity funcional	Corto	⬜	Tú
CP-2	Escena jugable mínima	Corto	⬜	Tú
CP-3	Primer APK instalable	Corto	⬜	Tú
CP-4	Documentación y GitHub	Corto	⬜	Tú
MP-1	Integración AdMob	Medio	⬜	Agente IA
MP-2	APK con anuncios	Medio	⬜	Tú
MP-3	Sistema de mejoras	Medio	⬜	Agente IA
MP-4	Misiones diarias	Medio	⬜	Agente IA
LP-1	Pulido y optimización	Largo	⬜	Ambos
LP-2	Documentación legal	Largo	⬜	Tú
LP-3	Keystore y build final	Largo	⬜	Tú
LP-4	Publicación en Play Store	Largo	⬜	Tú
📊 PROGRESO ACTUAL
Estado actual: CP-1 y CP-2 completados; CP-3 pendiente de ejecución manual

text
[████████░░░░░░░░░░░░░░░░░░░░░░░░] 25%
Próximo objetivo: CP-3 - Primer APK instalable

📝 NOTAS IMPORTANTES
Para el Desarrollador (Tú)
Sigue el orden de los objetivos - No saltes fases

Documenta cada objetivo con capturas de pantalla

Sube a GitHub después de completar cada objetivo

Pregunta si tienes dudas - No avances con errores

Para el Agente IA
Genera código solo cuando se te pida (objetivos MP-1, MP-3, MP-4)

Incluye comentarios en español

Usa IDs de prueba de AdMob en el código

Actualiza el README con los cambios realizados

✅ PRÓXIMA ACCIÓN INMEDIATA
Objetivo CP-1: Proyecto Unity funcional

Abre Unity Hub

Crea proyecto "2D Mobile" llamado TapMonProject

Copia GameManager.cs y UpgradeSO.cs a Assets/Scripts/

Abre Unity y verifica que no haya errores