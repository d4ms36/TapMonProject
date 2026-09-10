using UnityEngine;

/// <summary>
/// ScriptableObject que define una mejora para el juego TapMon: Mi Compi Virtual.
/// Configúralo desde el Inspector con su nombre, descripción, costo e icono.
/// </summary>
[CreateAssetMenu(fileName = "NewUpgrade", menuName = "TapMon/UpgradeSO")]
public class UpgradeSO : ScriptableObject
{
    public enum UpgradeEffect
    {
        Danio,
        AutoClicker,
        Mascota
    }

    [Header("Identificación")]
    public string nombre = "Nueva Mejora";
    [TextArea]
    public string descripcion = "Descripción de la mejora.";

    [Header("Configuración de coste")]
    [Min(1)] public int costeBase = 100;
    [Min(1f)] public float multiplicadorCoste = 1.5f;

    public UpgradeEffect efecto = UpgradeEffect.Danio;
    public Sprite icono;

    public int GetCostForLevel(int level)
    {
        float cost = costeBase * Mathf.Pow(multiplicadorCoste, Mathf.Max(0, level));
        return Mathf.Max(1, Mathf.CeilToInt(cost));
    }
}