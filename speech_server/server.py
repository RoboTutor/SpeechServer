import socket

class Server:
	def __init__(self, h, p, sm):
		self.host = h
		self.port = p
		self.speech_module = sm
		self.s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
		self.listen()
		self.connect()

	def listen(self):
		self.s.bind((self.host, self.port))
		self.s.listen(1)

	def connect(self):
		self.conn, addr = self.s.accept()
		if self.conn:
			print("Connected to: ", addr)

	def getData(self):
		while True:
			data = self.conn.recv(1024).decode('ascii')
			if data:
				return data

	def sendData(self):
		data = self.startSpeechModule()
		if data:
			print("Sending data...")
			print(data)
			self.conn.send(data.encode('ascii'))
			print("Data sent")

	def startSpeechModule(self):
		data = self.speech_module.run()
		return data

	def run(self):
		while True:
			data = self.getData()
			if data == "start":
				self.sendData()
			elif data == "stop":
				self.close()

	def close(self):
		self.conn.close()
		self.s.close()
		print("Connection terminated.")

#S = Server('', 6969)
#S.run()
#S.close()
