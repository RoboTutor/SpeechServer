#!/usr/bin/env python3

# NOTE: this example requires PyAudio because it uses the Microphone class

import speech_recognition as sr

# obtain audio from the microphone
r = sr.Recognizer()
m = sr.Microphone()
with m as source:
    r.adjust_for_ambient_noise(source)
    print("Say something!")
    audio = r.listen(source)

# write audio to a WAV file
with open("wav/microphone-results.wav", "wb") as f:
    f.write(audio.get_wav_data())
