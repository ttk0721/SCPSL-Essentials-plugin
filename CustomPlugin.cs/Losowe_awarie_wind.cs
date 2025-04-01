using System.Collections.Generic;
using UnityEngine;
using PluginAPI.Core;

namespace CustomPlugin
{
    public class LosoweAwarieWind
    {
        private readonly CustomPlugin plugin;
        private HashSet<GameObject> brokenElevators = new HashSet<GameObject>();

        public LosoweAwarieWind(CustomPlugin plugin)
        {
            this.plugin = plugin;
        }

        // Placeholder dla awarii wind
        private void BreakRandomElevator()
        {
            // Logika awarii wind (do zaimplementowania)
            Log.Info("Symulacja awarii windy...");
        }
    }
}