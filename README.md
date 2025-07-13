# VirtualKeyboard Mod - Console-Based Keybind Emulation

A powerful Stardew Valley mod that provides console-based keyboard input simulation and keybind automation. This mod has been completely refactored from its original visual interface to use a sophisticated console command system based on the TAS (Tool-Assisted Speedrun) mod patterns.

## 🚀 Features

### Core Functionality
- **Console-Based Control**: All functionality accessible through SMAPI console commands
- **Real-Time Key Simulation**: Simulate keyboard input that integrates seamlessly with the game
- **Advanced Timing Control**: Precise timing for key sequences, holds, and combinations
- **Macro System**: Record and playback complex key sequences
- **Zero Visual Overhead**: No on-screen UI elements, completely console-driven

### Key Simulation Capabilities
- Single key presses
- Timed key holds
- Key sequences with custom intervals
- Simultaneous key combinations (Ctrl+C, etc.)
- Repeating key patterns
- Conditional key releases

## 📋 Console Commands

All commands are available through the SMAPI console (press `~` in-game).

### Basic Commands

#### `keybind_press <key>`
Press a key once.
```
keybind_press W
keybind_press Space
keybind_press LeftControl
```

#### `keybind_hold <key> <duration_ms>`
Hold a key for a specific duration in milliseconds.
```
keybind_hold W 1000          # Hold W for 1 second
keybind_hold Space 500       # Hold Space for 0.5 seconds
```

#### `keybind_release <key>`
Release a currently held key.
```
keybind_release W
keybind_release LeftShift
```

### Advanced Commands

#### `keybind_sequence <key1,key2,key3> [interval_ms]`
Execute a sequence of key presses with optional interval (default: 100ms).
```
keybind_sequence W,A,S,D 200              # WASD sequence with 200ms intervals
keybind_sequence Q,W,E,R                  # Quick ability sequence
keybind_sequence LeftAlt,D,LeftAlt,D 300  # Farming pattern
```

#### `keybind_combo <key1+key2+key3> [duration_ms]`
Execute simultaneous key presses (combinations).
```
keybind_combo LeftControl+C               # Copy
keybind_combo LeftShift+A 100            # Shift+A for 100ms
keybind_combo LeftControl+LeftShift+S    # Save As
```

#### `keybind_repeat <key> <count> [interval_ms]`
Repeat a key press multiple times.
```
keybind_repeat Space 5 150    # Press Space 5 times with 150ms intervals
keybind_repeat E 10 100       # Press E 10 times quickly
```

### Utility Commands

#### `keybind_list [filter]`
List all available SButton keys, optionally filtered.
```
keybind_list                  # Show all keys
keybind_list Left             # Show keys containing "Left"
keybind_list Mouse            # Show mouse-related keys
```

#### `keybind_status`
Show current keybind status and held keys.
```
keybind_status
# Output: Held Keys: [W, LeftShift], Active Sequences: 2, Enabled: true
```

#### `keybind_clear`
Clear all active keybinds and release all held keys.
```
keybind_clear
```

#### `keybind_enable <true|false>`
Enable or disable the keybind system.
```
keybind_enable false    # Disable keybind simulation
keybind_enable true     # Re-enable keybind simulation
```

#### `keybind_help [command]`
Show help for keybind commands.
```
keybind_help                    # Show all commands
keybind_help keybind_sequence   # Show specific command help
```

## 🎮 Usage Examples

### Basic Movement
```bash
# Move forward for 2 seconds
keybind_hold W 2000

# Quick strafe left
keybind_hold A 500

# Move in a square pattern
keybind_sequence W,D,S,A 1000
```

### Farming Automation
```bash
# Use tool, move right, repeat
keybind_sequence LeftAlt,D,LeftAlt,D,LeftAlt,D 400

# Water crops in a line
keybind_repeat LeftAlt 10 300
```

### Combat Sequences
```bash
# Quick ability combo
keybind_sequence Q,W,E,R 150

# Block and attack
keybind_combo LeftShift+LeftAlt 200
```

### Inventory Management
```bash
# Select all and organize
keybind_combo LeftControl+A
keybind_press Tab

# Quick save
keybind_combo LeftControl+S
```

## 🛠️ Technical Details

### Architecture
- **Harmony Patches**: Intercepts and overrides keyboard input at multiple levels
- **KeyboardDispatcher Patches**: Suppresses normal input when virtual keys are active
- **SInputState Patches**: Integrates virtual keys with SMAPI's input system
- **Modular Command System**: Extensible console command architecture

### Key Components
1. **KeybindManager**: Central manager for all virtual key operations
2. **ConsoleCommandHandler**: Processes and executes console commands
3. **VirtualKeyboardState**: Manages XNA Framework keyboard state integration
4. **IPatch System**: Base class for all Harmony patches

