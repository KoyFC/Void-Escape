using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    [System.Serializable]
    public enum ItemType
    {
        Ship,
        Color,
        Upgrade
    }

    public ItemType m_ItemType;
    public int m_ID;
    public int m_Price;
}
