extends Node2D


# When dying update high score if needed (and save to file)

const MAX_LIFES: int = 3
const SCORE_LIFE_INCREASE_THRESHOLD: int = 10_000


@onready var up1_label: Label = $'UI/CanvasLayer/HBoxContainer/VBoxContainer/1UP_Label'
@onready var high_score_label: Label = $UI/CanvasLayer/HBoxContainer/VBoxContainer2/HighScoreLabel
@onready var up2_label: Label = $'UI/CanvasLayer/HBoxContainer/VBoxContainer3/2UP_Label'


var _score: int = 0
var _lifes: int = MAX_LIFES


func _ready() -> void:
    # TODO: load high score
    #
    var error: Error = Events.dot_eaten.connect(_increase_score) as Error
    if error != OK:
        print('Failed to connect dot_eaten to _increase_score in level_1.gd: ', error_string(error))

    error = Events.power_pellet_eaten.connect(_on_power_pellet_eaten) as Error
    if error != OK:
        print('Failed to connect power_pellet_eaten to _on_power_pellet_eaten in level_1.gd: ', error_string(error))

    error = Events.cherry_eaten.connect(_increase_score) as Error
    if error != OK:
        print('Failed to connect cherry_eaten to _increase_score in level_1.gd: ', error_string(error))


func _on_power_pellet_eaten(score: int, _time: float) -> void:
    # TODO: do i have to do smth about the power pellet consumption? (timer/ghosts)
    #
    _increase_score(score)


func _increase_score(score: int) -> void:
    _score += score
    up1_label.text = str(_score)

    if _score > SCORE_LIFE_INCREASE_THRESHOLD:
        _update_lifes(false)


func _update_lifes(lost: bool) -> void:
    if _lifes == MAX_LIFES and not lost:
        # TODO: more than MAX_LIFES?
        return

    if _lifes == 0 and lost:
        # TODO: lose?
        return

    _lifes += -1 if lost else 1
    if lost:
        # TODO: hide pacman life ui nodes
        pass
    else:
        # TODO: show pacman life ui nodes
        pass
