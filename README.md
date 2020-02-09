# ExerciseRemote

Exercise done by:

- Maxime Filipovich (Artlock)
- Pierre-Louis Verguin (Oxyporum-PLV)

IMPORTANT:
- All commits were made from one account.
- This is because we screenshared/liveshared the development/code.
- We decided it was best since neither of us knew the game's code beforehand.
- It helped figuring out solutions to the bugs and also to implement the new coop feature.

Steps to add a player (Could break if more than 9 are added):
- Duplicate a player prefab in the scene.
- Setup the player ids correctly (0, 1, etc...).
- Have as many players setup in rewired (Players/KeyboardLayouts/KeyboardMaps).

Co-op:
- If a player dies, both die
- If a player finishes, both finish
- Both players share their food
- Each player moves twice before the enemy does
- Turns go Player1 > Player2 > Enemies
