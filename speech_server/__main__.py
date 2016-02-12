from speech_server.server import *
from speech_server.google_speech import *
from speech_server.sphinx4 import *

GS = GoogleSpeech()
#SP = Sphinx4()
S = Server('', 6969, GS)
S.run()
#S.close()
