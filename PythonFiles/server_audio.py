import time
import zmq

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")

print('Sending now..')

file = r'audio.wav'

with open(file, 'rb') as f:
    data = f.read()

while True:
    #  Wait for next request from client
    message = socket.recv()
    print("Received request: ", message)
    
    s_t = time.time()
    socket.send(data)
    print('Time taken: ',time.time()-s_t)
