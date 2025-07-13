# WARP BEHAVIOR FIX - VIRTUAL INPUT RESTORATION

## Problem
The virtual keyboard input was stopping whenever the player warped to a new location. This was happening because the warp process calls `Farmer.Halt()` which sets `CanMove = false` and other movement-blocking properties, preventing virtual input simulation from working.

## Root Cause Analysis
Based on investigation of the Stardew Valley decompiled source code, the warp process follows this sequence:

1. `Farmer.warpFarmer()` is called
2. `Farmer.Halt()` is immediately called before the warp
3. `Halt()` sets `CanMove = false`, clears movement directions, and may set freeze states
4. The warp occurs but the movement-blocking state persists
5. Virtual input simulation fails because `CanMove` is false

Key methods involved:
- `Farmer.warpFarmer(Warp w, int warp_collide_direction)` - calls `Halt()` before warping
- `Farmer.warpFarmer(Warp w)` - overload that also triggers halt
- `Farmer.Halt()` - sets movement-blocking properties
- `Farmer.OnWarp()` - called after warp completion

## Solution
Added three new Harmony patches to `InputState_Patches.cs`:

### 1. Farmer_Halt_Patches
**Purpose**: Restores movement capabilities immediately after `Halt()` when virtual input is active.

**Approach**: Uses a `[HarmonyPostfix]` on `Farmer.Halt()` to:
- Restore `CanMove = true`
- Clear `freezePause = 0`
- Ensure `Game1.freezeControls = false`

### 2. Farmer_WarpFarmer_Patches
**Purpose**: Ensures movement is restored after warp initiation.

**Approach**: Uses `[HarmonyPostfix]` on both `warpFarmer` overloads with a delayed action to:
- Restore movement properties after a 100ms delay
- Clear movement directions that might interfere
- Handle both warp method signatures

### 3. Farmer_OnWarp_Patches
**Purpose**: Final cleanup after warp completion to ensure stable virtual input.

**Approach**: Uses `[HarmonyPostfix]` on `Farmer.OnWarp()` with a 200ms delay to:
- Ensure all movement properties are restored
- Clear animation states that might block input
- Provide comprehensive state cleanup

## Implementation Details

```csharp
// Example of the Halt patch approach
[HarmonyPatch(typeof(Farmer), "Halt")]
[HarmonyPostfix]
public static void Halt_Postfix(Farmer __instance)
{
    if (VirtualInputSimulator.Active && __instance.IsLocalPlayer)
    {
        __instance.CanMove = true;
        __instance.freezePause = 0;
        Game1.freezeControls = false;
    }
}
```

## Key Benefits
1. **Non-intrusive**: Only activates when virtual input is active
2. **Targeted**: Only affects the local player
3. **Comprehensive**: Handles multiple warp scenarios and edge cases
4. **Reliable**: Uses delayed actions to ensure timing doesn't interfere with warp logic
5. **Safe**: Preserves normal game behavior when virtual input is not active

## Testing Scenarios
The fix should be tested with:
- Farm warps (using totems, cave entrance, etc.)
- Building entrances/exits
- Mine level transitions
- Cutscene warps
- Multiplayer warps
- Horse-mounted warps

## Technical Notes
- Uses `DelayedAction` to avoid timing conflicts with the warp process
- Checks `VirtualInputSimulator.Active` to only intervene when necessary
- Verifies `__instance.IsLocalPlayer` to avoid affecting other players in multiplayer
- Multiple patch points provide redundancy for different warp scenarios

## Related Files
- `VirtualKeyboard/InputState_Patches.cs` - Contains the new warp patches
- `VirtualKeyboard/Simulation/VirtualInputSimulator.cs` - Virtual input system
- Previous fixes in `InputState_Patches.cs` for minimized window scenarios

This fix complements the existing virtual input system by ensuring that warp events don't permanently disable virtual input functionality.

---

# PREVIOUS STICKY KEY FIX ANALYSIS (for reference)

## Root Cause Analysis

After analyzing the Stardew Valley decompiled source code, I discovered the exact mechanism that causes sticky keys and why warping clears them when minimized but not when focused.

### The Core Problem: InputState.GetKeyboardState() Focus Check

```cs
public virtual KeyboardState GetKeyboardState()
{
    if (!Game1.game1.IsMainInstance || !Game1.game1.HasKeyboardFocus())
    {
        return default(KeyboardState); // EMPTY STATE - No keys pressed
    }
    // ... rest handles cached keyboard state with potential sticky keys
}
```

### Why Warping Clears Sticky Keys When Minimized But NOT When Focused

1. **When window is minimized/unfocused**: 
   - `HasKeyboardFocus()` returns `false`
   - `GetKeyboardState()` returns empty `KeyboardState` 
   - This automatically clears all cached keyboard state
   - No sticky keys persist

2. **When window is focused**:
   - `HasKeyboardFocus()` returns `true`
   - `GetKeyboardState()` returns cached keyboard state
   - Cached state includes sticky virtual keys
   - Sticky keys persist through warp

## The Solution: Focus Loss Simulation

Our fix simulates the focus loss mechanism by adding a `ForceInputClearing` flag that:

1. Forces `InputState.GetKeyboardState()` to return empty state
2. Clears cached keyboard state using reflection
3. Automatically resets after one call

### Implementation Details

1. **VirtualInputSimulator.ForceInputClearing**: New static property that triggers forced clearing
2. **Enhanced InputState_Patches**: Checks for `ForceInputClearing` and simulates focus loss
3. **Updated ClearAllInputs()**: Sets `ForceInputClearing = true` when clearing input
4. **Auto-reset mechanism**: Flag resets after one clearing cycle

### How It Works

```cs
// When command stops, this happens:
VirtualInputSimulator.Instance.ClearAllInputs();
// -> Sets ForceInputClearing = true

// Next GetKeyboardState() call:
if (VirtualInputSimulator.ForceInputClearing)
{
    ClearCachedKeyboardState();        // Clear cached state
    __result = new KeyboardState();    // Return empty state
    VirtualInputSimulator.ForceInputClearing = false; // Reset flag
    return false;
}
```

## Why This Fix Works

1. **Mimics natural behavior**: Uses the exact same mechanism that already works during warping when minimized
2. **Clears cached state**: Eliminates the root cause (cached keyboard state with virtual keys)
3. **One-time operation**: Flag automatically resets to prevent interference
4. **Focus-independent**: Works regardless of window focus state

## Commands That Trigger Fix

- `stop_movement` - Now calls `ClearAllInputs()` 
- `stop_all` - Already called `ClearAllInputs()`
- `KeybindManager.ReleaseAllKeys()` - Already called `ClearAllInputs()`

## Testing

The fix should eliminate sticky keys in all scenarios:
- Window focused + command stopped
- Window minimized + command stopped  
- Focus changes during/after commands
- Multiple rapid start/stop cycles

This leverages the game's existing, proven input clearing mechanism rather than fighting against it.
