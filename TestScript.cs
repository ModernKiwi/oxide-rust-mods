using System.Collections.Generic;
using System;
using System.Data;
using UnityEngine;
using Oxide.Core;
using Oxide.Core.Plugins;
using System.Timers;
using System.Text;
using System.Linq;
using Facepunch;
using Rust;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Oxide.Core.Configuration;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Oxide.Game.Rust.Cui;
using Oxide.Core.Libraries;
using System.Globalization;

namespace Oxide.Plugins


{
    [Info("TestScript", "Alpha", "0.0.1", ResourceId = 000)]
    class TestScript : RustPlugin
    {
        private FieldInfo serverinput;

        void OnServerInitialized()
        {
            serverinput = typeof(BasePlayer).GetField("serverInput", (BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic));
        }
        [ChatCommand("findentity")]
        void cmdFind(BasePlayer player, string command, string[] args)
        {
            BaseEntity ent = FindObject(player);
            if (ent == null) { Puts("Entity is null"); return; }
            Puts("EntityName: " + ent.name);
            Puts("Type: " + ent.GetType().ToString());
            Puts("PrefabID: " + ent.prefabID.ToString());
            Puts("NetID: " + ent.net.ID);
            Puts("OwnerID: " + ent.OwnerID);
            if (ent is BasePlayer)
            {
                BasePlayer e = ent as BasePlayer;
                Puts("Playername: " + e.displayName);
                Puts("SteamID: " + e.UserIDString);
            }
            else if (ent is LootContainer)
            {
                StorageContainer e = ent as LootContainer;
                Puts("PanelName: " + e.panelName);
            }
        }
        BaseEntity FindObject(BasePlayer player, float distance = 10f)
        {
            var input = serverinput.GetValue(player) as InputState;
            Ray ray = new Ray(player.eyes.position, Quaternion.Euler(input.current.aimAngles) * Vector3.forward);
            RaycastHit hit;
            if (!UnityEngine.Physics.Raycast(ray, out hit, distance))
                return null;
            return hit.GetEntity();
        }


        
    }
}