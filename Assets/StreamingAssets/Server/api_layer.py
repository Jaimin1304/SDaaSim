import json
from skyway_model import *
import custom_algorithm

skyway_data = None
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
    global skyway_data

    payloads = []
    for r in data.get('requests'):
        for p in r.get('payloads'):
            payload = Payload(p.get('id'), p.get('weight'))
            payloads.append(payload)
    
    



def update_swarm(data):
    print('update swarm')
    print(data)


def update_subswarm(data):
    print('update subswarm')
    print(skyway_data['swarms']['subSwarms'])
    print('---')
    print(data)


def get_skyway():
    return skyway_data


def set_subswarm_targetNode(subswarm_id, node_id):
    instructions.append(
        {"set_subswarm_targetNode": {'subswarm_id': subswarm_id, 'node_id': node_id}}
    )


def execute_user_logic():
    custom_algorithm.run()
    return instructions
