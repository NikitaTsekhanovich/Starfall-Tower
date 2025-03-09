using UnityEngine;

namespace StoreControllers
{
    [CreateAssetMenu(fileName = "ConfigAbility", menuName = "Config Abilities/ Config")]
    public class ConfigAbility : ScriptableObject
    {
        [field: SerializeField] public int Index { get; private set; }
        [field: SerializeField] public int Cost { get; private set; }
        [field: SerializeField] public Sprite StoreIcon { get; private set; }
        [field: SerializeField] public string NameItem { get; private set; }
        [field: SerializeField] public string DescriptionItem { get; private set; }

        public string CountAbilityKey => $"{StoreSaveKeys.AbilitiesCountKey}{Index}";
        public int CountAbility => PlayerPrefs.GetInt($"{CountAbilityKey}");
    }
}
