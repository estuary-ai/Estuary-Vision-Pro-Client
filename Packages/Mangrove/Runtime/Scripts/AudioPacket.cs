using System;

namespace Mangrove
{
    [Serializable]
    public class IncomingAudioPacket
    {
        public byte[] bytes;
        public int sampleRate;
        public int numChannels;
        public int sampleWidth;
        // public int packetID;

        public String ToString()
        {
            return "Audio Bytes:  " + bytes + " Sample Rate: " + sampleRate + " Num Channels: " + numChannels +
                   " Sample Width: " + sampleWidth + " Length: " + bytes.Length;
            // + " Packet ID: " + packetID;
        }
    }


    [Serializable]
    public class AudioPacket
    {
        public float[] audio;
        public int numChannels;
        public int sampleRate;
        public int sampleWidth;
        public int timestamp; // miliseconds
        public long packetID;

        public int Length
        {
            get { return audio.Length; }
        }

        // public float[] FloatAudio {
        //     get {
        //         return ShortToFloat(audio);
        //     }
        // }

        public static long currentPacketID = 0;

        // TODO: check sampleWidth default for mic
        public AudioPacket(float[] audio, int numChannels, int sampleRate, double timestampMS, int sampleWidth = 2)
        {
            this.packetID = currentPacketID;
            currentPacketID += 1; // CLASS LEVEL
            this.audio = audio;
            this.numChannels = numChannels;
            this.sampleRate = sampleRate;
            this.sampleWidth = sampleWidth;
            this.timestamp = (int)timestampMS;
        }

        public AudioPacket(float[] audio, int numChannels, int sampleRate, double timestampMS, long packetID,
            int sampleWidth = 2)
        {
            this.packetID = packetID;
            this.audio = audio;
            this.numChannels = numChannels;
            this.sampleRate = sampleRate;
            this.sampleWidth = sampleWidth;
            this.timestamp = (int)timestampMS;
        }

        public static AudioPacket operator +(AudioPacket p1, AudioPacket p2)
        {
            // Assert.IsTrue(p1 < p2);
            float[] concatAudio = new float[p1.audio.Length + p2.audio.Length];
            p1.audio.CopyTo(concatAudio, 0);
            p2.audio.CopyTo(concatAudio, p1.audio.Length);

            return new AudioPacket(
                concatAudio,
                p1.numChannels, // Assuming same channel provided
                p1.sampleRate, // assuming same sample rate
                p1.timestamp, // timestamp is the first
                (p1.packetID + p2.packetID) / 2 // artifical packetID
            );
        }

        public static bool operator <(AudioPacket p1, AudioPacket p2)
        {
            return (p1.timestamp < p2.timestamp);
        }

        public static bool operator >(AudioPacket p1, AudioPacket p2)
        {
            return (p1.timestamp > p2.timestamp);
        }

        // private static float[] ShortToFloat(short[] source) {
        //     // float[] output = new float[source.Length];
        //     // for (int i = 0; i < source.Length; i++) {
        //     //     output[i] = (float)source[i] / short.MaxValue;
        //     // }
        //     // return output;

        //     byte[] target = new byte[source.Length * 2];
        //     Buffer.BlockCopy(source, 0, target, 0, source.Length * 2);
        //     return ByteToFloat(target);
        // }

        // private static float[] ByteToFloat(byte[] source) {
        //     int x = sizeof(Int16);
        //     int convertedSize = source.Length / x;
        //     float[] data = new float[convertedSize];
        //     Int16 maxValue = Int16.MaxValue;

        //     for (int i = 0; i < convertedSize; i++) {
        //         int offset = i * x;
        //         data[i] = (float) BitConverter.ToInt16(source, offset) / maxValue;
        //         ++i;
        //     }

        //     return data;
        // }

    }
}