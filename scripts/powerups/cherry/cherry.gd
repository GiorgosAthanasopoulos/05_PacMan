extends Area2D


@export var CHERRY_EATEN_SCORE: int = 100


func _on_body_entered(_body: Node2D) -> void:
    Events.cherry_eaten.emit(CHERRY_EATEN_SCORE)
    queue_free()
