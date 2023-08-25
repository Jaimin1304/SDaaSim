import socket
import api_layer as api
import globals
import json

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((globals.ip, globals.port))
s.listen(5)

print('--- py server up ---')

while True:
    clientsocket, address = s.accept()

    msg = ''
    while True:
        chunk = clientsocket.recv(4096).decode('utf-8')
        if chunk.endswith('|'):  # The client has sent the end-of-message signal
            msg += chunk[:-1]  # Exclude the 'EOF' from the message
            break
        msg += chunk

    # exit if signal received
    if msg == 'exit':
        clientsocket.send(bytes('Server terminated', 'utf-8'))
        break

    # api.process_request(msg)
    # response = json.dumps(api.execute_user_logic())
    response = json.dumps(api.process_request(msg))

    clientsocket.send(bytes(response, 'utf-8'))
    clientsocket.close()
