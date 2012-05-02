Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Public Module RS_Main
    Public RS As RealmServerClass

    Class RealmServerClass
        Implements IDisposable

        Public _flagStopListen As Boolean = False
        Private RealmSocket As Socket = Nothing
        Private RSIP As Net.IPAddress = Net.IPAddress.Parse("0.0.0.0")
        Private RSPort As Int32 = Config.RSPort
        Private lstHostRealm As Net.IPAddress = Net.IPAddress.Parse(Config.RSHost)
        Private ConnectionRealm As TcpListener


        Public Sub start()
            Console.WriteLine("[{0}] Realm Server Starting...", Format(TimeOfDay, "hh:mm:ss"))
            Try
                ConnectionRealm = New TcpListener(lstHostRealm, RSPort)
                ConnectionRealm.Start()
                Dim RSListenThread As Thread
                RSListenThread = New Thread(AddressOf AcceptConnectionRealm)
                RSListenThread.Name = "Realm Server, Listening"
                RSListenThread.Start()

                Console.WriteLine("[{0}] Realm Server Listening on {1} on port {2}", Format(TimeOfDay, "hh:mm:ss"), lstHostRealm, RSPort)
            Catch e As Exception
                Console.WriteLine()
                Console.ForegroundColor = System.ConsoleColor.Red
                Console.WriteLine("[{0}] Error in {2}: {1}.", Format(TimeOfDay, "hh:mm:ss"), e.Message, e.Source)
                Console.ForegroundColor = System.ConsoleColor.Gray
            End Try
        End Sub


        Public Sub AcceptConnectionRealm()
            Do While Not _flagStopListen
                Thread.Sleep(100)
                If ConnectionRealm.Pending() Then
                    Dim RealmClient As New RealmServerClass
                    RealmClient.RealmSocket = ConnectionRealm.AcceptSocket

                    Dim NewThread As New Thread(AddressOf RealmClient.ProcessRealmClient)
                    NewThread.Start()
                End If
            Loop
        End Sub


        Public Sub ProcessRealmClient()
            RSIP = CType(RealmSocket.RemoteEndPoint, IPEndPoint).Address
            RSPort = CType(RealmSocket.RemoteEndPoint, IPEndPoint).Port

            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] Incoming Realm connection from [{1}:{2}]", Format(TimeOfDay, "hh:mm:ss"), RSIP, RSPort)
            Console.ForegroundColor = System.ConsoleColor.Gray

            Dim realmWriter As New PacketWriter()
            realmWriter.WriteUInt8(1)
            realmWriter.WriteBytes(System.Text.Encoding.ASCII.GetBytes("|cFF00FFFF" & Config.RealmName))
            realmWriter.WriteUInt8(0)
            realmWriter.WriteBytes(System.Text.Encoding.ASCII.GetBytes(Config.MMHost & ":" & Config.MMPort))
            realmWriter.WriteUInt8(0)
            realmWriter.WriteUInt32(0)

            SendRealmClient(realmWriter)
        End Sub


        Public Sub SendRealmClient(ByVal packet As PacketWriter)

            If packet Is Nothing Then Throw New ApplicationException("Realm Packet doesn't contain data!")
            SyncLock Me
                Try
                    Dim buffer As Byte() = packet.ReadDataToSend(True)

                    'Send Data sync:
                    Dim bytesSent As Integer = 0
                    'bytesSent = Socket.Send(buffer, 0, buffer.Length, SocketFlags.None)
                    'Console.WriteLine("[{0}:{1}] Realm Data sent {2} bytes, opcode={3}", IP, Port, i, packet.Opcode)

                    'Send Data async
                    RealmSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, New AsyncCallback(AddressOf AsyncSendRealmClientCallback), RealmSocket)


                    Dim PacketLogger As New PacketLog
                    PacketLogger.DumpPacket(buffer, ">>")


                Catch Err As Exception
                    Console.ForegroundColor = System.ConsoleColor.Red
                    Console.WriteLine("[{0}] Realm Connection from [{1}:{2}] do not exist - ERROR!!!", Format(TimeOfDay, "hh:mm:ss"), RSIP, RSPort)
                    Console.ForegroundColor = System.ConsoleColor.Gray
                    RealmSocket.Close()
                End Try
            End SyncLock

        End Sub


        Public Sub AsyncSendRealmClientCallback(ByVal result As IAsyncResult)

            Try
                Dim RealmSocket As Socket = TryCast(result.AsyncState, Socket)
                Dim bytesSent As Integer = 0
                bytesSent = RealmSocket.EndSend(result)

                'Console.WriteLine("[{0}:{1}] Realm Data sent {2} bytes, opcode={3}", RSIP, RSPort, bytesSent, packet.Opcode)
                Console.WriteLine("[{0}:{1}] Realm Data sent {2} bytes", RSIP, RSPort, bytesSent)

                RealmSocket.Close()

                Console.ForegroundColor = System.ConsoleColor.DarkGray
                Console.WriteLine("[{0}] Realm Connection from [{1}:{2}] closed", Format(TimeOfDay, "hh:mm:ss"), RSIP, RSPort)
                Console.ForegroundColor = System.ConsoleColor.Gray

                Me.DisposeRealm()

            Catch socketException As SocketException
                Console.WriteLine("Realm Connection from [{0}:{1}] cause error {3}{4}", RSIP, RSPort, socketException.Message.ToString, vbNewLine)
            End Try

        End Sub


        Public Sub OnRealmData(ByVal data() As Byte)
            'not used in Alpha ...
            Console.ForegroundColor = System.ConsoleColor.Red
            Console.WriteLine("[{0}] [{1}:{2}] Unknown Realm Opcode 0x{3}", Format(TimeOfDay, "hh:mm:ss"), RSIP, RSPort, data(0))
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Sub


        Public Sub DisposeRealm() Implements System.IDisposable.Dispose
            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] Realm Connection from [{1}:{2}] deleted", Format(TimeOfDay, "hh:mm:ss"), RSIP, RSPort)
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Sub


    End Class

End Module