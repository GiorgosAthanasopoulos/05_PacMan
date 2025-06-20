extends Area2D


@export var DOT_EATEN_SCORE: int = 10


func _on_body_entered(_body: Node2D) -> void:
    Events.dot_eaten.emit(DOT_EATEN_SCORE)
    queue_free()
