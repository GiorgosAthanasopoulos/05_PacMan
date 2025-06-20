extends Area2D

@export var POWER_PELLET_EATEN_SCORE: int = 50


func _on_body_entered(_body: Node2D) -> void:
    Events.power_pellet_eaten.emit(POWER_PELLET_EATEN_SCORE)
    queue_free()
