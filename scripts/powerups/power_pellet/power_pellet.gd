extends Area2D

@export var power_pellet_eaten_score: int = 50
@export var power_pellet_time: float = 6.5 # TODO: drops to 0 by level 19


func _on_body_entered(_body: Node2D) -> void:
    Events.power_pellet_eaten.emit(power_pellet_eaten_score, power_pellet_time)
    queue_free()
