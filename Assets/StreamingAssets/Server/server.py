import socket, api_layer, custom_algorithm

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind(('127.0.0.1', 1235))
s.listen(5)

print('--- py server up ---')

while True:
    clientsocket, address = s.accept()
    msg = clientsocket.recv(1024).decode('utf-8')
    # exit if signal received
    if msg == 'exit':
        clientsocket.send(bytes('Server terminated', 'utf-8'))
        break
    api_layer.update_state(msg)
    response = custom_algorithm.run()
    clientsocket.send(bytes(response, 'utf-8'))
    clientsocket.close()
