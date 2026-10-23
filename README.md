# Apradh 3D Trial

A Unity 6 prototype for **Apradh**: third-person combat inside grounded Indian locations, with a separate campaign-map layer for moving units across terrain.

This repository is a technical trial of character control, weapons, NavMesh AI (patrol, engage, cover, peek), and location blockouts — not a shipped game.

**Engine:** Unity 6000.0.26f1 · **Pipeline:** URP 17 · **Input:** Unity Input System (also compatible with the legacy Input Manager)

---

## Features

- Third-person shooter with Cinemachine follow camera, ADS, recoil, and movement-based accuracy (bloom)
- Shared character model: player and NPCs use the same actor (`Npc`) for health, aiming, shooting, and animation
- Enemy AI on Unity NavMesh: team-aware vision, last-known-position memory, follow/flank, cover splines, corner peeks, burst fire
- ScriptableObject weapons: hitscan or projectile, semi/auto fire, magazine reload driven by animation events
- Location interiors (NGO office, dhaba, club, factory, coaching institute, film set, desert camp, akhada, chaupal)
- Campaign map: RTS-style camera, click-to-select units and buildings, right-click move with grid snap

---

## Getting started

1. Install **Unity 6000.0.26f1** and open this folder as a project.
2. Wait for the editor to import packages and rebuild the Library if needed.
3. Open a scene under `Assets/Scenes/` and press Play.

**Combat:** open a location scene such as `NgoScene`. The cursor is locked for shooter controls.

**Campaign map:** open `MapScene` or `Map2Scene`.

The player build list currently includes only `SampleScene` (URP template). Play content from the editor until other scenes are added in Build Settings.

### Combat controls

| Action | Input |
| --- | --- |
| Move | WASD |
| Sprint | Shift |
| Look | Mouse |
| Aim | Right mouse |
| Fire | Left mouse |
| Reload | R |

### Campaign controls

| Action | Input |
| --- | --- |
| Pan camera | WASD or screen-edge mouse |
| Fast pan | Shift |
| Orbit | Middle mouse + drag |
| Zoom | Scroll wheel |
| Select unit / building | Left click |
| Move / approach | Right click |

---

## Architecture

Player and AI both sit on an **`Npc`**: a small event hub (move, look, shoot, reload, death) plus animator and bloom. Controllers only write into that hub.

```
Npc
├── PlayerController     CharacterController + Cinemachine third-person camera
├── AI agent             NavMeshAgent + behaviour / weapon state machines
├── Weapon               FireMode + Shooter (ScriptableObjects)
├── NpcHealth            IDamageable → ragdoll on death
└── NpcAnimator          locomotion, posture, aim, gun layer
```

**AI loop:** sensor (FOV + occlusion) → team filter → scored memory → Find / Engage (follow or flank) / Cover (claim a wall spline, hide, peek) / Investigate last known position. Shooting uses the same `Weapon` path as the player, with a burst director for cadence and first-shot inaccuracy.

**Weapons:** `FireMode` (semi / auto, ammo, rate) decides *when* to fire; `Shooter` decides *how* (raycast or spawned bullet). Stats live in `Assets/Fight/GunSO/`.

Input maps: `Player` for combat, `MapPlayer` for the campaign camera (`Assets/Fight/PlayerInputActions.inputactions`). Campaign selection still uses mouse raycasts on layers `Units`, `Buildings`, and `Terrain`.

---

## Scenes

| Scene | Role |
| --- | --- |
| `NgoScene` | NGO / institutional interior (typical combat test scene) |
| `DhabaScene` | Roadside dhaba |
| `ClubScene` | Nightclub |
| `FactoryScene` | Warehouse |
| `CoachingScene` | Coaching institute |
| `FilmScene` | Film location |
| `DesertCampScene` | Outdoor camp |
| `AkhadaScene` | Wrestling ground |
| `ChaupalScene` | Village gathering space |
| `MapScene` / `Map2Scene` | Campaign overworld |
| `SampleScene` | URP template scene (build settings default) |

Floor-plan references for interiors are in `Assets/Plan/`. Several scenes include baked NavMesh data.

---

## Repository layout

```
Assets/
  Fight/           Combat: characters, AI, weapons, animation, prefabs
  Scenes/          Playable levels
  Plan/            Location floor plans
  RTSCamera/       Campaign camera
  Settings/        URP (PC and mobile)
  ClothModels/, Hairstyles/, Skin/   Character art
  dhaba/           Dhaba environment
CampaignController.cs    Map selection and move orders
```

Core gameplay code is under `Assets/Fight/` (especially `NpcCommon`, `AiCommon`, `AiStates`, `Gun`, `Health`).  
`Assets/Pathfinding` and `Assets/_` are third-party Code Monkey utilities, not the live NavMesh combat path.

---

## Tech stack

| Area | Choice |
| --- | --- |
| Engine | Unity 6 (6000.0.26f1) |
| Rendering | Universal Render Pipeline 17, PC and mobile quality |
| Camera | Cinemachine 3 (third-person follow, orbital map camera) |
| AI / map movement | AI Navigation 2 (NavMesh) |
| Animation | Animator + Animation Rigging |
| Input | Input System 1.11 |
| Level tools | ProBuilder, Terrain Tools |
| UI | uGUI / TextMesh Pro |

Third-party art: DevAssets modern weapons, locomotion animation pack. There is no networked multiplayer; `NgoScene` is an NGO office, not Netcode.

---

## Project status

This is an **R&D prototype**. Combat, AI, weapons, and location blockouts are the focus. Campaign map and interiors are separate scenes (no “enter location from the map” flow yet). Some earlier FPS tutorial scripts remain in `Fight` beside the current `Npc` pipeline; new work should follow the `Npc` / ScriptableObject weapon path.

Product version in project settings: `0.1.0`.
