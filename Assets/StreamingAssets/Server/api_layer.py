import json
from skyway_model import *
import custom_algorithm
from typing import Dict

skyway = None
instructions = []


def process_request(msg):
    request = json.loads(msg)
    header = request['header']
    body = request['body']

    match header:
        case 'initSkyway':
            init_skyway(body)

        case 'updateSwarm':
            update_swarm(body)

        case 'updateSubSwarm':
            update_subswarm(body)

        case _:
            print(f'unknown header: {header}')


def init_skyway(data):
    global skyway

    payloads: Dict[str, Payload] = {
        payload["id"]: Payload(payload["id"], payload["weight"])
        for payload in data["payloads"]
    }

    drones: Dict[str, Drone] = {
        drone["id"]: Drone(
            drone["id"],
            drone["selfWeight"],
            drone["speed"],
            drone["maxPayloadWeight"],
            drone["batteryStatus"],
            [payloads[id] for id in drone["payloads"]]
        )
        for drone in data["drones"]
    }

    pads: Dict[str, Pad] = {
        pad["id"]: Pad(pad["id"], pad["node"], pad["isAvailable"])
        for pad in data["pads"]
    }

    nodes: Dict[str, Node] = {
        node["id"]: Node(
            node["id"],
            node["position"],
            [pads[id] for id in node["pads"]],
            [drones[id] for id in node["drones"]],
            node["edges"]
        )
        for node in data["nodes"]
    }

    for pad in pads.values():
        pad.node = nodes[pad.node]

    edges: Dict[str, Edge] = {
        edge["id"]: Edge(
            edge["id"],
            nodes[edge["leftNode"]],
            nodes[edge["rightNode"]]
        )
        for edge in data["edges"]
    }

    for node in nodes.values():
        node.edges = [edges[id] for id in node.edges]

    requests: Dict[str, Request] = {
        request["id"]: Request(
            request["id"],
            nodes[request["startNode"]],
            nodes[request["destNode"]],
            [payloads[id] for id in request["payloads"]]
        )
        for request in data["requests"]
    }

    subSwarms: Dict[str, SubSwarm] = {
        subSwarm["id"]: SubSwarm(
            subSwarm["id"],
            subSwarm["position"],
            [drones[id] for id in subSwarm["drones"]],
            nodes[subSwarm["node"]],
            # edge could be None
            edges[subSwarm["edge"]] if subSwarm["edge"] != '' else None,
            subSwarm["wayPointIndex"],
            subSwarm["currentState"]
        )
        for subSwarm in data["subSwarms"]
    }

    swarms: Dict[str, Swarm] = {
        swarm["id"]: Swarm(
            swarm["id"],
            requests[swarm["request"]],
            [subSwarms[id] for id in swarm["subSwarms"]]
        )
        for swarm in data["swarms"]
    }

    skyway = Skyway(
        [nodes[id] for id in nodes],
        [edges[id] for id in edges],
        [requests[id] for id in requests],
        [swarms[id] for id in swarms]
    )

    skyway.log()


def update_swarm(data):
    print('update swarm')


def update_subswarm(data):
    print('update subswarm')


def get_skyway():
    return skyway


def set_subswarm_targetNode(subswarm_id, node_id):
    instructions.append(
        {"set_subswarm_targetNode": {'subswarm_id': subswarm_id, 'node_id': node_id}}
    )


def execute_user_logic():
    custom_algorithm.run()
    return instructions
