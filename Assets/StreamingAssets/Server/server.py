import socket
import api_layer
import custom_algorithm

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind(('127.0.0.1', 1235))
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
    print(msg)

    # exit if signal received
    if msg == 'exit':
        clientsocket.send(bytes('Server terminated', 'utf-8'))
        break

    api_layer.update_state(msg)
    response = custom_algorithm.run()
    clientsocket.send(bytes(msg, 'utf-8'))
    clientsocket.close()
