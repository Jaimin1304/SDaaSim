class Drone:
    def __init__(self, swarm, sub_swarm, position, weight, max_payload_weight, speed, battery_life):
        self.swarm = swarm
        self.sub_swarm = sub_swarm
        self.position = position
        self.weight = weight
        self.payloads = []
        self.max_payload_weight = max_payload_weight
        self.speed = speed
        self.battery_life = battery_life