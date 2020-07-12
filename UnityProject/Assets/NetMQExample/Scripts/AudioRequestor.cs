using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;
using System;
using System.Text;
using WavUtils;

public class AudioRequester : RunAbleThread
{       
protected override void Run()
    {  
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        using (RequestSocket client = new RequestSocket())
        {
            client.Connect("tcp://localhost:5555");

            for (int i = 0; i < 1 && Running; i++)
            {
                Debug.Log("Sending: Hello");
                client.SendFrame("Hello");

                bool gotMessage = false;
                byte[] messagebytes = null; 
                while (Running)
                {
                    gotMessage = client.TryReceiveFrameBytes(out messagebytes); // this returns true if it's successful
                    if (gotMessage) 
                    {
                        Dispatcher.ExecuteOnMainThread.Enqueue(() =>
                        {
                        WAV wav = new WAV(messagebytes);
                        Debug.Log(wav);
                        AudioClip myClip = AudioClip.Create("Answer",wav.SampleCount,1,wav.Frequency,false);
                        myClip.SetData(wav.LeftChannel,0);
                        AudioSource.PlayClipAtPoint(myClip, new Vector3(0, 0, 0), 1.0f);       
                        Debug.Log("This is a debug log called on the main thread!");
                        }); 
                        break;
                    }
                }
                if (gotMessage) 
                {
					Debug.Log("Received: audio");
				}
            }
        }
        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }
}
