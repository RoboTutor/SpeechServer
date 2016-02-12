from speech_server.speech_module import *

class Sphinx4(SpeechModule):
	def __init__(self):
		self.data = "Can you hear me"

	def run(self):
		while True:
			return self.data

#S = Sphinx4()
#S.run()
