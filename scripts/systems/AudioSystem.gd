extends Node


func play_event(event_name: String):
	var instance = FMODRuntime.create_instance_path("event:/" + event_name)
	instance.start()
	instance.release()

