import socket

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

host = socket.gethostname()
port = 6969

s.connect((host, port))

data = ''

start = "start"
s.send(start.encode('ascii'))

while True:
	data = s.recv(1024)
	if data:
		data = data.decode('ascii')
		print("Ontvangen: ", data)
		s.send(start.encode('ascii'))
#	else:
#		print("Niks ontvangen")

#if data == "sta op":
#	confirm = "Confirmed"
#	s.send(confirm.encode('ascii'))
s.close()