# Weapon System Setup Guide

This guide explains how to set up the weapon system for your roguelike game.

## Overview

The weapon system consists of:
- **WeaponItem**: ScriptableObject that defines weapon properties (damage, fire rate, ammo, etc.)
- **WeaponController**: Handles shooting, weapon switching, and ammo management
- **WeaponHUD**: Displays current weapon and ammo in the HUD
- **WeaponPickup**: Component for picking up weapons in the world
- **Projectile**: Component for projectiles that deal damage

## Step-by-Step Setup

### 1. Create a Weapon ScriptableObject

1. In Unity, right-click in the Project window
2. Go to `Create > Items > Weapon`
3. Name it (e.g., "Rifle")
4. Configure the weapon:
   - **Display Name**: Name shown in UI (e.g., "Rifle")
   - **Icon**: Sprite for inventory
   - **Weapon Sprite**: Sprite shown in HUD when equipped
   - **Damage**: Base damage per shot
   - **Fire Rate**: Shots per second
   - **Max Ammo**: Maximum ammo capacity
   - **Starting Ammo**: Ammo when first picked up
   - **Projectile Prefab**: Prefab to spawn when shooting (create this next)
   - **Projectile Speed**: How fast projectiles travel
   - **Projectile Lifetime**: How long projectiles exist
   - **Uses Ammo**: Whether weapon consumes ammo
   - **Infinite Ammo**: Whether weapon has unlimited ammo

### 2. Create a Projectile Prefab

1. Create a new GameObject (e.g., "Bullet")
2. Add components:
   - **SpriteRenderer**: For visual representation
   - **Rigidbody2D**: For physics movement
   - **CircleCollider2D** (or BoxCollider2D): Set as Trigger
   - **Projectile** script (already created)
3. Set the Rigidbody2D:
   - **Gravity Scale**: 0 (unless you want gravity)
   - **Collision Detection**: Continuous (recommended)
4. Set the layer to something appropriate (e.g., "Projectile")
5. Save as a prefab

### 3. Set Up the Player

1. Select your Player GameObject
2. Add the **WeaponController** component
3. Configure WeaponController:
   - **Fire Point**: Create an empty child GameObject positioned where bullets spawn (e.g., slightly to the right of player)
   - **Enemy Layers**: Set the layer mask to include enemy layers
   - **Shoot Key**: Default is Mouse0 (left click), change if needed
   - **Allow Hold To Shoot**: Whether holding the button continuously shoots
   - **Inventory Holder**: Should auto-find if player has RegularInventoryHolder

### 4. Set Up the HUD

1. Find your **HUDPanel** in the scene (should be under Canvas)
2. Inside HUDPanel, create a new GameObject called "WeaponInfo" (or similar)
3. Add the **WeaponHUD** component to WeaponInfo
4. Create UI elements inside WeaponInfo:
   - **Weapon Icon** (Image): Shows weapon sprite
   - **Weapon Name** (TextMeshProUGUI): Shows weapon name
   - **Ammo Text** (TextMeshProUGUI): Shows ammo count
5. In WeaponHUD component, assign:
   - **Weapon Icon**: The Image component
   - **Weapon Name Text**: The TextMeshProUGUI for name
   - **Ammo Text**: The TextMeshProUGUI for ammo
   - **Weapon Info Panel**: The parent GameObject (WeaponInfo)
6. Position the WeaponInfo panel where you want it (e.g., bottom-right corner)

### 5. Create a Weapon Pickup

1. Create a GameObject in the scene (e.g., "RiflePickup")
2. Add components:
   - **SpriteRenderer**: Set sprite to weapon icon
   - **CircleCollider2D** (or BoxCollider2D): Set as Trigger
   - **WeaponPickup** component
3. Configure WeaponPickup:
   - **Weapon To Give**: Assign your WeaponItem ScriptableObject
   - **Item Pickup Prefab**: Assign your item pickup prefab (if you have one)
   - **Use Proximity Pickup**: Check if you want E key pickup
   - **Proximity Distance**: How close player needs to be
   - **Pickup Key**: Key to press (default E)

### 6. Layer Setup

Make sure you have proper layers set up:
- **Enemy Layer**: For enemies that can be hit
- **Projectile Layer**: For projectiles (optional, but recommended)
- **Ground/Wall Layers**: For collision detection

In WeaponController, set the **Enemy Layers** mask to include enemy layers.

## How It Works

### Weapon Selection
- Weapons are stored in the **RegularInventory** (same as other items)
- When you select a weapon slot in the inventory, it automatically equips
- The WeaponController listens to inventory changes and equips/unequips accordingly

### Shooting
- Press **Mouse0** (or configured key) to shoot
- If "Allow Hold To Shoot" is enabled, holding the button continuously shoots
- Weapons respect fire rate cooldowns
- Ammo is consumed (unless infinite ammo is enabled)

### HUD Display
- When a weapon is equipped, the HUD shows:
  - Weapon icon
  - Weapon name
  - Current ammo / Max ammo (or ∞ for infinite)
- When no weapon is equipped, the weapon info panel is hidden

### Ammo System
- Each weapon has its own ammo count
- Ammo is consumed when shooting (if enabled)
- You can add ammo by calling `WeaponController.AddAmmo(amount)`
- Ammo resets to starting ammo when weapon is picked up

## Integration with Existing Systems

### Inventory System
- Weapons use the existing **RegularInventory** system
- They appear in the regular inventory slots
- Selecting a weapon slot equips it automatically
- Dropping a weapon works the same as other items

### Player Stats
- Weapon damage is **added** to player's base attack damage
- So if player has 20 base damage and weapon has 25 damage, total is 45
- Fire rate is independent of player attack speed (weapon has its own fire rate)

### Player Attack
- The existing **PlayerAttack** script handles melee (K key) and ranged (J key)
- The **WeaponController** handles weapon shooting (Mouse0)
- They can coexist - player can have both melee and ranged weapons

## Tips

1. **Projectile Setup**: Make sure projectiles have proper collision layers and are set as triggers
2. **Fire Point**: Position the fire point slightly in front of the player sprite
3. **Ammo Balance**: Adjust starting ammo and max ammo based on game balance
4. **Fire Rate**: Higher fire rate = faster shooting (e.g., 2.0 = 2 shots per second)
5. **Multiple Weapons**: Players can carry multiple weapons and switch between them via inventory

## Example: Creating a Rifle

1. Create WeaponItem: "Rifle" with:
   - Damage: 30
   - Fire Rate: 2.0
   - Max Ammo: 30
   - Starting Ammo: 30
   - Projectile Speed: 20
   - Uses Ammo: true
   - Infinite Ammo: false

2. Create Bullet prefab with:
   - Small sprite (e.g., 8x8 pixel circle)
   - Rigidbody2D (gravity = 0)
   - CircleCollider2D (trigger)
   - Projectile component

3. Assign bullet prefab to rifle's Projectile Prefab field

4. Create pickup in scene and assign rifle ScriptableObject

5. Player picks up rifle → equips automatically → can shoot with Mouse0

## Troubleshooting

- **Weapon not shooting**: Check that Fire Point is assigned and Enemy Layers are set
- **HUD not updating**: Make sure WeaponHUD references are assigned correctly
- **Projectiles not hitting**: Check layer masks and that colliders are triggers
- **Ammo not decreasing**: Check "Uses Ammo" and "Infinite Ammo" settings
- **Weapon not equipping**: Check that player has RegularInventoryHolder and inventory is set up

