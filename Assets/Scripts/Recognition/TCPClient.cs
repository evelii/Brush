using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using UnityEngine;

public class TCPClient : MonoBehaviour
{
	public DrawTubes tubes;
	public GameObject bestFitPlane;
	public RecognitionResult recognitionResult;
	public SketchEntity curObjectForRecognition;
	public string address;
	public int port;
	string result;
	string userChoice; // decision made by the user

	#region private members 	
	private TcpClient socketConnection;
	private Thread clientReceiveThread;
	#endregion

	public List<List<Vector3>> strokesList;

	// Use this for initialization 	
	void Start()
	{
		ConnectToTcpServer();
		result = "";
		userChoice = "";
	}

	// Update is called once per frame
	void Update()
	{
		if (OVRInput.GetDown(OVRInput.Button.Two))
		{
			Debug.LogWarning("Current sketch is done!");
			SendMessage();
			tubes.FinishSketch();
		}

		if (result != "")
        {
			Debug.LogWarning(result);  // result containing the top 2 results, separated by newline char
			string[] tokens = result.Split('\n');
			recognitionResult.ShowPredictionResults(tokens[0], tokens[1]);
			result = "";
		}

		if (userChoice != "")
        {
			curObjectForRecognition.ObjectIdentity(result);
			userChoice = "";
		}
	}

	/// <summary> 	
	/// Setup socket connection. 	
	/// </summary> 	
	private void ConnectToTcpServer()
	{
		try
		{
			clientReceiveThread = new Thread(new ThreadStart(ListenForData));
			clientReceiveThread.IsBackground = true;
			clientReceiveThread.Start();
		}
		catch (Exception e)
		{
			Debug.Log("On client connect exception " + e);
		}
	}

	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	private void ListenForData()
	{
		try
		{
			socketConnection = new TcpClient(address, port);
			Byte[] bytes = new Byte[1024];
			while (true)
			{
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream())
				{
					int length;
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						// Convert byte array to string message. 						
						string serverMessage = Encoding.ASCII.GetString(incommingData);
						Debug.Log("server message received as: " + serverMessage);
						result = serverMessage;
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}

	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	private void SendMessage()
	{
		string clientMessage = "This is a message from one of your clients.";

		FindPlane findPlane = bestFitPlane.GetComponent<FindPlane>();
		clientMessage = findPlane.GetTranslatedPoints();
		WriteString(clientMessage);

		if (socketConnection == null)
		{
			return;
		}
		try
		{
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream();
			if (stream.CanWrite)
			{
				// Convert string message to byte array.
				
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
				Debug.Log("Client sent his message - should be received by server");
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}

	private void WriteString(string content)
	{
		string path = "Assets/Resources/Points.txt";
		File.WriteAllText(path, String.Empty);  // clear the old content
		//Write some text to the test.txt file
		StreamWriter writer = new StreamWriter(path, true);
		writer.WriteLine(content);
		writer.Close();
		StreamReader reader = new StreamReader(path);
		//Print the text from the file
		Debug.Log(reader.ReadToEnd());
		reader.Close();
	}
}