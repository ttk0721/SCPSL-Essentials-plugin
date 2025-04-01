using System.Collections.Generic;
using UnityEngine;
using PluginAPI.Core;

namespace CustomPlugin
{
    public class LosoweAwarieSwiatla
    {
        private readonly CustomPlugin plugin;
        private HashSet<GameObject> brokenElevators = new HashSet<GameObject>();

        public LosoweAwarieSwiatla(CustomPlugin plugin)
        {
            this.plugin = plugin;
        }
    }
}