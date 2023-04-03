import socket
import json

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind(('127.0.0.1', 1235))
s.listen(5)

print('--- py server up ---')

def dist(pos_1, pos_2):
    x1 = pos_1['x']
    y1 = pos_1['y']
    z1 = pos_1['z']
    x2 = pos_2['x']
    y2 = pos_2['y']
    z2 = pos_2['z']
    return ((x1-x2)**2 + (y1-y2)**2 + (z1-z2)**2)*(1/2)

def play_algorithm(msg):
    data = json.loads(msg)
    player_pos = data['playerPosition']
    floor_pos_lst = data['floorPositions']

    target = {'x': 22334455, 'y': 0, 'z': 0}
    for i in floor_pos_lst:
        if i['z'] > player_pos['z'] and dist(i, player_pos) < dist(target, player_pos):
            target = i
    if target['x'] == 22334455:
        return '0 0 True'
    if dist(player_pos, target) > 8:
        if player_pos['x'] < target['x']:
            return '1 1 True'
        else:
            return '-1 1 True'
    return '0 1 False'

while True:
    clientsocket, address = s.accept()
    msg = clientsocket.recv(1024).decode('utf-8')
    if msg == 'exit':
        break
    print(f'Connection from {address} has been established, msg: {msg}')
    clientsocket.send(bytes(play_algorithm(msg), 'utf-8'))
    clientsocket.close()
