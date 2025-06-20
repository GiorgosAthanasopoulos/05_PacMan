extends CharacterBody2D


enum Direction {
    NONE = 0,
    UP = 1,
    LEFT = 2,
    DOWN = 3,
    RIGHT = 4,
}


@export var move_interval: float = .5 # in seconds
@export var tile_size: int = 16

@export var move_up_action: String = 'move_up'
@export var move_left_action: String = 'move_left'
@export var move_down_action: String = 'move_down'
@export var move_right_action: String = 'move_right'

@export var idle_animation: String = 'idle'
@export var moving_animation: String = 'moving'
@export var sprite_rotation_move_direction: Dictionary[Direction, int] = {
    Direction.UP: 270,
    Direction.LEFT: 180,
    Direction.DOWN: 90,
    Direction.RIGHT: 0,
    Direction.NONE: 0,
}


@onready var _animated_sprite: AnimatedSprite2D = $AnimatedSprite2D


var _direction: Direction = Direction.NONE
var _move_timer: float = 0


func _physics_process(delta: float) -> void:
    _handle_input()
    _process_input(delta)
    _handle_animation()


func _handle_input() -> void:
    if Input.is_action_just_pressed(move_up_action):
        _direction = Direction.UP
    if Input.is_action_just_pressed(move_left_action):
        _direction = Direction.LEFT
    if Input.is_action_just_pressed(move_down_action):
        _direction = Direction.DOWN
    if Input.is_action_just_pressed(move_right_action):
        _direction = Direction.RIGHT


func _process_input(delta: float) -> void:
    _move_timer -= delta

    if _move_timer > 0:
        return

    _move_timer = move_interval

    if not _can_move_direction():
        return

    _move_direction()


func _move_direction() -> void:
    if _direction == Direction.UP:
        global_position.y -= tile_size
    if _direction == Direction.LEFT:
        global_position.x -= tile_size
    if _direction == Direction.DOWN:
        global_position.y += tile_size
    if _direction == Direction.RIGHT:
        global_position.x += tile_size


func _can_move_direction() -> bool:
    var space_state: PhysicsDirectSpaceState2D = get_world_2d().direct_space_state
    var target: Vector2

    if _direction == Direction.UP:
        target = global_position + Vector2(0, -tile_size)
    if _direction == Direction.LEFT:
        target = global_position + Vector2(-tile_size, 0)
    if _direction == Direction.DOWN:
        target = global_position + Vector2(0, tile_size)
    if _direction == Direction.RIGHT:
        target = global_position + Vector2(tile_size, 0)

    var query: PhysicsRayQueryParameters2D = PhysicsRayQueryParameters2D.create(global_position, target)
    return space_state.intersect_ray(query) == {}


func _handle_animation() -> void:
    var still_in_idle_animation: bool = _direction != Direction.NONE and _animated_sprite.animation == idle_animation
    if still_in_idle_animation:
        _animated_sprite.animation = moving_animation
        _animated_sprite.play()

    _animated_sprite.rotation_degrees = sprite_rotation_move_direction[_direction]
