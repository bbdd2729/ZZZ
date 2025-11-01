# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity game project (version 2022.3.44f1c1) featuring a character action game with a sophisticated Finite State Machine (FSM) system for character control.

## Key Architecture

### FSM System (Finite State Machine)
The project uses a comprehensive FSM pattern for character behavior management:

**Core Components:**
- `Assets/Scripts/Controller/FSM/IState.cs` - Base state interface defining EnterState, UpdateState, ExitState, and condition checking
- `Assets/Scripts/Controller/FSM/BaseState.cs` - Abstract base class implementing IState with common functionality
- `Assets/Scripts/Controller/FSM/StateMachine.cs` - Manages state transitions and current state execution

**Character States:** Located in `Assets/Scripts/Controller/FSM/CharacterState/`:
- `IdleState.cs` - Default idle behavior
- `WalkState.cs` & `RunState.cs` - Movement states
- `AttackState.cs` & `AttackEndState.cs` - Basic attack chain
- `BigSkillState.cs` - Special ability state
- `EvadeState.cs`, `EvadeEndState.cs`, `EvadeBackState.cs`, `EvadeBackEndState.cs` - Dodge system
- `SwitchInState.cs` - Character switching system

### Update Management System
Centralized update system using UpdateManager pattern:
- `Assets/Scripts/Tool/UpdateManager/IUpdateManager.cs` - Interface for managed updates
- `Assets/Scripts/Tool/UpdateManager/UpdataManager.cs` - Singleton manager handling MonoBehaviour updates
- Objects register for Update/FixedUpdate calls through this system

### Controller Architecture
- `PlayerController.cs` - Main player input handling and character switching
- Uses R3 (Reactive Extensions) for event handling
- Manages health, energy, and character activation/deactivation
- State machine controls character behavior with states like SwitchOutState, SkillState, JumpState, etc.

## Development Commands

### Unity Development
```bash
# Open Unity project with specific version
"C:\Program Files\Unity\Hub\Editor\2022.3.44f1c1\Editor\Unity.exe" -projectPath E:\Unity\ZZZ

# Build for different platforms (via Unity Editor)
# File -> Build Settings -> Select platform -> Build
```

### Package Management
Unity uses Package Manager - dependencies defined in:
- `Packages/manifest.json` - Main package dependencies
- `Packages/packages-lock.json` - Locked versions

Key packages:
- R3 (Reactive Extensions) for reactive programming
- ZLinq for LINQ operations

## Code Patterns

### State Implementation Pattern
When creating new character states:
1. Inherit from `BaseState` or implement `IState`
2. Override `EnterState()`, `UpdateState()`, `ExitState()`
3. Implement condition checking logic
4. Register state transitions in appropriate controller

### Update Registration Pattern
```csharp
// Register for updates
UpdateManager.Instance.AddUpdate(this);
// or
UpdateManager.Instance.AddFixedUpdate(this);

// Don't forget to remove on destroy
UpdateManager.Instance.RemoveUpdate(this);
```

### Event System Pattern
Uses R3 for reactive event handling. Example from PlayerController:
```csharp
public Subject<PlayerEvent> OnPlayerEvent = new Subject<PlayerEvent>();
// Subscribe to events
GameEvents.OnPlayerSwitched.Subscribe(OnPlayerSwitched);
```

## Important Files to Reference

- Character state logic: `Assets/Scripts/Controller/FSM/CharacterState/`
- Core FSM system: `Assets/Scripts/Controller/FSM/StateMachine.cs`
- Player control: `Assets/Scripts/Controller/PlayerController.cs`
- Update management: `Assets/Scripts/Tool/UpdateManager/`
- Recent git activity shows focus on character switching and attack combos

## Notes

- Recent commits show work on character switching (SwitchInState), ultimate abilities ("大招"), and attack combos ("普攻连招")
- The PlayerController registers 12+ different states including combat, movement, and switching states
- Uses reactive programming patterns with R3 library for event handling