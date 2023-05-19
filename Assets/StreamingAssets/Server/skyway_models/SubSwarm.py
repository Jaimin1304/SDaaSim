class SubSwarm:
    def __init__(self, parent_swarm):
        self.parent_swarm = parent_swarm
        self.drones = []

    def add_drone(self, drone):
        self.drones.append(drone)
        drone.sub_swarm = self

    def remove_drone(self, drone):
        self.drones.remove(drone)
        drone.sub_swarm = None