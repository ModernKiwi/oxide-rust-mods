using System.Reflection;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Core.Libraries;
using System;                      //DateTime
using System.Data;
using System.Linq;
using System.Collections.Generic;  //Required for Whilelist
using UnityEngine;
using Rust;
using System.Globalization;

namespace Oxide.Plugins
{
    [Info("Mod Name", "Alphawar", "0.0.0", ResourceId = 0000)]
    [Description("Mod for handling Thief class.")]
    class Templates : RustPlugin
    {

        //Test
        private void Loaded()
        {
            Puts(this +" Is loaded");
            permissionHandle();
        }

        #region Rust:Config Template
        //Call using LoadVariables();
        // Use in protected override void LoadDefaultConfig() to create config, message to Rcon recommended
        // Use in private void Loaded() to load config when mod is loaded

        private bool Template1;
        private double Template2;
        private float Template3;
        private int Template4;
        private string Template5;

        void LoadVariables() //Stores Default Values, calling GetConfig passing: menu, dataValue, defaultValue
        {
            //Booleans
            Template1 = Convert.ToBoolean(GetConfig("category", "Desciption", false));
            //Double
            Template2 = Convert.ToDouble(GetConfig("category", "Desciption", 1));
            //Floats (aka ToSingle)
            Template3 = Convert.ToSingle(GetConfig("category", "Desciption", 1f));
            //Ints
            Template4 = Convert.ToInt32(GetConfig("category", "Desciption", 1));
            //Strings
            Template5 = Convert.ToString(GetConfig("category", "Desciption", "1"));
        }

        object GetConfig(string menu, string dataValue, object defaultValue){
            var data = Config[menu] as Dictionary<string, object>;if (data == null){
                data = new Dictionary<string, object>();Config[menu] = data;}
            object value;if (!data.TryGetValue(dataValue, out value)){
                value = defaultValue;data[dataValue] = value;}return value;}
        #endregion


        #region Rust:Permission registry

        //////////////////////////////////////////////////////////////////////////////////////
        // Permision /////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////
        void permissionHandle()
        {
            string[] Permissionarray = { "admin", "wipe" };
            foreach (string i in Permissionarray)
            {
                string regPerm = Title.ToLower() + "." + i;
                Puts("Checking if " + regPerm + " is registered.");
                if (!permission.PermissionExists(regPerm))
                {
                    permission.RegisterPermission(regPerm, this);
                    Puts(regPerm + " is registered.");
                }
                else
                {
                    Puts(regPerm + " is already registered.");
                }
            }
        }

        #endregion
    }
}
