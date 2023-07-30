from typing import List


class Payload:
    def __init__(self, id, weight):
        self.id = id
        self.weight = weight


class Request:
    def __init__(self, id, startNode, destNode, payloads: List[Payload]):
        self.id = id
        self.startNode = startNode
        self.destNode = destNode
        self.payloads = payloads


class Edge:
    def __init__(self, id, leftNode, rightNode):
        self.id = id
        self.leftNode = leftNode
        self.rightNode = rightNode


class Pad:
    def __init__(self, id, node, isAvailable):
        self.id = id
        self.node = node
        self.isAvailable = isAvailable


class Drone:
    def __init__(self, id, selfWeight, speed, maxPayloadWeight, batteryStatus, payloads: List[str]):
        self.id = id
        self.selfWeight = selfWeight
        self.speed = speed
        self.maxPayloadWeight = maxPayloadWeight
        self.batteryStatus = batteryStatus
        self.payloads = payloads


class SubSwarm:
    def __init__(self, id, position, drones: List[Drone], node, edge, wayPointIndex, currentState):
        self.id = id
        self.position = position
        self.drones = drones
        self.node = node
        self.edge = edge
        self.wayPointIndex = wayPointIndex
        self.currentState = currentState


class Swarm:
    def __init__(self, id, request, subSwarms: List[SubSwarm]):
        self.id = id
        self.request = request
        self.subSwarms = subSwarms


class Node:
    def __init__(self, id, position, pads: List[Pad], drones, edges: List[str]):
        self.id = id
        self.position = position
        self.pads = pads
        self.drones = drones
        self.edges = edges


class Skyway:
    def __init__(self, nodes: List[Node], edges: List[Edge], requests: List[Request], swarms: List[Swarm]):
        self.nodes = nodes
        self.edges = edges
        self.requests = requests
        self.swarms = swarms
