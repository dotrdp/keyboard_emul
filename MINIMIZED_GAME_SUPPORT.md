# Windows Input Simulation - Minimized Game Support

## 🎯 New Feature: Works When Game is Minimized!

Your VirtualKeyboard mod now supports **low-level Windows input simulation** that works even when Stardew Valley is minimized or in the background!

## 🚀 How It Works

The mod now uses **dual input simulation**:

1. **SMAPI Input Patches** - Normal simulation when game is focused
2. **Windows API Input** - Low-level simulation that works when minimized

Both systems work together automatically - no configuration needed!

## 🎮 New Commands

### **Windows Input Control**
```bash
keybind_windows enable     # Enable Windows input (default: enabled)
keybind_windows disable    # Disable Windows input  
keybind_windows status     # Show Windows input status
keybind_windows reinit     # Reinitialize if window handle changes
```

## 🔧 Features

### **Automatic Window Detection**
- Finds Stardew Valley window automatically
- Works with different window titles:
  - "Stardew Valley"
  - "Stardew Valley v1.6" 
  - "Stardew Valley - SMAPI"
- Verifies correct process to avoid conflicts

### **Complete Key Support**
- ✅ **Movement**: W, A, S, D
- ✅ **Actions**: C, X, F, Y, N
- ✅ **Inventory**: 1-9, 0, -, +
- ✅ **Menus**: E, M, J, I, Tab, Escape
- ✅ **Modifiers**: Shift, Ctrl, Alt, Space, Enter

### **Dual Operation Mode**
- **Game Focused**: Uses SMAPI patches for perfect integration
- **Game Minimized**: Uses Windows API for background operation
- **Seamless**: Both systems work simultaneously

## 🎯 Testing Guide

### **1. Test with Game Focused**
```bash
move_up 2000              # Should work normally
keybind_press C           # Should work normally
keybind_windows status    # Check Windows input status
```

### **2. Test with Game Minimized**
1. **Minimize Stardew Valley** (Alt+Tab or minimize button)
2. **Run commands from VS Code console**:
   ```bash
   move_up 3000           # Should move character even when minimized!
   keybind_hold C 1000    # Should use action key even when minimized!
   keybind_sequence W,W,D,D 200  # Should work when minimized!
   ```

### **3. Verify Windows Input Status**
```bash
keybind_windows status
```

Expected output:
```
Windows Input Status:
• KeybindManager Windows Input: True
• Windows Input Simulator - Initialized: True, Window Handle: [handle], Enabled: True

This allows keybind simulation to work even when the game is minimized.
```

## 🔍 Technical Implementation

### **Windows API Integration**
- Uses `user32.dll` functions:
  - `FindWindow()` - Locate game window
  - `PostMessage()` - Send keyboard input
  - `GetWindowThreadProcessId()` - Verify correct process

### **Input Conversion**
- Converts SMAPI `SButton` → XNA `Keys` → Windows Virtual Key Codes
- Maintains timing and duration for held keys
- Supports async operations for timed inputs

### **Safety Features**  
- Process verification to avoid sending input to wrong windows
- Graceful fallback if Windows input fails
- Automatic reinitialization if needed

## ✅ Expected Results

### **Game Focused**
- Normal SMAPI input simulation works perfectly
- Windows input works as backup/supplement

### **Game Minimized** 
- Character should move when using movement commands
- Actions should execute when using action commands  
- Inventory slots should change when using number keys
- Menus should open/close when using menu keys

## 🎉 Complete Solution

Your VirtualKeyboard mod now provides:

1. ✅ **Visual Interface Removed**
2. ✅ **Console-Based Commands** 
3. ✅ **TASMod-Inspired Input Simulation**
4. ✅ **Comprehensive Key Support**
5. ✅ **Advanced Sequences & Combinations**
6. ✅ **Clean Console Output** 
7. 🆕 **Minimized Game Support** via Windows API

**Test it now by minimizing the game and running keybind commands!** 🎮
