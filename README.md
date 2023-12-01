# L2-Unity

<p>This project aim is to create a basic playable demo of Lineage2 on Unity.</p>

This [video](https://www.youtube.com/watch?v=IEHY37bJ7nk) inspired me to start on this project.

<p>Preview of the current state of the project:</p>

![Preview](https://media.discordapp.net/attachments/584218502148259901/1180162232814940280/image.png?ex=657c6aba&is=6569f5ba&hm=7ba3f918f9a96d7a48c29f5aaa063b5e05543ad976bb2765c02522a6b9af696d&=&format=webp&quality=lossless)

## What are the expected features?

For now the aim is to create a basic demo, therefore only basic features will be available:
- Client-side Pathfinding
- Click to move and WASD movements
- Camera collision
- Basic UI
    - Status window
    - Chat window
    - Target window
    - Nameplates
    - Skillbar
- Basic combat (only autoattacks)
- Basic RPG features
    - HP Loss and regen
    - Exp gain on kills
    - Leveling
- Small range of npc models
    - 1 to 2 models for players
    - 2 to 5 npc types
- Server/Client features (java.net.ServerSocket)
    - Player position/rotation sync
    - Animation sync
    - Chat
    - Friendly NPCs
    - Monsters (AI with Pathfinding)
- Import Lineage2's world
    - Talking island region only (for now)
        - StaticMeshes
        - Brushes
        - Terrain
        - DecoLayer
- Day/Night cycle

## How to run?

<p>Open the "Game" scene and drag&drop the 1x_1x scenes into your scene.</p>

![Import](https://media.discordapp.net/attachments/584218502148259901/1180168459104034877/image.png?ex=657c7087&is=6569fb87&hm=8da8cfdd84e33b729bf989b79b547dd8de97faf06dfa511b9cd4b7961501781a&=&format=webp&quality=lossless&width=575&height=608)

## Contributing

Pull requests are very welcome. For major changes, please open an issue first
to discuss what you would like to change.
