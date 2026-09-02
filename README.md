# vs_cop

An isometric survivor-vs-zombies shooter built in Unity, with a PSX (PS1)-inspired retro visual style.

🎮 **Play it on itch.io:** https://kutpac.itch.io/isometric-zombie-shooter-demo

## Features

- Isometric twin-stick style shooting with weapon switching (pistol / rifle)
- Zombie AI with noticing/alert states, hit reactions, a crawl-on-low-health mechanic, and corpse persistence
- Escalating difficulty over time — zombie spawn rate and max horde size ramp up the longer you survive
- Dynamic weather: rain with rippling puddle decals, thunder, and ambient audio
- Blood spray and blood puddle decals on zombie hits
- Rare healing pill drops from killed zombies
- PSX-inspired retro rendering (low-res render target + point filtering)
- Full HUD: health, ammo, weapon indicator, and a game-over flow

## Built With

- Unity 6000.3.9f1 (Universal Render Pipeline)
- Unity's new Input System
- NavMesh AI (`com.unity.ai.navigation`)
- Custom Shader Graphs (decals, ground ripple/puddle effects)
- ProBuilder (level geometry)

## Controls

- **WASD** — move
- **Mouse** — aim
- **Left Click** — shoot
- **R** — reload
- **1 / 2** — switch weapon (rifle / pistol)

## Credits

Built using a mix of original code/design and third-party art, audio, and model assets (see `Assets/3rd Party Assets/` for individual pack attributions). Game logic and scripting were developed with AI assistance (Claude).

## License

This project is for portfolio/educational purposes.
