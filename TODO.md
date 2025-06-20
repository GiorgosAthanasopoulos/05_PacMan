# TODOs
## Map
- There is a tunnel that allows Pac-Man and the ghosts to wrap across the screen, appearing on the other side.

## Pacman
- He should be able to move in four directions through the maze.
- When Pac-Man collides with a dot, he will eat it, increasing the score.

## Ghosts
- Add four ghosts.
- They will chase Pac-Man through the level.
- The ghosts start in a “pen” and are released after enough dots are eaten.
- Each ghost will cycle between “chase mode” and “scatter mode.” 
- During chase mode, the ghosts will move toward specific cells to give the illusion of teamwork and intelligence.
    - Red ghost “Blinky” will target Pac-Man directly.
    - Pink ghost “Pinky” will try to get 4 tiles in front of Pac-Man.
    - Blue ghost “Inky” will target a special position. Draw a line from Blinky’s position to the cell two tiles in front of Pac-Man, then double the length of the line. That is Inky’s target position.
    - Orange ghost “Clyde” will target Pac-Man directly, but will scatter whenever he gets within an 8 tile radius of Pac-Man.
    - Each ghost has an assigned corner that it will scatter to during scatter mode.
    - For more details on the Pac-Man AI, check out this detailed breakdown. If you are using a modern game engine, then you will probably solve some problems (such as pathfinding) differently and will therefore have to improvise a little.

- When eaten:
    - Their body disappears, leaving just eyes, which return to the ghost house.

## Powerups
### Power pellet
- Add the “power pellet” mode.
- When Pac-man eats the pellet, the ghosts will turn blue (scared), and will scatter.
- Pac-Man can eat the ghosts.
- After a timer elapses, the ghosts will flash white, then return to normal.
- Eaten ghosts will award points, turn into eyes, and then return to the pen before coming back as regular ghosts.

## Win/Lost states
- Add the win states and lose states. 
- Pac-Man will die when eaten by a ghost, consuming a life.
- When all dots are consumed, the level will reset.

## Stretch goals
- Feel free to customize the maze, add more/different AI behaviors, or add more levels.

- Add fruit pickups. Fruit appears after enough dots are eaten, and awards bonus points. (This is a stretch goal because I forgot about it until now…)

## AI

- Make each ghost's ai

## UI

- Add main menu
- Add settings menu
- Add pause menu

## Music

- Add bgm

## SFX

- moving
- eating fruit
- eating ghost
- eating power pellet
- ghost eyes returning home
- pacman death
- intro
- interlevel intro

## Session

- Save/Load settings/highscores
