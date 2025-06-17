extends CharacterBody2D


enum Direction {NONE, UP, LEFT, DOWN, RIGHT}


@export var tile_size: int = 16
@export var speed: Vector2 = Vector2(6, 6)


@onready var animated_sprite: AnimatedSprite2D = $AnimatedSprite2D


var _direction: Direction = Direction.NONE
var _direction_just_changed: bool = false
var _has_moved: bool = false


func _physics_process(delta: float) -> void:
    _handle_input()
    _process_input(delta)
    _handle_animation()


func _handle_input() -> void:
    if Input.is_action_just_pressed("move_up"):
        _direction = Direction.UP
        _direction_just_changed = true
    if Input.is_action_just_pressed("move_left"):
        _direction = Direction.LEFT
        _direction_just_changed = true
    if Input.is_action_just_pressed("move_down"):
        _direction = Direction.DOWN
        _direction_just_changed = true
    if Input.is_action_just_pressed("move_right"):
        _direction = Direction.RIGHT
        _direction_just_changed = true

    if _direction_just_changed and not _has_moved:
        _has_moved = true


func _process_input(_delta: float) -> void:
    if _can_move_direction(_direction):
        var __: Vector2 = _move_direction(_direction)
        # var movement_vector: Vector2 = _move_direction(_direction)
        # var collision: KinematicCollision2D = move_and_collide(movement_vector * delta)

        # if collision != null:
        #     _direction = Direction.NONE


func _handle_animation() -> void:
    if _direction == Direction.NONE and not _has_moved:
        animated_sprite.animation = 'idle'
    else:
        animated_sprite.animation = 'moving'

        if _direction_just_changed:
            if _direction == Direction.UP:
                animated_sprite.rotation_degrees = 270
            if _direction == Direction.LEFT:
                animated_sprite.rotation_degrees = 180
            if _direction == Direction.DOWN:
                animated_sprite.rotation_degrees = 90
            if _direction == Direction.RIGHT:
                animated_sprite.rotation_degrees = 0

            _direction_just_changed = false


func _can_move_direction(__direction: Direction) -> bool:
    return true

func _move_direction(direction: Direction) -> Vector2:
    var movement_vector: Vector2 = Vector2.ZERO

    if direction == Direction.UP:
        movement_vector.y -= tile_size
    if direction == Direction.LEFT:
        movement_vector.x -= tile_size
    if direction == Direction.DOWN:
        movement_vector.y += tile_size
    if direction == Direction.RIGHT:
        movement_vector.x += tile_size

    return movement_vector
