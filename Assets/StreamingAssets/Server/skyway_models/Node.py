from .Pad import Pad

class Node:
    def __init__(self, id, recharge_pads, position):
        self.id = id
        self.recharge_pads = recharge_pads
        self.position = position
        self.drones = []
        self.edges = []  # Adding edges list to Node class
