using Facepunch;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Custom Map Vehicle Vendor Fix", "Pinkstink", "1.0.3")]
    [Description("Links the VehicleVendor NPC with the VehicleSpawner entity")]
    public class CustomMapVehicleVendorFix : RustPlugin
    {
        void OnServerInitialized()
        {
            var vehicleVendors = UnityEngine.Object.FindObjectsOfType<VehicleVendor>();
            if (vehicleVendors == null || vehicleVendors.Length < 1)
            {
                PrintWarning("Failed to find any Vehicle Vendors entity");
                return;
            }

            foreach (var vehicleVendor in vehicleVendors)
            {
                if (vehicleVendor == null)
                    continue;

                if (vehicleVendor.GetVehicleSpawner() != null && vehicleVendor.vehicleSpawner != null)
                    continue;

                var vehicleSpawners = Pool.GetList<VehicleSpawner>();
                Vis.Entities(vehicleVendor.transform.position, 100f, vehicleSpawners);
                if (vehicleSpawners == null || vehicleSpawners.Count < 1)
                {
                    PrintError($"Failed to find Vehicle Spawner for Vendor @ {vehicleVendor.transform.position}");
                    continue;
                }

                VehicleSpawner vehicleSpawner;
                if (vehicleSpawners.Count > 1)
                {
                    vehicleSpawner = vehicleSpawners.OrderBy(x => Vector3.Distance(x.transform.position, vehicleVendor.transform.position)).First();
                }
                else
                {
                    vehicleSpawner = vehicleSpawners[0];
                }

                if (vehicleSpawner == null)
                {
                    PrintError("Failed to find Vehicle Spawner entity");
                    return;
                }

                vehicleVendor.spawnerRef.Set(vehicleSpawner);
                vehicleVendor.vehicleSpawner = vehicleSpawner;
                Puts($"Set Vehicle Spawner for Vehicle Vendor @ {vehicleVendor.transform.position}");
                vehicleSpawners.Clear();
                Pool.FreeList(ref vehicleSpawners);
            }
        }
    }
}
