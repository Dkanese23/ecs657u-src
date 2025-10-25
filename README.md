# Drawn to Battle

A turn-based card combat RPG where strategic deck building meets tactical battle encounters.

## Table of Contents
- [Game Overview](#game-overview)
- [How to Play](#how-to-play)
- [Controls](#controls)
- [Combat System](#combat-system)
- [Deck Management](#deck-management)
- [Installation](#installation)
- [Credits](#credits)

---

## Game Overview

**Drawn to Battle** is a fantasy RPG that combines exploration with strategic card-based combat. Navigate the overworld, collect clues, manage your inventory, and engage in tactical turn-based battles using a customizable deck of cards.

---

## How to Play

### Exploration
- **Move through the map** using WASD or arrow keys
- **Collect clues and items** by approaching them and pressing **E**
- **Interact with objects** in the environment using **E**
- **Open your inventory** at any time by pressing **I** to:
  - View collected items
  - Manage your card deck

### Entering Combat
When you encounter an enemy on the overworld map, you'll automatically enter a battle scene where you'll face off using your deck of cards.

---

## Controls

### Overworld
| Key | Action |
|-----|--------|
| **W/A/S/D** or **Arrow Keys** | Move character |
| **Mouse** | Look around |
| **E** | Interact / Collect items |
| **I** | Open/Close Inventory |

### Battle
| Input | Action |
|-------|--------|
| **Left Click** | Play selected card |
| **Draw & Skip Button** | Draw one card and end your character's turn |

---

## Combat System

### Turn Order
Combat follows a **turn-based system** with the following flow:

1. **Character A's Turn** → Play a card OR draw and skip
2. **Character B's Turn** → Play a card OR draw and skip  
3. **Character C's Turn** → Play a card OR draw and skip
4. **Enemy's Turn** → Enemy attacks automatically
5. Repeat from Character A

### Your Turn Options

**Option 1: Play a Card**
- Click any card in your hand to play it
- The card effect activates immediately
- Your turn ends and moves to the next character

**Option 2: Draw & Skip**
- Click the "Draw & Skip" button
- Draw one card from your deck into your hand
- Your turn ends immediately

### Card Types

**Physical Cards**
- **Attack**: Deal damage based on your Strength stat
- **Taunt**: Force the enemy to target you next turn

**Magic Cards**
- **Magic Bolt**: Deal damage based on your Intelligence stat

**Support Cards**
- **Guard**: Increase defense for 2 turns
- **Heal**: Restore HP to yourself or an ally
- **Block**: Add a shield to absorb incoming damage

### Combat Mechanics

- **Hand Management**: You start each battle with 5 cards
- **Deck Cycling**: When your draw pile is empty, your discard pile is shuffled back in
- **Enemy Targeting**: Enemies prioritize:
  1. Characters using Taunt
  2. The character with the lowest HP
- **Victory**: Defeat the enemy by reducing their HP to 0
- **Defeat**: If all party members reach 0 HP, you lose the battle

### Strategic Tips
- **Balance offense and defense**: Don't just attack - use Guard and Block to survive
- **Manage your hand**: Sometimes drawing cards is better than playing a weak card
- **Use Taunt wisely**: Protect low-HP allies by forcing enemies to attack your tank
- **Pay attention to turn order**: Plan ahead for which character needs healing or buffs

---

## Deck Management

### Accessing Your Deck
Press **I** in the overworld to open your inventory and access the deck management screen.

### Building Your Deck
- View all available cards
- Add or remove cards from your battle deck
- Each card you add to your deck will be shuffled in at the start of combat
- **Note**: You can have multiple copies of the same card in your deck

### Card Strategy
- **Balanced decks** work well for most encounters
- **Specialized decks** can be powerful but risky
- Consider your party composition when building your deck

---

## Installation

### Playing the Game
Visit: https://dkanese23.github.io/ecs657u-build/v0.2/index.html

### System Requirements
- **OS**: Windows 10 or later
- **Processor**: Intel Core i3 or equivalent
- **Memory**: 4 GB RAM
- **Graphics**: DirectX 11 compatible graphics card
- **Storage**: 500 MB available space

---

## Credits

### Development Team
This game was created as part of the ECS657U Game Development module at Queen Mary University of London.

**Programming**: Dylan Kane, Milans Klein, Layan Alassaf, Chizara Udeh  
**Design**: Dylan Kane, Milans Klein, Layan Alassaf, Chizara Udeh  


### Tools & Technologies
- **Unity Engine** (Version [2023.2.20f1])
- **C# Scripting**
- **Git/GitHub** for version control

### External Assets
BrokenVector

---

## Known Issues
- 

## Future Updates
- Additional card types and mechanics
- More enemy varieties with unique abilities
- Enhanced visual effects and animations
- Additional levels and story content

---

**Thank you for playing Drawn to Battle!**
