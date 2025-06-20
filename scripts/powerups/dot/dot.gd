extends Area2D


@export var dot_eaten_score: int = 10


func _on_body_entered(_body: Node2D) -> void:
    Events.dot_eaten.emit(dot_eaten_score)
    queue_free()
