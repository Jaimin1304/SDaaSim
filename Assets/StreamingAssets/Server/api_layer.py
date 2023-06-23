import json

skyway_data = {}


def process_request(msg):
    request = json.loads(msg)
    header = request['header']
    body = request['body']
    match header:
        case 'initSKyway':
            init_skyway(body)
        case _:
            print('something went wrong')


def init_skyway(data):
    global skyway_data
    skyway_data = data


def get_skyway():
    return skyway_data


def set_subswarm_targetNode(subswarm_id, node_id):
    return {"set_subswarm_targetNode": {'subswarm_id': subswarm_id, 'node_id': node_id}}
