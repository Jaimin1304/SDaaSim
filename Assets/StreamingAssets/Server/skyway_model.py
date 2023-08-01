from typing import List


class Payload:
    def __init__(self, id, weight):
        self.id = id
        self.weight = weight

    def log(self):
        print('--payload--')
        print(self.id)
        print(self.weight)


class Request:
    def __init__(self, id, startNode, destNode, payloads: List[Payload]):
        self.id = id
        self.startNode = startNode
        self.destNode = destNode
        self.payloads = payloads

    def log(self):
        print('----request----')
        print(self.id)
        print(self.startNode.id)
        print(self.destNode.id)
        for i in self.payloads:
            i.log()


class Edge:
    def __init__(self, id, leftNode, rightNode):
        self.id = id
        self.leftNode = leftNode
        self.rightNode = rightNode

    def log(self):
        print('----edge----')
        print(self.id)
        print(self.leftNode.id)
        print(self.rightNode.id)


class Pad:
    def __init__(self, id, node, isAvailable):
        self.id = id
        self.node = node
        self.isAvailable = isAvailable

    def log(self):
        print('--pad--')
        print(self.id)
        print(self.node.id)
        print(self.isAvailable)


class Drone:
    def __init__(self, id, selfWeight, speed, maxPayloadWeight, batteryStatus, payloads: List[Payload]):
        self.id = id
        self.selfWeight = selfWeight
        self.speed = speed
        self.maxPayloadWeight = maxPayloadWeight
        self.batteryStatus = batteryStatus
        self.payloads = payloads

    def log(self):
        print('--edge--')
        print(self.id)
        print(self.selfWeight)
        print(self.speed)
        print(self.maxPayloadWeight)
        print(self.batteryStatus)
        for i in self.payloads:
            i.log()


class SubSwarm:
    def __init__(self, id, position, drones: List[Drone], node, edge, wayPointIndex, currentState):
        self.id = id
        self.position = position
        self.drones = drones
        self.node = node
        self.edge = edge
        self.wayPointIndex = wayPointIndex
        self.currentState = currentState

    def log(self):
        print('---subSwarm---')
        print(self.id)
        print(self.position)
        for i in self.drones:
            i.log()
        print(self.node.id)

        if self.edge is None:
            print('no edge')
        else:
            print('edge: ' + str(self.edge.id))

        print('wayPointIndex: ' + str(self.wayPointIndex))
        print('currentState: ' + self.currentState)


class Swarm:
    def __init__(self, id, request, subSwarms: List[SubSwarm]):
        self.id = id
        self.request = request
        self.subSwarms = subSwarms

    def log(self):
        print('----swarm----')
        print(self.id)
        print(self.request)
        for i in self.subSwarms:
            i.log()


class Node:
    def __init__(self, id, position, pads: List[Pad], drones: List[Drone], edges: List[Edge]):
        self.id = id
        self.position = position
        self.pads = pads
        self.drones = drones
        self.edges = edges

    def log(self):
        print('----node----')
        print(self.id)
        print(self.position)
        for i in self.pads:
            i.log()
        for i in self.drones:
            i.log()
        for i in self.edges:
            print(i.id)


class Skyway:
    def __init__(self, nodes: List[Node], edges: List[Edge], requests: List[Request], swarms: List[Swarm]):
        self.nodes = nodes
        self.edges = edges
        self.requests = requests
        self.swarms = swarms

    def log(self):
        print('--skyway--')
        for i in self.nodes:
            i.log()
        for i in self.edges:
            i.log()
        for i in self.requests:
            i.log()
        for i in self.swarms:
            i.log()
