# L2-Unity

<p>This project aim is to create a basic playable demo of Lineage2 on Unity.</p>

This [video](https://www.youtube.com/watch?v=IEHY37bJ7nk) inspired me to start on this project.

<p>Preview of the current state of the project:</p>

![Preview](https://media.discordapp.net/attachments/584218502148259901/1180162232814940280/image.png?ex=657c6aba&is=6569f5ba&hm=7ba3f918f9a96d7a48c29f5aaa063b5e05543ad976bb2765c02522a6b9af696d&=&format=webp&quality=lossless)

## What are the expected features?

For now the aim is to create a basic demo, therefore only basic features will be available:
- Client-side Pathfinding 🛠️ (Need to search for path in neighboring terrains)
- Click to move and WASD movements ✅
- Camera collision ✅
- Basic UI
    - Status window ✅
    - Chat window ✅
    - Target window ✅
    - Nameplates ✅
    - Skillbar
- Basic combat (only autoattacks)
- Basic RPG features 🛠️ (Structure only for now)
    - HP Loss and regen 🛠️ (Players can send and receive damage packets for now)
    - Exp gain on kills
    - Leveling
- Small range of npc models
    - 1 to 2 models for players 🛠️ (Only Female Dark elf for now)
    - All Monsters of map 17_25 (Talking island) ✅
    - All NPCs of map 17_25 (Talking island)
- Server/Client features (server project [here](https://gitlab.com/shnok/unity-mmo-server))
    - Player position/rotation sync ✅
    - Animation sync ✅
    - Chat ✅
    - Server Ghosting/Grid system ✅
    - NPCs
    - Monsters ✅
    - Monsters AI with Pathfinding 🛠️ (Needs refactoring)
- Import Lineage2's world
    - Talking island region only (for now) ✅
        - StaticMeshes ✅
        - Brushes ✅
        - Terrain ✅
        - DecoLayer ✅
- Day/Night cycle 🛠️ (Ready but need to sync with server time)
- Game sounds (FMOD project [here](https://gitlab.com/shnok/l2-unity/-/tree/main/l2-unity-fmod/))
    - Ambient sounds 🛠️
    - Step sounds (based on ground) ✅
    - UI sounds ✅
    - NPC sounds 🛠️

## How to run?

<p>Open the "Game" scene and drag&drop the 1x_1x scenes into your scene.</p>

![Import](https://media.discordapp.net/attachments/584218502148259901/1180168459104034877/image.png?ex=657c7087&is=6569fb87&hm=8da8cfdd84e33b729bf989b79b547dd8de97faf06dfa511b9cd4b7961501781a&=&format=webp&quality=lossless&width=575&height=608)

If you don't want to setup the [server](https://gitlab.com/shnok/unity-mmo-server) and just want to run in an empty map. Select the <b>Game</b> GameObject in the <b>Game</b> scene and tick the <i>"offline mode"</i> checkbox.

![offline](https://cdn.discordapp.com/attachments/584218502148259901/1182499680056250418/image.png?ex=6584eba5&is=657276a5&hm=f6f37f9d0a9c7f2e1f43060f11b6b62fb7cf8975a26e91796f5990b3e09b97af&&width=575&height=608)



## Contributing

Pull requests are very welcome. For major changes, please open an issue first
to discuss what you would like to change.
