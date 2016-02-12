class SpeechModule:
	def __init__(self, iterable):
		self.__run(iterable)

	def run(self):
		print("SpeechModule run() has to be overidden")

	__run = run
