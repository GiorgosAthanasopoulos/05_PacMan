extends CharacterBody2D


@export var wearing_time: float = 3 # in seconds


@onready var ghost: AnimatedSprite2D = $Ghost
@onready var eyes: AnimatedSprite2D = $Eyes


var _scared_time: float = 0
var _wearing_time: float = 0
var _just_scared: bool = false
var _just_wearing: bool = false


func _ready() -> void:
    var error: Error = Events.power_pellet_eaten.connect(_on_power_pellet_eaten) as Error
    if error != OK:
        print('Failed to connect power_pellet_eaten to _on_power_pellet_eaten in blinky.gd: ', error_string(error))


func _physics_process(delta: float) -> void:
    _handle_scared(delta)
    _handle_animation()


func _handle_scared(delta: float) -> void:
    if _scared_time > 0:
        _scared_time -= delta

        if _scared_time <= 0:
            _wearing_time = wearing_time
            _just_wearing = true

        return

    if _wearing_time > 0:
        _wearing_time -= delta

        return


func _handle_animation() -> void:
    if _just_scared:
        ghost.animation = 'moving_scared' # TODO: dont do this every time
        ghost.play()
        eyes.animation = 'scared'
        _just_scared = false
        return

    if _just_wearing:
        ghost.animation = 'moving_wearing'
        ghost.play()
        eyes.animation = 'scared'
        _just_wearing = false
        return

    if _scared_time <= 0 and _wearing_time <= 0:
        ghost.animation = 'moving'
        eyes.animation = 'default'
        ghost.play()

    
func _on_power_pellet_eaten(_score: int, time: float) -> void:
    _scared_time = time

    _just_scared = true
