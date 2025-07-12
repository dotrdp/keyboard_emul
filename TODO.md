# VirtualKeyboard Mod Refactoring TODO

## Project Overview
Refactor the VirtualKeyboard mod to remove visual interface components and replace with console-based keybind emulation using patterns from SDVTASMod.

## Core Infrastructure Changes

### ✅ Phase 1: Remove Visual Components
- [ ] Remove all visual rendering code from ModEntry.cs
- [ ] Remove KeyButton.cs entirely (visual button implementation)
- [ ] Remove visual-related fields from ModEntry.cs:
  - [ ] VirtualToggleButton
  - [ ] ToolbarAlignTop, ToolbarVertical, ToolbarItemSlotSize
  - [ ] VirtualToggleButtonBound
  - [ ] All rendering event handlers
- [ ] Clean up ModConfig.cs to remove visual-specific configurations

### ✅ Phase 2: Implement IPatch Base Class
- [ ] Create IPatch.cs abstract base class (based on TASMod pattern)
- [ ] Implement logging utilities (Trace, Warn, Alert, Error)

### ✅ Phase 3: Implement KeyboardDispatcher Patches
- [ ] Create KeyboardDispatcher patches directory
- [ ] Implement KeyboardDispatcher_ShouldSuppress patch
- [ ] Implement KeyboardDispatcher_Event_TextInput patch  
- [ ] Implement KeyboardDispatcher_EventInput_CharEntered patch
- [ ] Implement KeyboardDispatcher_EventInput_KeyDown patch
- [ ] Implement KeyboardDispatcher_Event_KeyDown patch

### ✅ Phase 4: Console Command System
- [ ] Create console command infrastructure
- [ ] Implement IConsoleCommand interface
- [ ] Create KeybindConsole class for managing keybind commands
- [ ] Implement command parsing and execution

### ✅ Phase 5: Key Simulation System
- [ ] Create TASInputState equivalent for keyboard state management
- [ ] Implement key press/release simulation
- [ ] Create KeybindManager for mapping console commands to key actions
- [ ] Implement SInputState patches for keyboard state override

### ✅ Phase 5: Error Resolution & Optimization  
- [x] ~~Resolve SMAPI assembly loading issues~~
- [x] ~~Fix test project dependencies to avoid xunit conflicts~~
- [x] ~~Resolve KeyboardState creation warnings (simplified approach)~~
- [ ] Optimize patch performance for high-frequency input
- [ ] Add error handling for invalid key sequences
- [ ] Implement graceful fallback when patches fail

### ✅ Phase 6: Console Commands Implementation
- [x] ~~**keybind_enable** - Enable virtual keybind system~~
- [ ] **keybind_press <key>** - Simulate single key press
- [ ] **keybind_hold <key> <duration>** - Hold key for specified duration (in frames/ms)
- [ ] **keybind_release <key>** - Release a held key
- [ ] **keybind_sequence <key1,key2,key3>** - Execute sequence of key presses
- [ ] **keybind_combo <key1+key2+key3>** - Execute simultaneous key combination
- [ ] **keybind_repeat <key> <count> <interval>** - Repeat key press multiple times
- [ ] **keybind_list** - List all available keybinds
- [ ] **keybind_status** - Show current held keys and active sequences
- [ ] **keybind_config_load <profile>** - Load keybind configuration profile
- [ ] **keybind_config_save <profile>** - Save current keybind configuration
- [ ] **keybind_macro_record <name>** - Start recording macro
- [ ] **keybind_macro_stop** - Stop recording macro
- [ ] **keybind_macro_play <name>** - Play recorded macro
- [ ] **keybind_macro_list** - List all available macros

### ✅ Phase 7: Advanced Features
- [ ] Implement macro recording and playback system
- [ ] Add timing-based key sequences
- [ ] Implement keybind profiles/configurations
- [ ] Add key state persistence across game sessions
- [ ] Implement conditional keybinds (context-aware)

### ✅ Phase 8: Testing Infrastructure
- [ ] Create unit tests for keybind simulation
- [ ] Create integration tests for console commands
- [ ] Test keyboard state override functionality
- [ ] Test macro recording/playback
- [ ] Performance testing for high-frequency key simulation

