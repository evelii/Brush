using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClient : MonoBehaviour
{

	public string address;
	public int port;

	#region private members 	
	private TcpClient socketConnection;
	private Thread clientReceiveThread;
	#endregion

	public List<List<Vector3>> strokesList;

	// Use this for initialization 	
	void Start()
	{
		ConnectToTcpServer();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Debug.LogWarning("press!");
			SendMessage();
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
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}

	private string prepareData()
    {
		string res = "";
		foreach (List<Vector3> strokes in strokesList)
		{
			string strokeStr = "";
			for (int i = 0; i < strokes.Count; i++)
			{
				Vector2 v2 = strokes[i];
				strokeStr += v2.ToString();
				if (i != strokes.Count - 1) strokeStr += ",";

			}
			strokeStr = strokeStr.Replace(" ", "");
			Debug.LogError(strokeStr);
			strokeStr += "#"; // delimiter of different strokes
			res += strokeStr;
		}
		return res;
	}

	private string prepareDataBestFit()
	{
		FindPlane findPlane = GameObject.Find("BestFitPlane").GetComponent<FindPlane>();
		List<Vector2> translatedPoints = findPlane.getTranslatedPoints();

		string res = "";
		for (int i = 0; i < translatedPoints.Count; i++)
		{
			res += translatedPoints[i].ToString();
			if (i != translatedPoints.Count - 1) res += ",";

		}

		return res;
	}

	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	private void SendMessage()
	{
		string clientMessage = "This is a message from one of your clients.";

		clientMessage = prepareDataBestFit();
		clientMessage = clientMessage.Replace(" ", "");

		Debug.Log(clientMessage);

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
}