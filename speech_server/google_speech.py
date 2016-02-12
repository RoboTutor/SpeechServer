import speech_recognition as sr
from speech_server.speech_module import *

class GoogleSpeech(SpeechModule):
	def __init__(self):
		self.recognizer = sr.Recognizer()
		self.mic = sr.Microphone()

	def adjustNoise(self, source):
		print("Moment van stilte aub...")
		self.recognizer.adjust_for_ambient_noise(source)
		#print("Minimale drempelwaarde: {}".format(self.recognizer.energy_threshold))

	def listen(self, source):
		print("\nSpreek iets in.")
		return self.recognizer.record(source, duration=5)
		#return self.recognizer.listen(source)

	def transcribe(self, audio):
		print("Gehoord! Nu even herkennen...")
		try:
			value = self.recognizer.recognize_google(audio, language="nl") #en-US or nl
			if str is bytes: # Python2 (bytes strings)
				value = value.encode("utf-8")
			return value
		except sr.UnknownValueError:
			print("Niet begrepen, wacht even om opnieuw te proberen.")
			return False
		except sr.RequestError:
			print("Kan niet verbinden met Google Speech Recognition service")
			return False

	def run(self):
		with self.mic as source:
			self.adjustNoise(source)
			data = False
			while data == False:
				audio = self.listen(source)
				data = self.transcribe(audio)
			return data

#S = GoogleSpeech()
#S.run()
