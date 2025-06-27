- Create the Pac-Man maze.
- Place a score and high score counter above the level, and a life counter below.
- The maze operates on a grid.
- There is a tunnel that allows Pac-Man and the ghosts to wrap across the screen, appearing on the other side.
- Fill the maze with dots and four large dots (Power Pellets).
- Each cell in the grid will contain a dot or power pellet.
- Create Pac-Man himself. He should be able to move in four directions through the maze.
- When Pac-Man collides with a dot, he will eat it, increasing the score.

- Add four ghosts.
    - They will chase Pac-Man through the level.
    - The ghosts start in a “pen” and are released after enough dots are eaten.
    - Each ghost will cycle between “chase mode” and “scatter mode.”
    - During chase mode, the ghosts will move toward specific cells to give the illusion of teamwork and intelligence.
        - Red ghost “Blinky” will target Pac-Man directly.
        - Pink ghost “Pinky” will try to get 4 tiles in front of Pac-Man.
        - Blue ghost “Inky” will target a special position.
            - Draw a line from Blinky’s position to the cell two tiles in front of Pac-Man, then double the length of the line.
            - That is Inky’s target position.
        - Orange ghost “Clyde” will target Pac-Man directly, but will scatter whenever he gets within an 8 tile radius of Pac-Man.
    - Each ghost has an assigned corner that it will scatter to during scatter mode.

- Add the “power pellet” mode. When Pac-man eats the pellet, the ghosts will turn blue (scared), and will scatter.
    - Pac-Man can eat the ghosts.
    - After a timer elapses, the ghosts will flash white, then return to normal.
    - Eaten ghosts will award points, turn into eyes, and then return to the pen before coming back as regular ghosts.

- Add the win states and lose states.
    - Pac-Man will die when eaten by a ghost, consuming a life.
    - When all dots are consumed, the level will reset.

- Stretch goal:
    - Feel free to customize the maze, add more/different AI behaviors, or add more levels.
    - Add fruit pickups.
        - Fruit appears after enough dots are eaten, and awards bonus points.


- Add main menu
- Add settings menu
- Add pause menu

- Add bgm

- Add SFX:
    - moving
    - eating fruit
    - eating ghost
    - eating power pellet
    - ghost eyes returning home
    - pacman death
    - intro
    - interlevel intro

- Save/Load settings/highscores

- Level / Powerup / Score
    - 1	Cherry	100
    - 2	Strawberry	300
    - 3–4	Orange	500
    - 5–6	Apple	700
    - 7–8	Melon	1,000
    - 9–10	Galaxian	2,000
    - 11–12	Bell	3,000
    - 13+	Key	5,000
- The fruit appears twice per level, generally near the maze center.
- If you don’t eat the fruit quickly, it will disappear after a few seconds.
- From Level 13 onward, the fruit remains a Key, awarding the maximum 5,000 points per appearance.

- Eating ghosts (for a single pellet run)
    - 1st	200
    - 2nd	400
    - 3rd	800
    - 4th	1,600

- White gates should not be accessible by pacman (= pacman cannot enter prison but ghosts can, e.g. when they die)

