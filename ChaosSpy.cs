using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using NetRoleManager;
using PlayerRoles;

namespace Spia
{
    public class ChaosSpy : CustomRole
    {
        public override string RoleId { get; set; } = "ciSpy";
        public override string Name { get; set; } = "Chaos Spy";

        public override string Description { get; set; } =
            "Sei una spia della chaos insurgency, \n puoi uccidere i tuoi teammate ntf";

        public override RoleTypeId RoleTypeId { get; set; } = RoleTypeId.NtfPrivate;
        public override int SpawnChance { get; set; } = 0;

        public override ItemType[] Inventory { get; set; } = new[]
        {
            ItemType.GunCrossvec,
            ItemType.KeycardMTFOperative,
            ItemType.ArmorCombat,
            ItemType.Medkit,
            ItemType.Radio,
        };

        public override void OnRoleAdded(Player player)
        {
            player.AddAmmo(ItemType.Ammo9x19,120);
            player.AddAmmo(ItemType.Ammo556x45,40);
            RoleEvents.spieSparato.TryAdd(player,false);
        }
    }
}