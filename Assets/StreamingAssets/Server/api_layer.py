import json

def process_request(msg):
    request = json.loads(msg)
    header = request['header']
    body = request['body']
    match header:
        case 'initSKyway':
            return ''
        case _:
            print('something went wrong')

def init_skyway():
    return