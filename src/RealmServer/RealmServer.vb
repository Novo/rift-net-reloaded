Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Public Module RS_Main
    Public RS As RealmServerClass

    Class RealmServerClass
        Implements IDisposable

        Public _flagStopListen As Boolean = False
        Private Socket As Socket = Nothing
        Private IP As Net.IPAddress = Net.IPAddress.Parse("0.0.0.0")
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
                    RealmClient.Socket = ConnectionRealm.AcceptSocket

                    Dim NewThread As New Thread(AddressOf RealmClient.ProcessRealmClient)
                    NewThread.Start()
                End If
            Loop
        End Sub


        Public Sub ProcessRealmClient()
            IP = CType(Socket.RemoteEndPoint, IPEndPoint).Address
            RSPort = CType(Socket.RemoteEndPoint, IPEndPoint).Port

            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] Incoming Realm connection from [{1}:{2}]", Format(TimeOfDay, "hh:mm:ss"), IP, RSPort)
            Console.ForegroundColor = System.ConsoleColor.Gray


            Dim packet As System.IO.MemoryStream = New System.IO.MemoryStream()
            Dim writer As System.IO.BinaryWriter = New System.IO.BinaryWriter(packet)
            writer.Write(CByte(1))
            writer.Write(System.Text.Encoding.ASCII.GetBytes("|cFF00FFFF" & Config.RealmName))
            writer.Write(CByte(0))
            writer.Write(System.Text.Encoding.ASCII.GetBytes(Config.MMHost & ":" & Config.MMPort))
            writer.Write(CByte(0))
            writer.Write(CInt(0))
            SendRealmClient(packet.ToArray())
            Socket.Close()


            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] Realm Connection from [{1}:{2}] closed", Format(TimeOfDay, "hh:mm:ss"), IP, RSPort)
            Console.ForegroundColor = System.ConsoleColor.Gray

            Me.DisposeRealm()
        End Sub


        Public Sub SendRealmClient(ByVal data() As Byte)
            Try
                Dim i As Integer = Socket.Send(data, 0, data.Length, SocketFlags.None)

            Catch Err As Exception
                Console.ForegroundColor = System.ConsoleColor.Red
                Console.WriteLine("[{0}] Realm Connection from [{1}:{2}] do not exist - ERROR!!!", Format(TimeOfDay, "hh:mm:ss"), IP, RSPort)
                Console.ForegroundColor = System.ConsoleColor.Gray
                Socket.Close()
            End Try
        End Sub


        Public Sub OnRealmData(ByVal data() As Byte)
            'not used in Alpha ...
            Console.ForegroundColor = System.ConsoleColor.Red
            Console.WriteLine("[{0}] [{1}:{2}] Unknown Realm Opcode 0x{3}", Format(TimeOfDay, "hh:mm:ss"), IP, RSPort, data(0))
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Sub


        Public Sub DisposeRealm() Implements System.IDisposable.Dispose
            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] Realm Connection from [{1}:{2}] deleted", Format(TimeOfDay, "hh:mm:ss"), IP, RSPort)
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Sub


    End Class

End Module