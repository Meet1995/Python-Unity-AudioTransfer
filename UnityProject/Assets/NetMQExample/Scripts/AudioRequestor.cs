using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;
using System;
using System.Text;
using WavUtils;

/// <summary>
///     Example of requester who only sends Hello. Very nice guy.
///     You can copy this class and modify Run() to suits your needs.
///     To use this class, you just instantiate, call Start() when you want to start and Stop() when you want to stop.
/// </summary>
public class AudioRequester : RunAbleThread
{       
    /// <summary>
    ///     Request Hello message to server and receive message back. Do it 10 times.
    ///     Stop requesting when Running=false.
    /// </summary>
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
                // ReceiveFrameString() blocks the thread until you receive the string, but TryReceiveFrameString()
                // do not block the thread, you can try commenting one and see what the other does, try to reason why
                // unity freezes when you use ReceiveFrameString() and play and stop the scene without running the server
//                string message = client.ReceiveFrameString();
//                Debug.Log("Received: " + message);
                bool gotMessage = false;
                byte[] messagebytes = null; 
                while (Running)
                {
                    gotMessage = client.TryReceiveFrameBytes(out messagebytes); // this returns true if it's successful
                    if (gotMessage) 
                    {
                        //messagebytes = Encoding.ASCII.GetBytes(message); 
                        //System.IO.File.WriteAllBytes("yourfilepath.wav", messagebytes);
                        //Debug.Log(messagebytes.Length);

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