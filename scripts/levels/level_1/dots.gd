extends Node2D


@onready var dot: PackedScene = preload('res://scenes/powerups/dot/dot.tscn')


func _ready() -> void:
    print(count_dots(self))


func count_dots(root: Node) -> int:
    if get_child_count() == 0:
        return root.scene_file_path == dot.resource_path

    if root.scene_file_path == dot.resource_path:
        return 1

    var count: int = 0

    for node: Node in root.get_children():
        count += count_dots(node)

    return count
