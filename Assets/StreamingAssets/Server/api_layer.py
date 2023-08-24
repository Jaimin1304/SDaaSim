import json
from skyway_model import *
import custom_algorithm
from typing import Dict
import globals


skyway = Skyway(None, None, None, None, None)


def execute_user_logic():
    return custom_algorithm.run(skyway)


def update_swarm(data):
    print('update swarm')


def update_subswarm(data):
    print('update subswarm')
    print(data)
    id = data.get('id')
    newPos = data.get('position')
    nodeId = data.get('node')
    edgeId = data.get('edge')
    # find target subswarm in skyway
    subSwarm = skyway.subSwarms.get(id)
    # update the subSwarm
    subSwarm.position = newPos
    subSwarm.node = skyway.nodes.get(nodeId)
    subSwarm.edge = skyway.edges.get(edgeId)


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
            {id: payloads[id] for id in drone["payloads"]}
        )
        for drone in data["drones"]
    }

    #pads: Dict[str, Pad] = {
    #    pad["id"]: Pad(
    #        pad["id"],
    #        pad["node"],
    #        pad["isAvailable"]
    #    )
    #    for pad in data["pads"]
    #}

    nodes: Dict[str, Node] = {
        node["id"]: Node(
            node["id"],
            node["position"],
            #{id: pads[id] for id in node["pads"]},
            {id: drones[id] for id in node["landedDrones"]},
            node["edges"]
        )
        for node in data["nodes"]
    }

    #for pad in pads.values():
    #    pad.node = nodes[pad.node]

    edges: Dict[str, Edge] = {
        edge["id"]: Edge(
            edge["id"],
            nodes[edge["leftNode"]],
            nodes[edge["rightNode"]],
            edge["totalLength"]
        )
        for edge in data["edges"]
    }

    for node in nodes.values():
        node.edges = {id: edges[id] for id in node.edges}

    requests: Dict[str, Request] = {
        request["id"]: Request(
            request["id"],
            nodes[request["startNode"]],
            nodes[request["destNode"]],
            {id: payloads[id] for id in request["payloads"]}
        )
        for request in data["requests"]
    }

    subSwarms: Dict[str, SubSwarm] = {
        subSwarm["id"]: SubSwarm(
            subSwarm["id"],
            subSwarm["position"],
            {id: drones[id] for id in subSwarm["drones"]},
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
            {id: subSwarms[id] for id in swarm["subSwarms"]}
        )
        for swarm in data["swarms"]
    }

    skyway = Skyway(
        nodes,
        edges,
        requests,
        swarms,
        subSwarms
    )

    skyway.log()


def process_request(msg):
    request = json.loads(msg)
    header = request['header']
    body = request['body']

    match header:
        case globals.init_skyway_header:
            init_skyway(body)
            custom_algorithm.init(skyway)

        case globals.update_swarm_header:
            update_swarm(body)

        case globals.update_subswarm_header:
            update_subswarm(body)

        case _:
            print(f'unknown header: {header}')

    return


def get_skyway():
    return skyway