### Performance
- Minimal performance impact when not in use
- Efficient key state tracking
- Optimized patch execution
- Memory-conscious sequence management

## ⚙️ Configuration

The mod includes a configuration file (`config.json`) with the following options:

```json
{
  "Enabled": true,
  "DefaultSequenceInterval": 100,
  "DefaultHoldDuration": 50,
  "LogKeybindOperations": true
}
```

### Configuration Options
- **Enabled**: Whether the keybind system is enabled by default
- **DefaultSequenceInterval**: Default time between sequence steps (ms)
- **DefaultHoldDuration**: Default duration for key holds (ms)
- **LogKeybindOperations**: Whether to log keybind operations for debugging

## 🧪 Testing

The mod includes comprehensive test suites:

```bash
# Run tests (requires test runner)
dotnet test
```

### Test Coverage
- Unit tests for all core functionality
- Integration tests for console commands
- Performance benchmarks
- Error handling validation

## 🔧 Development

### Building from Source
```bash
git clone <repository>
cd keyboard_emul/VirtualKeyboard
dotnet build
```

### Adding Custom Commands
1. Implement `IConsoleCommand` interface
2. Register in `ConsoleCommandHandler.Initialize()`
3. Add tests in `Tests/` directory

### Extending Functionality
The mod is designed to be extensible:
- Add new patch classes in `Patches/` directory
- Implement additional key simulation features
- Create custom macro systems

## 📝 Migration from Visual Version

If you were using the previous visual button version:

1. **Remove old configuration**: The visual button configuration is now obsolete
2. **Learn console commands**: All functionality is now console-based
3. **Update workflows**: Convert button sequences to console commands

### Command Migration Examples
- **Old**: Click virtual W button → **New**: `keybind_press W`
- **Old**: Hold virtual Space → **New**: `keybind_hold Space 1000`
- **Old**: Button sequence → **New**: `keybind_sequence W,A,S,D`

## 🐛 Troubleshooting

### Common Issues

**Keys not responding**
- Check if keybind system is enabled: `keybind_status`
- Verify key names with: `keybind_list`
- Clear stuck keys: `keybind_clear`

**Harmony patch failures**
- Check SMAPI log for patch errors
- Ensure no conflicting mods
- Restart game if patches fail to apply

**Performance issues**
- Disable logging: Set `LogKeybindOperations` to `false`
- Reduce sequence frequency
- Clear unused active sequences

### Debug Commands
```bash
keybind_status          # Check current state
keybind_enable false    # Temporarily disable
keybind_clear          # Reset all state
```

## 🔧 Recent Updates

### Warp Behavior Fix
**Issue**: Virtual keyboard input was stopping whenever the player warped to a new location.

**Root Cause**: The warp process calls `Farmer.Halt()` which sets `CanMove = false` and other movement-blocking properties, preventing virtual input simulation from working.

**Solution**: Added comprehensive Harmony patches that:
- Restore movement capabilities after `Farmer.Halt()` when virtual input is active
- Ensure movement properties are restored after warp completion  
- Handle all warp scenarios (farm totems, building transitions, mine levels, etc.)
- Use delayed actions to avoid timing conflicts with the warp process
- Only intervene when virtual input is active to preserve normal game behavior

**Files Modified**: `InputState_Patches.cs` - Added `Farmer_Halt_Patches`, `Farmer_WarpFarmer_Patches`, and `Farmer_OnWarp_Patches`

### Minimized Window Support  
The mod includes sophisticated patches to enable virtual input simulation even when Stardew Valley is minimized or doesn't have focus:
- Bypasses focus checks in `InputState.GetKeyboardState()`
- Forces `Game1.IsActiveNoOverlay` to return true when virtual input is active
- Maintains full functionality regardless of window state

## 🤝 Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Submit a pull request

### Development Guidelines
- Follow existing code patterns
- Add comprehensive tests
- Update documentation
- Maintain backwards compatibility where possible

## 📄 License

This mod is released under the MIT License. See LICENSE file for details.

## 🙏 Acknowledgments

- **SDVTASMod**: Inspiration for the patch-based architecture
- **SMAPI Community**: For the excellent modding framework
- **Stardew Valley**: For the amazing game that makes this possible

## 📞 Support

- **Issues**: Report bugs on GitHub Issues
- **Questions**: Use GitHub Discussions
- **Discord**: Join the Stardew Valley modding community

---

**Note**: This mod completely replaces the visual interface with a console-based system. The old visual button configuration is maintained for backwards compatibility but is no longer functional. All keybind operations now use console commands for maximum flexibility and power.
