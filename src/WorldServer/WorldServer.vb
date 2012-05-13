Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Text

Public Module WS_Main
    Public WS As WorldServerClass
    Public Log As New BaseWriter
    Public PacketHandlers As New Hashtable
    Delegate Sub HandlePacket(ByRef Packet As PacketReader, ByRef Client As WorldServerClass)


    Class WorldServerClass
        Implements IDisposable

        Public Character As CharacterObject = Nothing

        Public _flagStopListen As Boolean = False
        Private WorldServerSocket As Socket = Nothing
        Public WSIP As Net.IPAddress = Net.IPAddress.Parse("0.0.0.0")
        Public WSPort As Int32 = 8086
        Private lstHostWorld As Net.IPAddress = Net.IPAddress.Parse("127.0.0.1")
        Private ConnectionWorld As TcpListener


        Public Sub Start()
            Console.WriteLine("[{0}] World Server Starting...", Format(TimeOfDay, "hh:mm:ss"))
            Dim lstHostWorld As Net.IPAddress = Net.IPAddress.Parse("127.0.0.1")
            Try
                ConnectionWorld = New TcpListener(lstHostWorld, WSPort)
                ConnectionWorld.Start()

                Dim WSListenThread As Thread
                WSListenThread = New Thread(AddressOf AcceptConnectionWorld)
                WSListenThread.Name = "World Server Listening"
                WSListenThread.Start()

                Console.WriteLine("[{0}] World Server Listening on {1} on port {2}", Format(TimeOfDay, "hh:mm:ss"), lstHostWorld, WSPort)
            Catch e As Exception
                Console.WriteLine()
                Console.ForegroundColor = System.ConsoleColor.Red
                Console.WriteLine("[{0}] Error in {2}: {1}.", Format(TimeOfDay, "hh:mm:ss"), e.Message, e.Source)
                Console.ForegroundColor = System.ConsoleColor.Gray
            End Try
        End Sub


        Public Sub AcceptConnectionWorld()
            Do While Not _flagStopListen
                Thread.Sleep(100)
                If ConnectionWorld.Pending() Then
                    Dim WorldClient As New WorldServerClass
                    WorldClient.WorldServerSocket = ConnectionWorld.AcceptSocket

                    Dim NewThread As New Thread(AddressOf WorldClient.ProcessWorld)
                    NewThread.Start()
                End If
            Loop
        End Sub


        Public Sub ProcessWorld()
            WSIP = CType(WorldServerSocket.RemoteEndPoint, IPEndPoint).Address
            WSPort = CType(WorldServerSocket.RemoteEndPoint, IPEndPoint).Port

            Dim Buffer() As Byte
            Dim bytes As Integer

            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] New incoming World connection from [{1}:{2}]", Format(TimeOfDay, "hh:mm:ss"), WSIP, WSPort)
            Console.ForegroundColor = System.ConsoleColor.Gray


            Dim response As New PacketWriter(OpCodes.SMSG_AUTH_CHALLENGE, False)
            response.WriteUInt8(&H0)
            response.WriteUInt8(&H0)
            response.WriteUInt8(&H0)
            response.WriteUInt8(&H0)
            response.WriteUInt8(&H0)
            response.WriteUInt8(&H0)
            SendWorldClient(response)

            Console.WriteLine("Successfully sent: SMSG_AUTH_CHALLENGE")


            While Not _flagStopListen
                Thread.Sleep(100)
                If WorldServerSocket.Available > 0 Then
                    ReDim Buffer(WorldServerSocket.Available - 1)
                    bytes = WorldServerSocket.Receive(Buffer, Buffer.Length, 0)

                    OnWorldData(Buffer)
                End If
                If Not WorldServerSocket.Connected Then Exit While
                If (WorldServerSocket.Poll(100, SelectMode.SelectRead)) And (WorldServerSocket.Available = 0) Then Exit While
            End While

            WorldServerSocket.Close()


            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] World Connection from [{1}:{2}] closed", Format(TimeOfDay, "hh:mm:ss"), WSIP, WSPort)
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Sub


        Public Sub SendWorldClient(ByVal packet As PacketWriter)
            If packet Is Nothing Then Throw New ApplicationException("World Packet doesn't contain data!")

            SyncLock Me

                Try
                    Dim buffer As Byte() = packet.ReadDataToSend()

                    'Send Data sync:
                    'Dim bytesSent As Integer = 0
                    'bytesSent = Socket.Send(buffer, 0, buffer.Length, SocketFlags.None)
                    'Console.WriteLine("[{0}:{1}] World Data sent {2} bytes, opcode={3}", IP, Port, i, packet.Opcode)

                    'Send Data async
                    WorldServerSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, New AsyncCallback(AddressOf AsyncSendWorldClientCallback), WorldServerSocket)


                    Dim PacketLogger As New PacketLog
                    PacketLogger.DumpPacket(buffer, ">>")

                Catch ex As Exception
                    Console.WriteLine("World Connection from [{0}:{1}] cause error {3}", WSIP, WSPort, ex.ToString & Environment.NewLine)
                End Try

            End SyncLock
        End Sub

        Public Sub AsyncSendWorldClientCallback(ByVal result As IAsyncResult)

            Try
                Dim WorldServerSocket As Socket = TryCast(result.AsyncState, Socket)
                Dim bytesSent As Integer = 0
                bytesSent = WorldServerSocket.EndSend(result)

                'WorldServerSocket.Close()
                'Me.DisposeWorld()

                Console.WriteLine("[{0}:{1}] World Data sent {2} bytes", WSIP, WSPort, bytesSent)

            Catch socketException As SocketException
                Console.WriteLine("World Connection from [{0}:{1}] cause error {3}", WSIP, WSPort, socketException.Message.ToString & Environment.NewLine)
            End Try

        End Sub


        Public Sub OnWorldData(ByVal data() As Byte)
            Dim PacketBuffer As New PacketReader(data)
            
            Try
                If [Enum].IsDefined(GetType(OpCodes), PacketBuffer.Opcode) Then
                    Console.WriteLine("Recieved OpCode: {0}, Length: {1}", PacketBuffer.Opcode, PacketBuffer.Size)
                    PacketHandlers(PacketBuffer.Opcode).Invoke(PacketBuffer, Me)
                Else
                    Console.WriteLine("Received unknown OpCode: {0}, Length: {1}", PacketBuffer.Opcode, PacketBuffer.Size)
                End If

                Dim PacketLogger As New PacketLog
                PacketLogger.DumpPacket(data, "<<")


            Catch ex As Exception
                Console.WriteLine("World Connection from [{0}:{1}] caused an error {2}{3}", WSIP, WSPort, Err.ToString, vbNewLine)
                Console.WriteLine("Opcode Handler {2}:{3} caused an error:{1}{0}", ex.Message, vbNewLine, PacketBuffer.Opcode, CType(PacketBuffer.Opcode, OpCodes))
            End Try

        End Sub


        Public Sub DisposeWorld() Implements System.IDisposable.Dispose
            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] World Connection from [{1}:{2}] deleted", Format(TimeOfDay, "hh:mm:ss"), WSIP, WSPort)
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Sub


    End Class

End Module