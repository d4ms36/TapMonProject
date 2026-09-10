using UnityEngine;

public enum MissionType
{
    Toques,
    Gastos,
    Mejoras
}

[CreateAssetMenu(fileName = "NuevaMisionDiaria", menuName = "TapMon/Mision diaria")]
public class DailyMission : ScriptableObject
{
    public string missionName = "Nueva misión";
    [TextArea] public string description = "Descripción de la misión.";
    [Min(1)] public int target = 100;
    [Min(1)] public int reward = 200;
    public MissionType missionType = MissionType.Toques;
}
