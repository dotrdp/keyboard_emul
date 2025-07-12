# VirtualKeyboard Mod - Console Command Demonstration Script

This file contains examples of all console commands available in the VirtualKeyboard mod.
These commands should be entered in the SMAPI console (press `~` in-game).

## ================================
## BASIC KEY SIMULATION COMMANDS
## ================================

# Press a single key once
keybind_press W
keybind_press Space
keybind_press E
keybind_press LeftAlt

# Hold a key for specified duration (in milliseconds)
keybind_hold W 1000          # Hold W for 1 second
keybind_hold Space 500       # Hold Space for 0.5 seconds
keybind_hold LeftShift 2000  # Hold Left Shift for 2 seconds

# Release a currently held key
keybind_release W
keybind_release LeftShift

## ================================
## MOVEMENT AND NAVIGATION
## ================================

# Basic movement sequence
keybind_sequence W,W,W 300   # Move forward 3 times with 300ms intervals
keybind_sequence A,S,D 200   # Strafe left, back, right

# Square movement pattern
keybind_sequence W,D,S,A 1000  # Move in a square (1 second per direction)

# Quick dodging
keybind_sequence A,A 100     # Quick double-tap left
keybind_sequence D,D 100     # Quick double-tap right

## ================================
## FARMING AND TOOL USAGE
## ================================

# Use tool sequence (watering can, hoe, etc.)
keybind_sequence LeftAlt,D,LeftAlt,D,LeftAlt,D 400  # Use tool, move right, repeat

# Harvesting crops in a line
keybind_repeat E 10 200      # Harvest 10 times with 200ms intervals

# Planting seeds sequence
keybind_sequence LeftAlt,E,LeftAlt,E,LeftAlt,E 300  # Use hoe, plant seed, repeat

# Fishing automation
keybind_hold LeftAlt 3000    # Cast and hold for 3 seconds
keybind_press LeftAlt        # Reel in

## ================================
## COMBAT AND ABILITIES
## ================================

# Quick ability combo
keybind_sequence Q,W,E,R 150  # Use abilities 1-4 quickly

# Block and attack combo
keybind_combo LeftShift+LeftAlt 200  # Block while attacking

# Defensive sequence
keybind_sequence LeftShift,A,LeftShift,D 300  # Block, dodge left, block, dodge right

## ================================
## INVENTORY AND INTERFACE
## ================================

# Open inventory and organize
keybind_press Tab
keybind_combo LeftControl+A  # Select all
keybind_press Tab            # Close inventory

# Quick save
keybind_combo LeftControl+S

# Copy and paste (useful for naming items)
keybind_combo LeftControl+A  # Select all
keybind_combo LeftControl+C  # Copy
keybind_combo LeftControl+V  # Paste

# Navigate through toolbar
keybind_sequence D1,D2,D3,D4,D5 100  # Select toolbar slots 1-5

## ================================
## ADVANCED COMBINATIONS
## ================================

# Mining sequence (pickaxe + movement)
keybind_sequence LeftAlt,S,LeftAlt,S,LeftAlt,S 500  # Mine down

# Building placement
keybind_combo LeftControl+LeftAlt  # Place structure with precision

# Multiple modifier combo
keybind_combo LeftControl+LeftShift+A  # Advanced selection

## ================================
## REPEATED ACTIONS
## ================================

# Spam click for fishing or combat
keybind_repeat LeftAlt 20 50  # Rapid 20 clicks with 50ms intervals

# Continuous movement
keybind_repeat W 10 100       # Move forward 10 times

# Resource gathering
keybind_repeat E 15 250       # Gather/interact 15 times

## ================================
## UTILITY AND STATUS COMMANDS
## ================================

# Check current keybind status
keybind_status

# List available keys
keybind_list
keybind_list Mouse    # Show only mouse-related keys
keybind_list Left     # Show keys containing "Left"
keybind_list Pad      # Show gamepad keys

