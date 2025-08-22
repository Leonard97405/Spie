namespace Spia
{
    using LabApi.Features.Wrappers;
    using NetRoleManager;
    using PlayerRoles;

    
        public class Ntf_Spy : CustomRole
        {
            public override string RoleId { get; set; } = "NtfSpy";
            public override string Name { get; set; } = "Spia Ntf";
            public override string Description { get; set; } = "Sei una spia, puoi uccidere i tuoi teammate chaos";
            public override RoleTypeId RoleTypeId { get; set; } = RoleTypeId.ChaosMarauder;
            public override int SpawnChance { get; set; } = 0;

            public override ItemType[] Inventory { get; set; } = new[]
            {
                ItemType.GunShotgun,
                ItemType.GunRevolver,
                ItemType.Medkit,
                ItemType.Painkillers,
                ItemType.ArmorCombat,
            };

            public override void OnRoleAdded(Player player)
            {
                player.AddAmmo(ItemType.Ammo12gauge,24);
                player.AddAmmo(ItemType.Ammo44cal,24);
            }
        }
    }
