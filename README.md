# Tile Flip Engine

A card-matching memory game built from scratch in **Unity 2021 LTS**. Players flip cards to find matching pairs across multiple grid layouts, with combo scoring, save/load persistence, and smooth animations.

---

## Gameplay

- Flip two cards at a time to find matching pairs
- **Continuous flipping** — no need to wait for card comparisons before selecting more cards
- Match all pairs to complete the level
- Earn higher scores with consecutive matches through the **combo multiplier system**
- Progress is **auto-saved** and restored between sessions

---

## Features

### Core Mechanics
- Card flip animation with smooth eased rotation
- Continuous card flipping without input blocking
- Match and mismatch detection with visual/audio feedback
- Dynamic grid generation supporting various layouts (2x2, 2x3, 3x4, 4x4, 4x5, 5x6)
- Cards auto-scale to fit the display area with correct aspect ratio

### Scoring System
- Base score per match: **100 points**
- Combo multiplier: consecutive matches increase the multiplier by **0.5x** per streak
- Mismatch penalty: **-10 points** (score cannot go below zero)
- Combo resets on mismatch
- Tracks total moves and best combo

| Match Streak | Multiplier | Points Earned |
|:---:|:---:|:---:|
| 1st | 1.0x | 100 |
| 2nd | 1.5x | 150 |
| 3rd | 2.0x | 200 |
| 4th | 2.5x | 250 |
| ... | ... | ... |

### Save/Load System
- Auto-saves after every match
- Auto-saves every 30 seconds during gameplay
- Saves on app pause and quit
- Restores grid layout, card positions, matched state, and score on reload
- Save is deleted upon game completion

### Sound Effects
- Card flip
- Match (correct pair)
- Mismatch (wrong pair)
- Game over (all pairs matched)

### Visual Feedback
- Scale punch effect on matched cards
- Shake effect on mismatched cards
- Color highlight tint on successful match
- Combo text display with scale animation

### Level Selection
- Menu system with multiple difficulty levels
- Continue option to resume saved progress
- Restart and return-to-menu from game over screen

### Optimization
- Object pooling for card instantiation/recycling
- Singleton pattern for manager classes
- Efficient event-driven architecture (no Update polling for game logic)

---

## Project Structure

```
Assets/
├── Audio/                     # Sound effect clips
│   ├── sfx_card_flip
│   ├── sfx_match
│   ├── sfx_mismatch
│   └── sfx_game_over
├── Materials/
├── Prefabs/
│   ├── Card.prefab            # Card prefab with front/back renderers
│   └── LevelButton.prefab    # UI button for level selection
├── Scenes/
│   └── GameScene.unity
├── ScriptableObjects/
│   ├── CardData/              # Card face data (id + sprite)
│   └── LevelData/             # Level configs (rows x columns)
├── Scripts/
│   ├── Card/
│   │   └── Card.cs            # Card behavior, flip animation, visual effects
│   ├── Data/
│   │   ├── CardData.cs        # ScriptableObject for card face data
│   │   ├── LevelData.cs       # ScriptableObject for level configuration
│   │   └── GameSaveData.cs    # Serializable save data models
│   ├── Managers/
│   │   ├── GameManager.cs     # Core game loop, matching logic
│   │   ├── GridManager.cs     # Grid generation, card layout, auto-scaling
│   │   ├── ScoreManager.cs    # Scoring, combo multiplier
│   │   ├── AudioManager.cs    # Sound effect playback
│   │   ├── SaveManager.cs     # Save/load with PlayerPrefs
│   │   └── LevelManager.cs    # Level selection and switching
│   ├── UI/
│   │   ├── ScoreUI.cs         # Score, moves, combo display
│   │   ├── GameOverUI.cs      # Game over panel with stats
│   │   └── MenuUI.cs          # Level select menu
│   └── Utils/
│       ├── CardPool.cs        # Object pooling for cards
│       ├── InputHandler.cs    # Unified mouse + touch input
│       ├── ScreenScaler.cs    # Camera auto-scaling for screen sizes
│       ├── SafeAreaHandler.cs # Notch/safe area support
│       ├── PerformanceManager.cs  # FPS and quality settings
│       └── BuildConfig.cs     # Platform-specific configuration
└── Sprites/                   # Card face and back sprites
```

---

## Architecture

The project uses an **event-driven architecture** with decoupled managers:

```
Card (OnCardClicked event)
  └──> GameManager (handles matching logic)
         ├──> OnCardsMatched event
         │      ├──> ScoreManager (updates score/combo)
         │      ├──> AudioManager (plays match sound)
         │      └──> SaveManager (auto-save)
         ├──> OnCardsMismatched event
         │      ├──> ScoreManager (applies penalty, resets combo)
         │      └──> AudioManager (plays mismatch sound)
         ├──> OnCardFlipped event
         │      └──> AudioManager (plays flip sound)
         └──> OnGameOver event
                ├──> AudioManager (plays game over sound)
                ├──> GameOverUI (shows results)
                └──> SaveManager (deletes save)

ScoreManager
  ├──> OnScoreChanged ──> ScoreUI
  ├──> OnComboChanged ──> ScoreUI
  └──> OnMoveMade ──> ScoreUI
```

### Key Design Decisions

- **Static events on Card and GameManager** — allows any system to subscribe without direct references
- **ScriptableObjects for data** — CardData and LevelData are easily configurable in the editor
- **Object pooling** — cards are recycled rather than instantiated/destroyed when switching levels
- **Singleton AudioManager and SaveManager** — single points of access for global services
- **Continuous flipping via queue system** — when a third card is clicked before mismatch resolves, unmatched cards flip down immediately

---

## Supported Platforms

| Platform | Status |
|:---:|:---:|
| Windows | ✅ |
| macOS | ✅ |
| Linux | ✅ |
| Android | ✅ |

**Orientation:** Landscape only

---

## Requirements

- **Unity 2021 LTS** (2021.3.x)
- **TextMeshPro** (included with Unity)
- **Universal Render Pipeline (URP)** — 2D template

---

## Setup

1. Clone the repository
2. Open the project in Unity 2021 LTS
3. Open `Assets/Scenes/GameScene.unity`
4. Press Play

### Creating Card Data
1. Right-click in Project → Create → Card Match → Card Data
2. Assign a unique `cardId` and `cardFace` sprite
3. Add to the `GridManager` card data array

### Creating Levels
1. Right-click in Project → Create → Card Match → Level Data
2. Set `levelName`, `rows`, and `columns`
3. Add to the `LevelManager` levels array

---

## Build Instructions

### Desktop
1. File → Build Settings → PC, Mac & Linux Standalone
2. Set Resolution: 1920 x 1080
3. Build and Run

### Android
1. File → Build Settings → Switch to Android
2. Player Settings:
   - Minimum API Level: Android 7.0 (API 24)
   - Scripting Backend: IL2CPP
   - Target Architecture: ARM64
   - Default Orientation: Landscape Left
3. Build and Run

---

## License

This project is developed as a prototype demonstration. All assets used are either custom-made or sourced from free/open asset collections.
