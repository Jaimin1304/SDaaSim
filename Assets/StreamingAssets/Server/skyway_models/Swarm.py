from .SubSwarm import SubSwarm

class Swarm:
    def __init__(self):
        self.sub_swarms = [SubSwarm(self)]
        self.drones = []

    def add_sub_swarm(self, sub_swarm):
        self.sub_swarms.append(sub_swarm)
        sub_swarm.parent_swarm = self

    def remove_sub_swarm(self, sub_swarm):
        self.sub_swarms.remove(sub_swarm)
        sub_swarm.parent_swarm = None

    def assign_drone_to_sub_swarm(self, drone, sub_swarm):
        drone.sub_swarm = sub_swarm
        sub_swarm.drones.append(drone)