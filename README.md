# Tilevania

A 2D platformer built with **Unity 6** and **C#**. Run, jump, climb ladders, shoot enemies, and collect coins across three levels to reach the finish door.

**[▶ Play in your browser](https://isasebib.itch.io/tilevania)**

<img width="1026" height="574" alt="tilevania" src="https://github.com/user-attachments/assets/6941da7e-a812-4656-9a3e-35126ac5b9a1" />

## Features

- **Three levels** with automatic progression and a victory screen at the end
- **Player controller** with running, jumping, ladder climbing, and shooting, built on the new Input System
- **Patrolling enemies** that turn around at platform edges
- **Hazards and water** that kill the player, with a death animation and level reload
- **Coin collection** with a live "x/y" counter per level and pickup sound effects
- **Cinemachine** state-driven camera that follows the player

## Controls

| Action | Key |
|--------|-----|
| Move | A / D or Arrow Keys |
| Jump | Space |
| Climb | W / S on ladders |
| Shoot | Left Mouse / Fire key |

## Level Design

Each level is built from separate **Tilemap layers** under one Grid, so every layer has its own collision and behavior:

| Tilemap | Purpose |
|---------|---------|
| Platforms | Solid ground and walls (Ground layer, Tilemap Collider 2D) |
| Background | Decorative tiles with no collision |
| Water | Trigger tiles that kill the player on contact |
| Ladder | Trigger tiles that enable climbing and disable gravity |
| Bouncing | Tiles using a bouncy Physics Material 2D |
| Hazards | Spikes and other tiles that kill on contact |

Levels get harder gradually, adding ladders, water, enemies, and hazards in later levels.

## Rule Tiles

Terrain uses **Rule Tiles**, so edges, corners, and inner fills are picked automatically while painting with the Tile Palette. This made it fast to draw large levels without placing each tile variation by hand.

## Prefabs

Reusable objects are **prefabs**, which keeps all levels consistent and makes changes easy:

- **Player**: movement, colliders, animator, and input in one prefab
- **Red Enemy**: patrolling enemy placed multiple times per level
- **Coin**: collectible instanced across levels
- **Bullet**: spawned at runtime with `Instantiate`

Editing a prefab updates every instance in every scene.

## Technical Notes

Problems I ran into while building the game and how I solved them:

- **Tunneling through tiles**: fast movement made the player sink into ground tiles. Setting Rigidbody2D **Collision Detection** to *Continuous* fixed it.
- **Preserving gravity while running**: setting velocity every frame overwrote gravity, so I only assign the X velocity and keep `linearVelocity.y`.
- **Layer index vs. layer mask**: `gameObject.layer` returns an index, while `LayerMask.GetMask` returns a bitmask. Comparing them directly never matches, so I use `LayerMask.NameToLayer` for comparisons.
- **Double trigger events**: the player has two colliders (capsule and box), so triggers could fire twice. Coins are disabled with `SetActive(false)` before counting, and the finish door uses a flag.
- **Destroy order between objects**: when a bullet and an enemy destroyed each other in the same frame, one could miss the collision. A short delay on `Destroy` solved it.
- **Delayed actions**: death reload uses `Invoke`, and level transitions and the victory screen use coroutines with `WaitForSecondsRealtime`.
- **Scene progression**: the next level loads via `buildIndex + 1`, wrapping back to the first scene after the last level.
- **Resolution-independent UI**: Canvas Scaler is set to *Scale With Screen Size*.
- **Null reference bugs**: components must be cached in `Start()` before use.

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/isasebib/Tilevania.git
   ```
2. Open the project with **Unity 6 (6000.6.5f1)** through Unity Hub.
3. Open `Assets/Scenes/lvl1` and press **Play**.

## Built With

- Unity 6
- C#
- Cinemachine
- Unity Input System
- TextMeshPro

## Author

**İsa** · [GitHub](https://github.com/isasebib)
