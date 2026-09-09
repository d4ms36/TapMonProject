using UnityEngine;

/// <summary>
/// ScriptableObject que define una mejora para el juego TapMon: Mi Compi Virtual.
/// Configúralo desde el Inspector con su nombre, descripción, costo e icono.
/// </summary>
[CreateAssetMenu(fileName = "NewUpgrade", menuName = "TapMon/UpgradeSO")]
public class UpgradeSO : ScriptableObject
{
    [Header("Identificación")]
    [Tooltip("Nombre de la mejora.")]
    public string upgradeName = "Nueva Mejora";

    [Tooltip("Descripción del efecto de la mejora.")]
    [TextArea]
    public string description = "Descripción de la mejora.";

    [Header("Configuración de Costo")]
    [Tooltip("Costo base en monedas para la primera compra.")]
    public int baseCost = 100;

    [Tooltip("Multiplicador de costo por nivel. 1 = fijo, 2 = se duplica, 1.5 = aumenta un 50%.")]
    [Min(1f)] public float costMultiplier = 1f;

    [Header("Configuración de Efecto")]
    [Tooltip("Valor del efecto: 1 = Aumenta monedas por toque, 2 = Auto-clicker, 3 = Evolución visual.")]
    public int effectValue = 1;

    [Tooltip("Icono de la mejora para mostrar en el menú de tienda.")]
    public Sprite icon;

    [Header("Estado")]
    [Tooltip("Número de veces que se ha comprado esta mejora.")]
    public int currentPurchaseCount = 0;

    [Tooltip("¿Está desbloqueada esta mejora?")]
    public bool isUnlocked = true;

    /// <summary>
    /// Obtiene el costo actualizado de la mejora según las compras previas.
    /// </summary>
    public int GetCurrentCost()
    {
        float cost = baseCost * Mathf.Pow(costMultiplier, currentPurchaseCount);
        return Mathf.Max(1, Mathf.CeilToInt(cost));
    }

    /// <summary>
    /// Obtiene el nivel actual de la mejora.
    /// </summary>
    public int GetCurrentLevel()
    {
        return currentPurchaseCount + 1;
    }

    /// <summary>
    /// Obtiene el multiplicador de efecto actual.
    /// </summary>
    public float GetEffectMultiplier()
    {
        return 1f + (currentPurchaseCount * 0.1f); // +10% por nivel
    }
}