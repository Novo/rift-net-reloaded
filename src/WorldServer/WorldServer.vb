Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Text

Public Module WS_Main
    Public WS As WorldServerClass
    Public Log As New BaseWriter
    Public PacketHandlers As New Dictionary(Of OpCodes, HandlePacket)
    Delegate Sub HandlePacket(ByRef Packet As PacketReader, ByRef Client As WorldServerClass)

    Class WorldServerClass
        Implements IDisposable

		Public _flagStopListen As Boolean = False
        Public Character As CharacterObject = Nothing
        Private WorldServerSocket As Socket = Nothing
        Private ConnectionWorld As TcpListener = Nothing
        Public WSIP As Net.IPAddress = Net.IPAddress.Parse("0.0.0.0")
        Public WSPort As Int32 = Config.WSPort
        Private lstHostWorld As Net.IPAddress = Net.IPAddress.Parse(Config.WSHost)



        Public Sub Start()
            Console.WriteLine("[{0}] World Server Starting...", Format(TimeOfDay, "HH:mm:ss"))

            Try
                ConnectionWorld = New TcpListener(lstHostWorld, WSPort)
                ConnectionWorld.Start()

                Dim WSListenThread As Thread
                WSListenThread = New Thread(AddressOf AcceptConnectionWorld)
                WSListenThread.Name = "World Server Listening"
                WSListenThread.Start()

                Console.WriteLine("[{0}] World Server Listening on {1} on port {2}", Format(TimeOfDay, "HH:mm:ss"), lstHostWorld, WSPort)
            Catch e As Exception
                Console.WriteLine()
                Console.ForegroundColor = System.ConsoleColor.Red
                Console.WriteLine("[{0}] Error in {2}: {1}.", Format(TimeOfDay, "HH:mm:ss"), e.Message, e.Source)
                Console.ForegroundColor = System.ConsoleColor.Gray
            End Try
        End Sub


        Public Sub AcceptConnectionWorld()
            Do While Not _flagStopListen
                Thread.Sleep(100)
                If ConnectionWorld.Pending() Then

                    Dim WorldClient As New WorldServerClass 'Create a new World for every new Connection
                    WorldClient.Character = New CharacterObject(WorldClient) 'Create a new CharacterObject for every new World

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
            Console.WriteLine("[{0}] New incoming World connection from [{1}:{2}]", Format(TimeOfDay, "HH:mm:ss"), WSIP, WSPort)
            Console.ForegroundColor = System.ConsoleColor.Gray


            Dim response As New PacketWriter(OpCodes.SMSG_AUTH_CHALLENGE, False)
            response.WriteUInt8(&H0)
            response.WriteUInt8(&H0)
            response.WriteUInt8(&H0)
            response.WriteUInt8(&H0)
            response.WriteUInt8(&H0)
            response.WriteUInt8(&H0)
            SendWorldClient(response)


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
            Console.WriteLine("[{0}] World Connection from [{1}:{2}] closed", Format(TimeOfDay, "HH:mm:ss"), WSIP, WSPort)
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

                    Console.WriteLine("[{0}] [{1}:{2}] >> OpCode {3}({4}), Length:{5}", Format(TimeOfDay, "HH:mm:ss"), WSIP, WSPort, packet.Opcode.ToString, Val(packet.Opcode).ToString, packet.Size.ToString)

                    Dim PacketLogger As New PacketLog
                    PacketLogger.DumpPacket(buffer, ">>")

                Catch ex As Exception
                    Console.WriteLine("[{0}] [{1}:{2}] World Connection cause error {3}", Format(TimeOfDay, "HH:mm:ss"), WSIP, WSPort, ex.ToString & Environment.NewLine)
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

                Console.WriteLine("[{0}] [{1}:{2}] World Data sent {3} bytes", Format(TimeOfDay, "HH:mm:ss"), WSIP, WSPort, bytesSent)
            Catch socketException As SocketException
                Console.WriteLine("[{0}] [{1}:{2}] World Connection cause error {3}", Format(TimeOfDay, "HH:mm:ss"), WSIP, WSPort, socketException.ToString & Environment.NewLine)
            End Try

        End Sub


        Public Sub OnWorldData(ByVal data() As Byte)
            Dim PacketBuffer As New PacketReader(data)

            Try

                If [Enum].IsDefined(GetType(OpCodes), PacketBuffer.Opcode) Then
                    Console.WriteLine("[{0}] [{1}:{2}] << OpCode: {3}({4}), Length:{5}", Format(TimeOfDay, "HH:mm:ss"), WSIP, WSPort, PacketBuffer.Opcode, Val(PacketBuffer.Opcode).ToString, PacketBuffer.Size.ToString)

                    If PacketHandlers.ContainsKey(PacketBuffer.Opcode) = True Then
                        PacketHandlers(PacketBuffer.Opcode).Invoke(PacketBuffer, Me)
                    End If

                Else
                    Console.WriteLine("[{0}] [{1}:{2}] << Unknown OpCode: {3}, Length:{4}", Format(TimeOfDay, "HH:mm:ss"), WSIP, WSPort, PacketBuffer.Opcode, PacketBuffer.Size)
                End If


                Dim PacketLogger As New PacketLog
                PacketLogger.DumpPacket(data, "<<")

            Catch ex As Exception
                Console.WriteLine("[{0}] [{1}:{2}] World Connection cause error {3}, {4}", Format(TimeOfDay, "HH:mm:ss"), WSIP, WSPort, PacketBuffer.Opcode, Environment.NewLine & ex.ToString)
            End Try

        End Sub


        Public Sub DisposeWorld() Implements System.IDisposable.Dispose
            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] World Connection from [{1}:{2}] deleted", Format(TimeOfDay, "HH:mm:ss"), WSIP, WSPort)
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Sub


    End Class

End Module