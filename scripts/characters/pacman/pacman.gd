extends CharacterBody2D


enum Direction {
    NONE = 0,
    UP = 1,
    LEFT = 2,
    DOWN = 3,
    RIGHT = 4,
}


@export var move_interval: float = .5 # second
@export var tile_size: int = 16


@onready var animated_sprite: AnimatedSprite2D = $AnimatedSprite2D


var _direction: Direction = Direction.NONE
var _move_timer: float = 0


func _physics_process(delta: float) -> void:
    _handle_input()
    _process_input(delta)
    _handle_animation()


func _handle_input() -> void:
    if Input.is_action_just_pressed('move_up'):
        _direction = Direction.UP
    if Input.is_action_just_pressed('move_left'):
        _direction = Direction.LEFT
    if Input.is_action_just_pressed('move_down'):
        _direction = Direction.DOWN
    if Input.is_action_just_pressed('move_right'):
        _direction = Direction.RIGHT


func _process_input(delta: float) -> void:
    _move_timer -= delta

    if _move_timer > 0:
        return

    _move_timer = move_interval
    var movement_vector: Vector2 = Vector2.ZERO

    # TODO: actually move pacman (solving this will also solve the same problem for ghosts)
    if _direction == Direction.UP:
        movement_vector.y -= tile_size
        pass
    if _direction == Direction.LEFT:
        movement_vector.x -= tile_size
        pass
    if _direction == Direction.DOWN:
        movement_vector.y += tile_size
        pass
    if _direction == Direction.RIGHT:
        movement_vector.x += tile_size
        pass

    var collision: KinematicCollision2D = move_and_collide(movement_vector)
    if collision != null:
        # TODO: handle collisions
        pass


func _handle_animation() -> void:
    if _direction != Direction.NONE and animated_sprite.animation == 'idle':
        animated_sprite.animation = 'moving'
        animated_sprite.play()

    if _direction == Direction.UP:
        animated_sprite.rotation_degrees = 270
    if _direction == Direction.LEFT:
        animated_sprite.rotation_degrees = 180
    if _direction == Direction.DOWN:
        animated_sprite.rotation_degrees = 90
    if _direction == Direction.RIGHT:
        animated_sprite.rotation_degrees = 0
