using UnityEngine;
using System;
using System.Collections;
using System.IO;

namespace Mangrove
{
	[RequireComponent(typeof(AudioSource))]
	public class MicController : MonoBehaviour
	{
		public int NumFramesInBatch = 1;
		public int FrameLength = 256;
		private bool isStreamAllowed = false;
		private AudioPacket recentAudioPacket = null;
		private AudioPacket audioToBeRecorded = null;
		private string DEBUG_PREFIX = "[MicController]";

		public Mic _micSource;

		// private bool isSetup = false;
		public event Action<AudioPacket> OnAudioFrameCaptured;

		/// <summary>
		/// Singleton access
		/// </summary>
		private static MicController _instance;

		public static MicController Instance
		{
			get
			{
				if (_instance == null)
				{
					throw new Exception("MicController could not find the MicController object instance.");
				}

				return _instance;
			}
		}

		void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
				DontDestroyOnLoad(this.gameObject);
			}

			isStreamAllowed = false;
		}

		private void FixedUpdate()
		{
			// Debug.Log("Mic name:" + _micSource.CurrentDeviceName);
		}

		private void OnEnable()
		{
			if (_micSource == null)
			{
				_micSource = gameObject.GetComponentInChildren<Mic>();
			}

			if (_micSource != null)
			{
				_micSource.OnStartRecording += OnStartRecording;
				_micSource.OnSampleReady += OnSampleCaptured;
				_micSource.OnStopRecording += OnStopRecording;
			}
			else
			{
				Debug.Log($"{DEBUG_PREFIX} Mic source is null");
			}
		}

		public void Init()
		{
			StartCoroutine(InitMic());
		}

		private IEnumerator InitMic()
		{
			yield return new WaitForSeconds(3f);
			_micSource.StartRecording(FrameLength);
		}

		private void OnStartRecording()
		{
			Debug.Log($"{DEBUG_PREFIX} OnStartRecording");
		}

		private void OnStopRecording()
		{
			Debug.Log($"{DEBUG_PREFIX} OnStopRecording");
		}


		public void AllowStream()
		{
			isStreamAllowed = true;
		}

		public void BlockStream()
		{
			isStreamAllowed = false;
		}

		public void Dispose()
		{
			// Mic.Instance.Instance.StopRecording();
			if (_micSource != null)
			{
				if (_micSource.IsRecording)
				{
					_micSource.StopRecording();
				}

				_micSource.OnSampleReady -= OnSampleCaptured;
			}
		}


		void OnSampleCaptured(int sampleCount, float[] frame, float levelMax)
		{
			// Debug.Log($"{DEBUG_PREFIX} OnFrameCapturedPreCheck");
			if (!isStreamAllowed)
			{
				return;
			}

			// Debug.Log($"{DEBUG_PREFIX} OnFrameCaptured");

			// NOTE: Assuming one channel for the microphone
			AudioPacket newAudioPacket = new AudioPacket(
				frame, 1, 16000, AudioSettings.dspTime * 1000.0f
			);

			if (recentAudioPacket == null)
			{
				recentAudioPacket = newAudioPacket;
			}
			else
			{
				// Debug.Log("Adding to recentAudioPacket");
				recentAudioPacket += newAudioPacket;
			}

			// Debug.Log($"{DEBUG_PREFIX} timestamp of recent: = {recentAudioPacket.timestamp}");
			// Debug.Log($"{DEBUG_PREFIX} timestamp of new: = {newAudioPacket.timestamp}");
			if (recentAudioPacket.Length >= newAudioPacket.Length * NumFramesInBatch)
			{
				// handle sending to server elsewhere, but add to queue
				// ~20ms*NumFramesInBatch

				OnAudioFrameCaptured?.Invoke(recentAudioPacket);
				// RecordAudio(recentAudioPacket);
				recentAudioPacket = null; // Zero it out once sent
			}
		}


		private void RecordAudio(AudioPacket newAudioPacket)
		{
			if (audioToBeRecorded == null)
			{
				audioToBeRecorded = newAudioPacket;
			}
			else
			{
				audioToBeRecorded += newAudioPacket;
			}

			if (audioToBeRecorded.Length < newAudioPacket.Length * NumFramesInBatch * 500)
			{
				return;
			}

			Debug.Log($"{DEBUG_PREFIX} Saving audio to file after {NumFramesInBatch * 1000} frames in batch");

			string filename = "audio_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".wav";
			AudioClip clip = AudioClip.Create(
				"recording", audioToBeRecorded.Length,
				audioToBeRecorded.numChannels,
				audioToBeRecorded.sampleRate,
				false
			);
			clip.SetData(audioToBeRecorded.audio, 0);
			Save(filename, clip);

			audioToBeRecorded = null;
		}

		static FileStream CreateEmpty(string filepath)
		{
			var fileStream = new FileStream(filepath, FileMode.Create);
			byte emptyByte = new byte();

			for (int i = 0; i < 44; i++) //preparing the header
			{
				fileStream.WriteByte(emptyByte);
			}

			return fileStream;
		}

		public static bool Save(string filename, AudioClip clip)
		{
			if (!filename.ToLower().EndsWith(".wav"))
			{
				filename += ".wav";
			}

			var filepath = Path.Combine(Application.persistentDataPath, filename);

			Debug.Log(filepath);

			// Make sure directory exists if user is saving to sub dir.
			Directory.CreateDirectory(Path.GetDirectoryName(filepath));

			using (var fileStream = CreateEmpty(filepath))
			{

				ConvertAndWrite(fileStream, clip);

				WriteHeader(fileStream, clip);
			}

			return true; // TODO: return false if there's a failure saving the file
		}


		static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
		{
			var samples = new float[clip.samples];

			clip.GetData(samples, 0);

			Int16[] intData = new Int16[samples.Length];
			//converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

			Byte[] bytesData = new Byte[samples.Length * 2];
			//bytesData array is twice the size of
			//dataSource array because a float converted in Int16 is 2 bytes.

			int rescaleFactor = 32767; //to convert float to Int16

			for (int i = 0; i < samples.Length; i++)
			{
				intData[i] = (short)(samples[i] * rescaleFactor);
				Byte[] byteArr = new Byte[2];
				byteArr = BitConverter.GetBytes(intData[i]);
				byteArr.CopyTo(bytesData, i * 2);
			}

			fileStream.Write(bytesData, 0, bytesData.Length);
		}

		static void WriteHeader(FileStream fileStream, AudioClip clip)
		{

			var hz = clip.frequency;
			var channels = clip.channels;
			var samples = clip.samples;

			fileStream.Seek(0, SeekOrigin.Begin);

			Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
			fileStream.Write(riff, 0, 4);

			Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
			fileStream.Write(chunkSize, 0, 4);

			Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
			fileStream.Write(wave, 0, 4);

			Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
			fileStream.Write(fmt, 0, 4);

			Byte[] subChunk1 = BitConverter.GetBytes(16);
			fileStream.Write(subChunk1, 0, 4);

			// UInt16 two = 2;
			UInt16 one = 1;

			Byte[] audioFormat = BitConverter.GetBytes(one);
			fileStream.Write(audioFormat, 0, 2);

			Byte[] numChannels = BitConverter.GetBytes(channels);
			fileStream.Write(numChannels, 0, 2);

			Byte[] sampleRate = BitConverter.GetBytes(hz);
			fileStream.Write(sampleRate, 0, 4);

			Byte[]
				byteRate = BitConverter.GetBytes(hz * channels *
				                                 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
			fileStream.Write(byteRate, 0, 4);

			UInt16 blockAlign = (ushort)(channels * 2);
			fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

			UInt16 bps = 16;
			Byte[] bitsPerSample = BitConverter.GetBytes(bps);
			fileStream.Write(bitsPerSample, 0, 2);

			Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
			fileStream.Write(datastring, 0, 4);

			Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
			fileStream.Write(subChunk2, 0, 4);

			fileStream.Close();
		}
	}
}
