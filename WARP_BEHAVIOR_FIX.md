# STICKY KEY FIX - WARP BEHAVIOR ANALYSIS

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