### ✅ Phase 9: Documentation & Examples
- [ ] Update README.md with new console-based usage
- [ ] Create examples for common keybind scenarios
- [ ] Document macro system usage
- [ ] Create troubleshooting guide
- [ ] Document integration with other mods

### ✅ Phase 10: Configuration Migration
- [ ] Create migration utility for existing visual configurations
- [ ] Convert existing button configurations to console commands
- [ ] Provide backwards compatibility layer if needed

## Technical Implementation Details

### Key Classes to Implement:
1. **IPatch** - Base class for Harmony patches
2. **KeyboardInputState** - Manages simulated keyboard state  
3. **KeybindManager** - Central manager for keybind operations
4. **ConsoleCommandHandler** - Processes console commands
5. **MacroSystem** - Records and plays back key sequences
6. **KeybindConfig** - Configuration management

### Harmony Patches Required:
1. **KeyboardDispatcher patches** - Intercept and override keyboard input
2. **SInputState patches** - Override SMAPI input state for simulated keys
3. **Game1 input patches** - Ensure compatibility with game input system

### Console Command Format:
```
keybind_<action> [parameters]
```

### Error Handling Strategy:
- Validate all key names against SButton enumeration
- Provide clear error messages for invalid commands
- Implement fallback mechanisms for patch failures
- Log all keybind operations for debugging

### Performance Considerations:
- Minimize patch overhead
- Efficient key state tracking
- Optimize macro playback timing
- Memory management for long-running sequences

## Testing Strategy

### Unit Tests:
- [ ] KeybindManager key simulation accuracy
- [ ] Console command parsing validation
- [ ] Macro recording/playback functionality
- [ ] Configuration serialization/deserialization

### Integration Tests:
- [ ] End-to-end keybind simulation in game context
- [ ] Console command execution through SMAPI console
- [ ] Compatibility with other input-related mods
- [ ] Performance under high-frequency key simulation

### Manual Testing Scenarios:
- [ ] Basic key press simulation (movement, actions)
- [ ] Complex key combinations (Ctrl+C, Shift+Click)
- [ ] Macro recording of farming sequences
- [ ] Profile switching between different keybind sets
- [ ] Error handling for invalid commands

## Known Challenges & Solutions

### Challenge 1: Timing Precision
**Problem:** Ensuring accurate timing for key sequences and macros
**Solution:** Use game tick-based timing instead of real-time for consistency

### Challenge 2: State Synchronization
**Problem:** Keeping simulated key state in sync with game expectations
**Solution:** Comprehensive SInputState patches and state validation

### Challenge 3: Mod Compatibility
**Problem:** Ensuring compatibility with other input-modifying mods
**Solution:** Careful patch ordering and optional integration hooks

### Challenge 4: Performance Impact
**Problem:** Harmony patches could impact game performance
**Solution:** Minimal patch overhead and optional disabling of unused features

## Future Enhancements

### Potential Features:
- [ ] **AI-Assisted Keybinding** - Machine learning for optimal keybind suggestions
- [ ] **Visual Macro Editor** - Optional GUI for complex macro creation
- [ ] **Network Synchronization** - Share keybinds in multiplayer
- [ ] **Context-Aware Bindings** - Different keybinds per game location/activity
- [ ] **Performance Analytics** - Track keybind usage and optimization
- [ ] **Integration APIs** - Allow other mods to register custom keybind commands

## Dependencies & Requirements

### Required Packages:
- Harmony (for patching)
- SMAPI (4.0.0+)
- .NET 6.0

### Optional Dependencies:
- Newtonsoft.Json (for configuration serialization)
- System.Linq (for advanced key filtering)

## Success Criteria

### Must Have:
- [ ] All visual components removed
- [ ] Console commands functional for basic key simulation
- [ ] Patches working without breaking game input
- [ ] Basic macro recording/playback

### Should Have:
- [ ] Complete test coverage
- [ ] Performance benchmarks
- [ ] Documentation with examples
- [ ] Error handling and validation

### Nice to Have:
- [ ] Advanced macro features
- [ ] Multiple configuration profiles
- [ ] Integration with other mods
- [ ] Performance optimization features

