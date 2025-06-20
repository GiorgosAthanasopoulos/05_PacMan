extends Area2D


@export var cherry_eaten_score: int = 100


func _on_body_entered(_body: Node2D) -> void:
    Events.cherry_eaten.emit(cherry_eaten_score)
    queue_free()
