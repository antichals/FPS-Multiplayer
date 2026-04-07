# 🎮 FPS-Multiplayer (Unity)

## Overview

This project is a **multiplayer First-Person Shooter (FPS)** developed in **Unity** as a technical prototype inspired by games like *CS:GO*.  

The focus of the project is on implementing the core gameplay systems and understanding how multiplayer synchronization works, rather than on graphics or polish.

---

## Features

- Basic weapon system (shooting and damage)
- Multiplayer using a server-authoritative approach
- Player controller with synchronized movement
- Health and respawn system
- Simple UI (health, ammo, scoreboard)
- Basic visual effects (particles, animations)

---

## Project Structure

The project is organized into several main systems:

### Player System
- `PlayerController`: Handles player movement and input  
- `PlayerHealth`: Manages damage, death, and respawn  

### Weapon System
- `PlayerWeapon`: Shooting logic and weapon handling  
- Damage is processed on the server side  

### Networking
- Built using **FishNet**  
- Uses `ServerRpc` and `ObserversRpc` for communication between client and server  
- Synchronizes player state and gameplay events  

### UI
- Simple UI to display:
  - Health  
  - Ammo  
  - Scoreboard  

### Effects
- Particle effects for shooting  
- Basic animation synchronization  

---

## Technologies

- Unity 2022.3 LTS  
- FishNet (networking)  
- C#  

---

## What I Learned

- Basics of multiplayer architectures (server vs client authority)  
- Synchronizing player actions across the network  
- Structuring gameplay systems in a modular way  
- Debugging common multiplayer issues (desync, timing, state updates)  

---

## Author

Carlos Martinez  

---

## Notes

This project was developed as part of my final degree project (TFG).  
It is mainly focused on learning and experimentation with multiplayer systems.
