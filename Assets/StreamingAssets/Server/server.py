import socket

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind(('127.0.0.1', 1235))
s.listen(5)

print('--- py server up ---')

while True:
    clientsocket, address = s.accept()
    msg = clientsocket.recv(1024).decode('utf-8')
    if msg == 'exit':
        clientsocket.send(bytes('Server terminated', 'utf-8'))
        break
    print(f'New message from {address}: {msg}')
    clientsocket.send(bytes(f'echo: {msg}', 'utf-8'))
    clientsocket.close()
