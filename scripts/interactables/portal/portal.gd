extends Area2D


@export var left: bool = true


func _on_body_entered(_body: Node2D) -> void:
    Events.portal_used.emit(left, global_position)
