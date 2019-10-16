import math , pywt , numpy as np
import csv
import sys
import os
import xml.etree.ElementTree

from datetime import datetime

import zmq
from msgpack import loads

import time
from threading import Thread

import keyboard
import datetime

from socket import *
import json

host = '255.255.255.255'
port = 50000

confidenceThreshold = .2
windowLengthSeconds = 60
maxSamplingRate = 120
minSamplesPerWindow = maxSamplingRate * windowLengthSeconds
wavelet = 'sym8'
# wavelet = 'sym16'

class ProcessingThread (Thread):
   def __init__(self, pupilData, targetsocket):
      Thread.__init__(self)
      self.data = pupilData
      self.targetsocket = targetsocket

   def run(self):
	   global threadRunning
	   processData(self.data, self.targetsocket)
	   threadRunning = False

class PupilData(float):
	def __init__(self, dia):
		self.X = dia
		self.timestamp = 0
		self.confidence = 0

def ipa(d):
	# obtain 2-level DWT of pupil diameter signal d
	try:
		(cA2,cD2,cD1) = pywt.wavedec(d,wavelet,'per',level=2)
		# (cA2,cD2,cD1) = pywt.wavedec(d, 'db8','per',level=2)
	except ValueError:
		return

	# get signal duration (in seconds)
	tt = d[-1].timestamp - d[0].timestamp
	# print("timestamp", tt)

	#using data from Pedrotti et al
	# tt = 1.0

	# normalize by 1=2j , j = 2 for 2-level DWT
	cA2[:] = [x / math.sqrt(4.0) for x in cA2]
	cD1[:] = [x / math.sqrt(2.0) for x in cD1]
	cD2[:] = [x / math.sqrt(4.0) for x in cD2]

	# detect modulus maxima , see Listing 2
	cD2m = modmax(cD2)

	# threshold using universal threshold lambda_univ = s*sqrt(p(2 log n))
	lambda_univ = np.std(cD2m) * math.sqrt(2.0 * np.log2(len(cD2m)))
	# where s is the standard deviation of the noise
	cD2t = pywt.threshold(cD2m ,lambda_univ, mode="hard")

	# compute IPA
	ctr = 0
	for i in range(len(cD2t)):
		# print(cD2t[i])
		if math.fabs(cD2t[i]) > 0:
			ctr += 1

	# print(ctr)
	IPA = float(ctr)/tt

	return IPA

def modmax(d):
	# compute signal modulus
	m = [0.0] * len(d)
	for i in range(len(d)):
		m[i] = math.fabs(d[i])

	# if value is larger than both neighbours , and strictly
	# larger than either , then it is a local maximum
	t = [0.0]*len(d)

	for i in range(len(d)):
		ll = m[i-1] if i >= 1 else m[i]
		oo = m[i]
		rr = m[i+1] if i < len(d)-2 else m[i]

		if (ll <= oo and oo >= rr) and (ll < oo or oo > rr):
			# compute magnitude
			t[i] = math.sqrt(d[i]**2)
		else:
			t[i] = 0.0

	return t

def createSendSocket():
	backlog = 5
	size = 1024
	sock = socket(AF_INET, SOCK_DGRAM)
	sock.setsockopt(SOL_SOCKET, SO_REUSEADDR, 1)
	sock.setsockopt(SOL_SOCKET, SO_BROADCAST, 1)
	return sock

def createPupilConnection():
	context = zmq.Context()
	# open a req port to talk to pupil
	addr = '127.0.0.1'  # remote ip or localhost
	req_port = "50020"  # same as in the pupil remote gui
	req = context.socket(zmq.REQ)
	req.connect("tcp://{}:{}".format(addr, req_port))
	# ask for the sub port
	req.send_string('SUB_PORT')
	sub_port = req.recv_string()

	# open a sub port to listen to pupil
	sub = context.socket(zmq.SUB)
	sub.connect("tcp://{}:{}".format(addr, sub_port))

	sub.setsockopt_string(zmq.SUBSCRIBE, 'pupil.')

	return sub

def cleanup(old_data):
	stddev = np.std(old_data)
	mean = np.mean(old_data)

	filtered = []
	runner = 0.0

	for i in range(len(old_data)):
		currentData = PupilData(old_data[i].X)
		currentData.timestamp = old_data[i].timestamp
		distanceToMean = abs(currentData.X - mean)

		if(distanceToMean < stddev * 2):
			filtered.append(currentData)
			runner += 1

	# print(str(stddev) + " / " + str(mean) + ' / ' + str(len(filtered)))

	return filtered

def cleanBlinks(data):
	blinks = []

	minNumForBlinks = 2
	numSamples = len(data)
	i = 0
	minConfidence = .25

	while (i < numSamples):
		if(data[i].confidence < minConfidence and i < numSamples - 1):
			runner = 1
			nextData = data[i + runner]
			while(nextData.confidence < minConfidence):
				runner = runner + 1

				if(i + runner >= numSamples):
					break

				nextData = data[i + runner]

			if(runner >= minNumForBlinks):
				blinks.append((i, runner))

			i = i + runner
		else:
			i = i + 1

	durationsSampleRemoveMS = 200
	numSamplesRemove = int(math.ceil(120 / (1000 / durationsSampleRemoveMS)))

	blinkMarkers = np.ones(numSamples)
	for i in range(len(blinks)):
		blinkIndex = blinks[i][0]
		blinkLength = blinks[i][1]

		for j in range(0, blinkLength):
			blinkMarkers[blinkIndex + j] = 0

		for j in range(0, numSamplesRemove):
			decrementIndex = blinkIndex - j
			incrementIndex = blinkIndex + blinkLength +j

			if(decrementIndex >= 0):
				blinkMarkers[decrementIndex] = 0

			if(incrementIndex < numSamples):
				blinkMarkers[incrementIndex] = 0

	newSamplesList = []

	for i in range(0, numSamples):
		if(blinkMarkers[i] == 1):
			newSamplesList.append(data[i])

	return newSamplesList

def fixTimestamp(data):
	runner = 0.0
	for i in range(len(data)):
		data[i].timestamp = runner / 120.0
		runner  += 1

def processData(data, socket):

	blinkedRemoved = cleanBlinks(data)
	cleanedData = cleanup(blinkedRemoved)
	fixTimestamp(cleanedData)
	currentIPA = ipa(cleanedData)

	valueString = ' ipa '+ str(currentIPA)
	#print (str(datetime.datetime.now()) + '  ' + valueString + '; ' + str(len(cleanedData))  + ' / '+ str(len(data)) + ' samples')
	socket.sendto(str.encode(valueString), (host, port))

def receivePupilData(udp, pupilSocket):
	while True:
		try:
			topic = pupilSocket.recv_string()
			msg = pupilSocket.recv()
			msg = loads(msg, encoding='utf-8')
			#print("\n{}: {}".format(topic, msg))

			data = PupilData(msg['diameter'])
			data.timestamp = msg['timestamp']
			data.confidence =  msg['confidence']

			currentPupilData.append(data)

			while len(currentPupilData) > minSamplesPerWindow:
				currentPupilData.pop(0)

			global threadRunning

			if(len(currentPupilData) == minSamplesPerWindow and threadRunning is False):
				threadRunning = True
				processingThread = ProcessingThread(list(currentPupilData), udp)
				processingThread.start()

		except KeyboardInterrupt:
			break

threadRunning = False
currentPupilData = list()

print(datetime.datetime.now())
socket = createSendSocket()
pupilSocket = createPupilConnection()

receivePupilData(socket, pupilSocket)
