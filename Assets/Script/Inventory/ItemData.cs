using UnityEngine;

[CreateAssetMenu(fileName = "New ItemData", menuName = "Item/Create New ItemData")]
public class ItemData : ScriptableObject {
    public new string name;
    public Sprite icon;
    public string description;
    // NOTE: qui tutti gli altri campi che possono essere utili
}