# Snowventure Demo

Snowventure is a top-down 2D action-adventure prototype built with Unity. Play as a snowman, explore a winter-themed map, fight enemies with melee and ranged attacks, collect rewards, and unlock upgrades.

> This repository is an early learning project. It is shared as a portfolio and archive project and may still contain prototype-quality systems.

## Demo

Curious how it plays? [Watch the gameplay video](https://youtu.be/r8eVF52w0Yo) to see the snowman dash, punch, and throw snowballs through the winter map.

## Features

- Top-down movement with keyboard and gamepad support
- Dash, punch, and snowball combat
- Two ultimate abilities with an attack-energy system
- Enemy encounters, hazards, and room-clear progression
- Chests, keys, doors, coins, and interactive upgrade statues
- Health, attack, pause, and in-game menu UI
- 2D lighting and rendering with the Universal Render Pipeline

## Controls

| Action | Keyboard and mouse | Gamepad |
| --- | --- | --- |
| Move | `WASD` or arrow keys | Left stick or D-pad |
| Dash | Left Shift | Right shoulder |
| Punch | Left mouse button | North button |
| Throw snowball | Right mouse button | West button |
| Spread ultimate | `Q` | Left shoulder |
| Heavy ultimate | `E` | Right shoulder |
| Interact | `F` | South button |
| In-game menu | Tab | Not mapped |

Some prototype chests use `R` instead of `F`.

## Requirements

- [Unity Hub](https://unity.com/download)
- Unity `6000.3.9f1`

Using the exact editor version is recommended because Unity projects can be changed during automatic upgrades.

## Getting Started

1. Clone the repository:

   ```bash
   git clone https://github.com/pclelif/snowenture-demo.git
   ```

2. Open Unity Hub and select **Add > Add project from disk**.
3. Choose the cloned repository directory.
4. Open the project with Unity `6000.3.9f1` and wait for package import to finish.
5. Open `Assets/Scenes/Outside.unity` and press **Play**.

The playable scenes included in Build Settings are:

- `Assets/Scenes/Outside.unity`
- `Assets/Scenes/Home.unity`

## Project Structure

```text
Assets/
├── Scenes/              # Playable Unity scenes
├── Scripts/
│   ├── Enemies/         # Enemy AI, states, health, and attacks
│   ├── Interactives/    # Doors, chests, keys, spawners, and upgrades
│   ├── Player/          # Movement, combat, projectiles, and health
│   └── Ui/              # HUD and menu logic
├── TileMap/             # Tile assets and prefabs
└── BayatGames/          # Third-party game art assets
Packages/                # Unity Package Manager configuration
ProjectSettings/         # Project-wide Unity settings
```

## Main Technologies

- Unity 6
- C#
- Universal Render Pipeline 2D
- Unity Input System
- Cinemachine
- Unity Tilemap
- TextMesh Pro

## Known Limitations

- This is a demo and prototype rather than a finished release.
- Interaction keys are not consistent across all chests.
- Automated tests and distributable builds are not currently included.

## Third-Party Assets and License

This project contains third-party assets, including **Free Platform Game Assets** by Bayat Games and assets distributed with TextMesh Pro. Their original license and attribution files, where provided, remain inside the related asset directories.

No license is currently granted for the original source code or project-specific artwork in this repository. Before redistributing the project or publishing a commercial build, review the license terms of every third-party asset.

## Contributing

This repository primarily documents an early learning project, but bug reports and focused improvements are welcome through GitHub Issues and pull requests.
