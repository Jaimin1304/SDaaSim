import api
from skyway_model import Skyway
from typing import List, Dict, Tuple
import random
import heapq


class Node:
    def __init__(self, id: str):
        self.id = id
        self.edges = []  # List of edges connected to this node

    def add_edge(self, edge) -> None:
        self.edges.append(edge)


class Edge:
    def __init__(self, node1: Node, node2: Node, weight: float):
        self.node1 = node1
        self.node2 = node2
        self.weight = weight

    def other(self, current: Node) -> Node:
        # Return the other node of the edge
        return self.node1 if current == self.node2 else self.node2


def dijkstra(nodes: Dict[str, Node], start_id: str, end_id: str) -> Tuple[float, List[str]]:
    distances = {node: float('infinity') for node in nodes}
    previous_nodes = {node: None for node in nodes}

    distances[start_id] = 0
    priority_queue = [(0, start_id)]

    while priority_queue:
        current_distance, current_id = heapq.heappop(priority_queue)

        if current_distance > distances[current_id]:
            continue

        for edge in nodes[current_id].edges:
            neighbor = edge.other(nodes[current_id])
            new_distance = current_distance + edge.weight

            if new_distance < distances[neighbor.id]:
                distances[neighbor.id] = new_distance
                previous_nodes[neighbor.id] = nodes[current_id]
                heapq.heappush(priority_queue, (new_distance, neighbor.id))

    path = []
    while end_id:
        path.append(end_id)
        if previous_nodes[end_id] is None:
            break
        end_id = previous_nodes[end_id].id
    path.reverse()

    return distances[end_id], path


computed_path = []  # Global variable to store the computed path


def init(skyway: Skyway):
    global computed_path
    print('init your own data structures')

    # init dijkstra
    nodes = {}
    edges = {}

    # init nodes
    for key, value in skyway.nodes.items():
        nodes[key] = Node(value.id)

    # init edges
    for key, value in skyway.edges.items():
        edges[key] = Edge(nodes[value.leftNode.id],
                          nodes[value.rightNode.id], value.totalLength)
        nodes[value.leftNode.id].add_edge(edges[key])
        nodes[value.rightNode.id].add_edge(edges[key])

    startNode_id = list(skyway.requests.values())[0].startNode.id
    destNode_id = list(skyway.requests.values())[0].destNode.id
    # compute shortest path tree
    distance, computed_path = dijkstra(nodes, startNode_id, destNode_id)
    print(distance)
    print(computed_path)


def run(skyway: Skyway):
    instructions = []
    print('run user logic')

    ###############################
    #  write your algorithm here  #
    ###############################

    for subSwarm in skyway.subSwarms.values():
        # land and recharge if low power
        for drone in subSwarm.drones.values():
            print(drone.batteryStatus)
            if drone.batteryStatus < 0.4:
                api.subswarm_land(subSwarm.id, subSwarm.node.id, instructions)
                print("subswarm landed")
                return instructions
        # find next edge to go
        next_node_id = None
        for i in range(len(computed_path)):
            if computed_path[i] == subSwarm.node.id:
                if i == len(computed_path) - 1:  # reach the end
                    next_node_id = computed_path[i-1]
                    print("reach target")
                next_node_id = computed_path[i+1]
        next_edge_id = None
        for edge in subSwarm.node.edges.values():
            if edge.leftNode.id == next_node_id or edge.rightNode.id == next_node_id:
                next_edge_id = edge.id
        print(next_node_id)
        print(next_edge_id)
        api.set_subswarm_edge(subSwarm.id, next_edge_id, instructions)

    ###############################
    # the end of custom algorithm #
    ###############################

    print('user logic execution complete')
    return instructions
