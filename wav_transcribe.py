#!/usr/bin/env python3

import speech_recognition as sr

from os import listdir

def transcribe(file):
	print("Decoderen: " + file)
	file = "wav/" + file;
	r = sr.Recognizer()
	with sr.WavFile(file) as source:
		audio = r.record(source)
	
	try:
		print(r.recognize_google(audio, language="nl"))
	except sr.UnknownValueError:
		print("Wav file is niet begrijpbaar.")
	except sr.RequestError:
		print("Geen connectie met Google Speech Recognition service")

for file in listdir("wav"):
	if file.endswith(".wav"):
		transcribe(file)

# obtain path to "test.wav" in the same folder as this script
#from os import path
#WAV_FILE = path.join(path.dirname(path.realpath(__file__)), "wav/hallo.wav")

# use "test.wav" as the audio source
#r = sr.Recognizer()
#with sr.WavFile(WAV_FILE) as source:
#    audio = r.record(source) # read the entire WAV file

# recognize speech using Google Speech Recognition
#try:
    # for testing purposes, we're just using the default API key
    # to use another API key, use `r.recognize_google(audio, key="GOOGLE_SPEECH_RECOGNITION_API_KEY")`
    # instead of `r.recognize_google(audio)`
#    print("Google Speech Recognition thinks you said " + r.recognize_google(audio))
#except sr.UnknownValueError:
#    print("Google Speech Recognition could not understand audio")
#except sr.RequestError:
#    print("Could not request results from Google Speech Recognition service")