## ✅ RECENT UPDATES - July 12, 2025

### 🔧 KeyboardState Warning Resolution
- **Issue**: "Failed to create custom KeyboardState" warnings were appearing
- **Root Cause**: Complex reflection-based approach to KeyboardState creation was failing
- **Solution**: Simplified approach using multiple patch layers instead of KeyboardState manipulation
- **Status**: ✅ Resolved - No more warnings, cleaner architecture

### 🔧 Virtual Key Detection Issues  
- **Issue**: Virtual keys set via KeybindManager.PressKey() weren't being recognized by the game
- **Root Cause**: SMAPI uses complex input handling that wasn't being properly intercepted
- **Investigation**: Created comprehensive patch system targeting multiple input layers:
  1. **InputHelper_IsDown**: Patches SMAPI's IInputHelper.IsDown method
  2. **SInputState_IsDown**: Patches SMAPI's internal input state
  3. **SMAPI_InputHelper**: Comprehensive scanning and patching of all SMAPI input methods
  4. **Game1_InputIsDown**: Direct patching of Game1.input.IsDown
  5. **KeyboardState_IsKeyDown**: XNA Framework fallback patches
- **Debugging Tools**: Added `keybind_test` command for testing virtual key recognition
- **Status**: 🔄 Under Investigation - Multiple patch layers implemented

### 🛠️ Architecture Improvements
- **Separated Test Project**: Removed SMAPI dependencies from test project to avoid assembly conflicts
- **Enhanced Logging**: Added comprehensive tracing in all patch classes
- **Modular Patches**: Each input interception method is now in separate patch classes
- **Defensive Programming**: Added proper error handling and fallback mechanisms

### 🧪 Testing Infrastructure
- **keybind_test Command**: New debugging command to verify virtual key simulation
  ```
  keybind_test W    // Test if virtual W key press works
  ```
- **Status Monitoring**: Enhanced keybind_status command for debugging
- **Comprehensive Coverage**: Multiple patch points ensure we catch all input pathways

### 📚 Technical Notes
- **Harmony Patching**: Using postfix patches to override input method return values
- **SMAPI Integration**: Targeting both public and internal SMAPI input APIs
- **XNA Fallback**: Direct XNA Framework patches for games that bypass SMAPI
- **Reflection Safety**: All reflection operations wrapped in try-catch blocks

### 🎯 Next Steps for Debugging
1. Test the `keybind_test` command in-game to see which patches are working
2. Monitor SMAPI console for patch application success messages
3. Use `keybind_status` to verify KeybindManager state management
4. If still not working, may need to patch even deeper into SMAPI's input pipeline

## 🔧 **SPAM LOGGING FIXED & CRITICAL PATCHES ADDED** 

### Issues Resolved:
1. **Logging Spam**: The console spam of "Virtual keybinds active during SMAPI GetState call" has been fixed by adding time-based throttling (only logs once per second)
2. **Critical Input Patches**: Added `CriticalInputPatch` that specifically targets SMAPI's `InputHelper.IsDown` method - this is the most likely method that needs to be patched for virtual keys to work
3. **Player Movement Patches**: Added `PlayerMovementPatch` to ensure virtual movement keys are properly recognized by the player movement system

### New Debugging Commands:
- **`keybind_direct_test W`**: Tests if SMAPI's input helper directly recognizes virtual keys
  - This will show you exactly where the virtual key detection is failing
  - Checks KeybindManager, SMAPI Helper.Input.IsDown, and Game1.input separately

### Testing Steps:
1. Try `keybind_direct_test W` to see if SMAPI sees the virtual key
2. If SMAPI sees it but movement doesn't work, the issue is in movement processing
3. If SMAPI doesn't see it, we need to patch different input methods

### Architecture Changes:
- **Time-throttled logging**: Reduced spam from every frame to once per second maximum
- **Critical patch priority**: `CriticalInputPatch` and `PlayerMovementPatch` are now applied first
- **Direct SMAPI testing**: `keybind_direct_test` directly accesses SMAPI's input helper to verify patch effectiveness

---
