from .Node import Node
from .Edge import Edge
from .Request import Request
from .Swarm import Swarm

class Skyway:
    def __init__(self, nodes, edges, requests, swarms):
        self.nodes = nodes
        self.edges = edges
        self.requests = requests
        self.swarms = swarms