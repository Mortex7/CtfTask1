using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

var hostName = "62.173.147.143";
var hostPort = 30100;
NetworkStream stream;

try
{
    var client = new TcpClient(hostName, hostPort);
    Console.WriteLine($"Connecting to: {hostName}:{hostPort}");
    stream = client.GetStream();
    ReadMessage();
    var response = SendMessage("start");
    var binaryData = ExtractBinaryNumberFromResponse(response);

    while (binaryData.Length > 1)
    {
        response = SendMessage(BinaryToString(binaryData));
        binaryData = ExtractBinaryNumberFromResponse(response);
    }

    Console.WriteLine();
    Console.ReadLine();
}
catch
{
    Console.WriteLine($"Failed while connecting to: {hostName}:{hostPort}");
    Environment.Exit(1);
}


string SendMessage(string command)
{
    var data = Encoding.ASCII.GetBytes(command + "\n");
    stream.Write(data, 0, data.Length);
    Console.WriteLine($"Sending : {command}");
    WaitShort();

    var response = ReadMessage();
    Console.WriteLine("Received : {0}", response);

    return response;
}

string ReadMessage()
{
    var responseData = new byte[256];
    var numberOfBytesRead = stream.Read(responseData, 0, responseData.Length);
    var response = Encoding.ASCII.GetString(responseData, 0, numberOfBytesRead);
    Console.WriteLine($"response:{Environment.NewLine} {response}");
    return response;
}


void WaitShort()
{
    Thread.Sleep(100);
}

string ExtractBinaryNumberFromResponse(string response)
{
    var pattern = @"(\s\b[01]+\b)";
    return Regex.Match(response, pattern).ToString().Trim();
}

static string BinaryToString(string binaryString)
{
    var result = new StringBuilder();
    for (var i = 0; i < binaryString.Length; i += 8)
    {
        var binaryByte = binaryString.Substring(i, 8);
        var asciiByte = Convert.ToByte(binaryByte, 2);
        result.Append((char)asciiByte);
    }

    return result.ToString();
}