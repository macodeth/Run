extends Node

var lst = {}

func play_event(event_name: String):
	var instance = FMODRuntime.create_instance_path("event:/" + event_name)
	instance.start()
	instance.release()

func play_event_loop(sawId: int, event_name: String):
	if not lst.has(sawId):
		var instance = FMODRuntime.create_instance_path("event:/" + event_name)
		instance.start()
		lst[sawId] = instance

var reverb = 0.0

func set_distance (sawId: int, val: float):
	var event = lst[sawId] as EventInstance;
	print("distance = " + str(val))
	event.set_parameter_by_name("distance", val)

func set_reverb (rev: float):
	reverb = rev

func _process(_delta):
	FMODStudioModule.get_studio_system().set_parameter_by_name("reverb", reverb, false)

func stop_all():
	FMODStudioModule.get_studio_system().get_bus("bus:/").stop_all_events(0)