# Clear all active keybinds
keybind_clear

# Enable/disable the system
keybind_enable false  # Disable keybind simulation
keybind_enable true   # Re-enable keybind simulation

# Get help
keybind_help
keybind_help keybind_sequence  # Get help for specific command

## ================================
## COMPLEX AUTOMATION SCENARIOS
## ================================

# Automated crop watering (zigzag pattern)
keybind_sequence LeftAlt,D,LeftAlt,D,LeftAlt,D,S,LeftAlt,A,LeftAlt,A,LeftAlt,A,S 400

# Automated fishing routine
keybind_hold LeftAlt 4000     # Cast line and wait
keybind_repeat LeftAlt 10 100 # Rapid clicks to reel in
keybind_hold W 1000           # Move to next spot

# Mass crafting sequence
keybind_repeat E 5 300        # Select recipe
keybind_repeat LeftAlt 20 150 # Craft 20 items

# Combat training dummy routine
keybind_sequence LeftAlt,A,LeftAlt,D,LeftAlt,S,LeftAlt,W 200  # Attack from all sides

## ================================
## DEBUGGING AND TESTING
## ================================

# Test basic functionality
keybind_direct_test W    # Direct test if SMAPI sees virtual W key
keybind_test W           # Test if virtual W key simulation works
keybind_press W          # Should move character forward
keybind_status           # Should show W as held
keybind_release W        # Should stop movement
keybind_status           # Should show no held keys

# Test different key types
keybind_test LeftAlt     # Test tool key
keybind_test Space       # Test jump key  
keybind_test MouseLeft   # Test mouse button

# Test timing
keybind_hold Space 1000  # Hold jump for exactly 1 second
keybind_status           # Should show Space as held
# Wait 1 second
keybind_status           # Should show no held keys

# Test sequences
keybind_sequence W,A,S,D 500  # Should move in square pattern
keybind_status                # Should show active sequence

# Test combinations
keybind_combo LeftControl+C   # Should copy (if applicable)
keybind_status                # Should show both keys held briefly

## ================================
## TROUBLESHOOTING COMMANDS
## ================================

# If keys seem stuck:
keybind_clear            # Clear all active keys

# If system seems unresponsive:
keybind_enable false     # Disable system
keybind_enable true      # Re-enable system

# If you need to see what's happening:
keybind_status           # Check current state
keybind_list             # Verify available keys

## ================================
## PERFORMANCE TESTING
## ================================

# Rapid fire test (use carefully)
keybind_repeat Space 50 10   # 50 rapid space presses

# Long sequence test
keybind_sequence W,A,S,D,W,A,S,D,W,A,S,D 1000  # Long movement pattern

# Multiple simultaneous operations
keybind_combo LeftControl+LeftShift+LeftAlt 100  # Triple modifier combo

## ================================
## NOTES AND BEST PRACTICES
## ================================

# 1. Always use keybind_status to check current state
# 2. Use keybind_clear if anything seems stuck
# 3. Start with longer intervals (500ms+) and reduce as needed
# 4. Test simple commands before complex sequences
# 5. Remember that some keys may not work in certain game contexts
# 6. Use keybind_enable false to quickly disable if needed

## ================================
## KEY REFERENCE
## ================================

# Common movement keys: W, A, S, D
# Common action keys: E (interact), LeftAlt (use tool), Space (jump)
# Common modifier keys: LeftControl, LeftShift, LeftAlt
# Mouse buttons: MouseLeft, MouseRight, MouseMiddle
# Number keys: D1, D2, D3, D4, D5, D6, D7, D8, D9, D0
# Function keys: F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12
# Arrow keys: Left, Right, Up, Down
# Special keys: Tab, Escape, Enter, Back (backspace)

## ================================
## END OF DEMONSTRATION SCRIPT
## ================================

# For more information, use:
keybind_help

# To see all available keys:
keybind_list
