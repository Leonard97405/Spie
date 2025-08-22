using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using HarmonyLib;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using Random = UnityEngine.Random;

namespace Spia
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Singleton = null;
        public override void Enable()
        {
            Singleton = this;
            NetRoleManager.NetRoleManager.Instance.RegisterRole(Singleton.Config.CS);
            NetRoleManager.NetRoleManager.Instance.RegisterRole(Singleton.Config.NtfSpy);
            CustomHandlersManager.RegisterEventsHandler(new RoleEvents());

        }

        public override void Disable()
        {
            Singleton = null;
            CustomHandlersManager.UnregisterEventsHandler(new RoleEvents());
        }

        public override string Name { get; } = "The Spy";
        public override string Description { get; } = "";
        public override string Author { get; } = "Lenard";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);
    }

    public class Config
    {
        public Ntf_Spy NtfSpy = new Ntf_Spy();
        public ChaosSpy CS = new ChaosSpy();

        [Description("Quante possibilità ci sono che la spia spawni")]
        public int SpyChance { get; set; } = 33;

        [Description(
            "Rapporto spie per player ( su 100 ) in una wave, consiglio di tenerlo basso ( ne spawnerà sempre almeno una se il min di player è raggiunto ) ")]
        public int rappS { get; set; } = 10;

        public int minPlayer { get; set; } = 1;
    }

    public class RoleEvents : CustomEventsHandler
    {
        private static Dictionary<Player, bool> spieSparato = new Dictionary<Player, bool>();
        public override void OnServerWaveRespawned(WaveRespawnedEventArgs ev)
        {
            int SpieSpawnate = 0;
            int maxSpie = ev.Players.Count / 100 * Plugin.Singleton.Config.rappS;
            if (maxSpie == 0) maxSpie = 1;
            if (ev.Wave.Faction == Faction.FoundationEnemy)
            {
                foreach (var z in ev.Players.Where(p => p.Role == RoleTypeId.ChaosMarauder))
                {
                    if (RandomNumberGenerator.GetInt32(101) < Plugin.Singleton.Config.SpyChance &&
                        SpieSpawnate < maxSpie)
                    {
                        NetRoleManager.NetRoleManager.Instance.AssignRole(z,Plugin.Singleton.Config.NtfSpy);
                        SpieSpawnate++;
                        spieSparato.Add(z,false);
                    }
                }
            }
            else
            {
                foreach (var z in ev.Players.Where(p => p.Role == RoleTypeId.NtfPrivate))
                {
                    if (RandomNumberGenerator.GetInt32(101) < Plugin.Singleton.Config.SpyChance &&
                        SpieSpawnate < maxSpie)
                    {
                        NetRoleManager.NetRoleManager.Instance.AssignRole(z,Plugin.Singleton.Config.NtfSpy);
                        SpieSpawnate++;
                        spieSparato.Add(z,false);
                    }
                }  
            }
        }

        public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
        {
            if (ev.Attacker == null) return;
            if (ev.Attacker.Faction == ev.Player.Faction)
            {
                
                if (spieSparato.ContainsKey(ev.Attacker) && ev.DamageHandler is AttackerDamageHandler hd)
                { 
                    AccessTools.Field(typeof(AttackerDamageHandler),"ForceFullFriendlyFire").SetValue(hd,true);
                    spieSparato[ev.Attacker] = true;
                }
                else if (spieSparato.ContainsKey(ev.Player))
                {
                    if (spieSparato[ev.Player] && ev.DamageHandler is AttackerDamageHandler zd )
                    {
                        AccessTools.Field(typeof(AttackerDamageHandler),"ForceFullFriendlyFire").SetValue(zd,true);
                    }
                }
            } else if ((ev.Attacker.Faction == Faction.FoundationEnemy && ev.Player.Faction == Faction.FoundationStaff)
                       || ( ev.Attacker.Faction == Faction.FoundationStaff &&
                       ev.Player.Faction == Faction.FoundationEnemy ) && ( spieSparato.ContainsKey(ev.Attacker) || spieSparato.ContainsKey(ev.Player) ))
            {
                ev.IsAllowed = false;
            }
        }
        
        
        
       
    }
}