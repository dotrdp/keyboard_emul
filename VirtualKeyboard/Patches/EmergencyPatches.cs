using HarmonyLib;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Emergency patch that tries to intercept ALL possible input methods.
    /// This is a brute-force approach to find where input is actually being checked.
    /// </summary>
    public class EmergencyInputPatch : IPatch
    {
        public override string Name => "Emergency All-Input Patch";

        public override void Patch(Harmony harmony)
        {
            try
            {
                Info("🚨 EMERGENCY PATCH: Attempting to patch ALL input methods");
                
                // Get all loaded assemblies
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                int patchCount = 0;

                foreach (var assembly in assemblies)
                {
                    try
                    {
                        if (assembly.GetName().Name?.Contains("Stardew") == true ||
                            assembly.GetName().Name?.Contains("SMAPI") == true)
                        {
                            patchCount += PatchAllInputMethodsInAssembly(harmony, assembly);
                        }
                    }
                    catch (Exception ex)
                    {
                        Warn($"Error scanning assembly {assembly.GetName().Name}: {ex.Message}");
                    }
                }

                Info($"🚨 EMERGENCY PATCH: Applied {patchCount} total patches");
            }
            catch (Exception ex)
            {
                Error($"Emergency patch failed: {ex.Message}");
            }
        }

        private int PatchAllInputMethodsInAssembly(Harmony harmony, Assembly assembly)
        {
            int count = 0;
            
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    // Look for any type that might handle input
                    if (type.Name.ToLower().Contains("input") || 
                        type.Name.ToLower().Contains("keyboard") ||
                        type.Name.ToLower().Contains("button") ||
                        type.Name.ToLower().Contains("key"))
                    {
                        count += PatchInputMethodsInType(harmony, type);
                    }
                }
            }
            catch
            {
                // Ignore reflection errors
            }

            return count;
        }

        private int PatchInputMethodsInType(Harmony harmony, Type type)
        {
            int count = 0;

            try
            {
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                
                foreach (var method in methods)
                {
                    try
                    {
                        // Look for methods that take SButton and return bool
                        if (method.Name.ToLower().Contains("down") && 
                            method.GetParameters().Length == 1 &&
                            method.GetParameters()[0].ParameterType == typeof(SButton) &&
                            method.ReturnType == typeof(bool))
                        {
                            harmony.Patch(
                                original: method,
                                postfix: new HarmonyMethod(this.GetType(), nameof(IsDownPostfix))
                            );
                            
                            Info($"🎯 EMERGENCY: Patched {type.Name}.{method.Name}");
                            count++;
                        }
                        
                        // Also look for methods that might be called "IsPressed" or similar
                        else if ((method.Name.ToLower().Contains("press") || 
                                 method.Name.ToLower().Contains("held") ||
                                 method.Name.ToLower().Contains("active")) &&
                                method.GetParameters().Length == 1 &&
                                method.GetParameters()[0].ParameterType == typeof(SButton) &&
                                method.ReturnType == typeof(bool))
                        {
                            harmony.Patch(
                                original: method,
                                postfix: new HarmonyMethod(this.GetType(), nameof(IsDownPostfix))
                            );
                            
                            Info($"🎯 EMERGENCY: Patched {type.Name}.{method.Name}");
                            count++;
                        }
                    }
                    catch
                    {
                        // Ignore individual method patch failures
                    }
                }
            }
            catch
            {
                // Ignore type scanning errors
            }

            return count;
        }

        public static void IsDownPostfix(SButton button, ref bool __result)
        {
            if (KeybindManager.IsKeyHeld(button))
            {
                __result = true;
                Info($"🚨 EMERGENCY OVERRIDE: {button} is DOWN via emergency patch!");
            }
        }
    }

    /// <summary>
    /// Direct injection into Game1's update loop to force movement.
    /// This bypasses input checking entirely and directly moves the player.
    /// </summary>
    public class DirectMovementPatch : IPatch
    {
        public override string Name => "Direct Movement Injection";

        public override void Patch(Harmony harmony)
        {
            try
            {
                // Patch Game1.UpdateControlInput which handles player movement
                var updateControlInputMethod = AccessTools.Method(typeof(StardewValley.Game1), "UpdateControlInput", new Type[] { typeof(GameTime) });
                if (updateControlInputMethod != null)
                {
                    harmony.Patch(
                        original: updateControlInputMethod,
                        prefix: new HarmonyMethod(this.GetType(), nameof(UpdateControlInputPrefix))
                    );
                    Info("🎯 DIRECT: Patched Game1.UpdateControlInput");
                }

                // Also patch the player's update method
                var farmerUpdateMethod = AccessTools.Method(typeof(StardewValley.Farmer), "Update", new Type[] { typeof(GameTime), typeof(StardewValley.GameLocation) });
                if (farmerUpdateMethod != null)
                {
                    harmony.Patch(
                        original: farmerUpdateMethod,
                        prefix: new HarmonyMethod(this.GetType(), nameof(FarmerUpdatePrefix))
                    );
                    Info("🎯 DIRECT: Patched Farmer.Update");
                }
            }
            catch (Exception ex)
            {
                Error($"Direct movement patch failed: {ex.Message}");
            }
        }

        public static bool UpdateControlInputPrefix(GameTime time)
        {
            try
            {
                // Check for virtual movement keys and directly manipulate player velocity
                if (StardewValley.Game1.player != null)
                {
                    bool moved = false;
                    
                    if (KeybindManager.IsKeyHeld(SButton.W))
                    {
                        // Try to move the player up using velocity
                        StardewValley.Game1.player.xVelocity = 0;
                        StardewValley.Game1.player.yVelocity = -StardewValley.Game1.player.getMovementSpeed();
                        moved = true;
                        Info("🎯 DIRECT MOVEMENT: Moving UP (W key virtual)");
                    }
                    
                    if (KeybindManager.IsKeyHeld(SButton.S))
                    {
                        StardewValley.Game1.player.xVelocity = 0;
                        StardewValley.Game1.player.yVelocity = StardewValley.Game1.player.getMovementSpeed();
                        moved = true;
                        Info("🎯 DIRECT MOVEMENT: Moving DOWN (S key virtual)");
                    }
                    
                    if (KeybindManager.IsKeyHeld(SButton.A))
                    {
                        StardewValley.Game1.player.xVelocity = -StardewValley.Game1.player.getMovementSpeed();
                        StardewValley.Game1.player.yVelocity = 0;
                        moved = true;
                        Info("🎯 DIRECT MOVEMENT: Moving LEFT (A key virtual)");
                    }
                    
                    if (KeybindManager.IsKeyHeld(SButton.D))
                    {
                        StardewValley.Game1.player.xVelocity = StardewValley.Game1.player.getMovementSpeed();
                        StardewValley.Game1.player.yVelocity = 0;
                        moved = true;
                        Info("🎯 DIRECT MOVEMENT: Moving RIGHT (D key virtual)");
                    }

                    if (moved)
                    {
                        Info("🎯 DIRECT MOVEMENT: Virtual movement applied directly via velocity!");
                    }
                }
            }
            catch (Exception ex)
            {
                Error($"Direct movement error: {ex.Message}");
            }

            return true; // Continue with original method
        }

        public static bool FarmerUpdatePrefix(StardewValley.Farmer __instance, GameTime time, StardewValley.GameLocation location)
        {
            try
            {
                // Additional direct movement injection at the farmer level
                if (KeybindManager.HasActiveKeybinds)
                {
                    Info("🎯 DIRECT: Virtual keybinds active during Farmer.Update");
                }
            }
            catch (Exception ex)
            {
                Error($"Farmer update error: {ex.Message}");
            }

            return true; // Continue with original method
        }
    }
}
