namespace StoreControllers
{
    public class ContainerAbilitiesConfigs
    {
        public ConfigAbility[] AbilitiesConfigs { get; private set; }

        public ContainerAbilitiesConfigs(ConfigAbility[] abilitiesConfigs)
        {
            AbilitiesConfigs = abilitiesConfigs;
        }
    }
}
