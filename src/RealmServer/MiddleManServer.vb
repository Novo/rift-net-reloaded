Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Public Module MS_Main
    Public MS As MiddleManServerClass

    Class MiddleManServerClass
        Implements IDisposable

        Private _flagStopListen As Boolean = False
        Private Socket As Socket = Nothing
        Private IP As Net.IPAddress = Net.IPAddress.Parse("0.0.0.0")
        Private MMPort As Int32 = Config.MMPort
        Private lstHostMiddleMan As Net.IPAddress = Net.IPAddress.Parse(Config.MMHost)
        Private ConnectionMiddleMan As TcpListener


        Public Sub Start()
            Console.WriteLine("[{0}] MiddleMan Server Starting...", Format(TimeOfDay, "hh:mm:ss"))
            Dim lstHostMiddleMan As Net.IPAddress = Net.IPAddress.Parse("127.0.0.1")
            Try
                ConnectionMiddleMan = New TcpListener(lstHostMiddleMan, MMPort)
                ConnectionMiddleMan.Start()

                Dim MSListenThread As Thread
                MSListenThread = New Thread(AddressOf AcceptConnectionMiddleMan)
                MSListenThread.Name = "MiddleMan Server, Listening"
                MSListenThread.Start()

                Console.WriteLine("[{0}] MiddleMan Server Listening on {1} on port {2}", Format(TimeOfDay, "hh:mm:ss"), lstHostMiddleMan, MMPort)
            Catch e As Exception
                Console.WriteLine()
                Console.ForegroundColor = System.ConsoleColor.Red
                Console.WriteLine("[{0}] Error in {2}: {1}.", Format(TimeOfDay, "hh:mm:ss"), e.Message, e.Source)
                Console.ForegroundColor = System.ConsoleColor.Gray
            End Try
        End Sub


        Public Sub AcceptConnectionMiddleMan()
            Do While Not _flagStopListen
                Thread.Sleep(100)
                If ConnectionMiddleMan.Pending() Then
                    Dim MiddleManClient As New MiddleManServerClass
                    MiddleManClient.Socket = ConnectionMiddleMan.AcceptSocket

                    Dim NewThread As New Thread(AddressOf MiddleManClient.ProcessMiddleMan)
                    NewThread.Start()
                End If
            Loop
        End Sub


        Public Sub ProcessMiddleMan()
            IP = CType(Socket.RemoteEndPoint, IPEndPoint).Address
            MMPort = CType(Socket.RemoteEndPoint, IPEndPoint).Port

            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] MiddleMan Incoming connection from [{1}:{2}]", Format(TimeOfDay, "hh:mm:ss"), IP, MMPort)
            Console.ForegroundColor = System.ConsoleColor.Gray


            Dim packet As System.IO.MemoryStream = New System.IO.MemoryStream()
            Dim writer As System.IO.BinaryWriter = New System.IO.BinaryWriter(packet)
            writer.Write(System.Text.Encoding.ASCII.GetBytes("127.0.0.1:8086"))
            writer.Write(CByte(0))
            SendMiddleManClient(packet.ToArray())
            Socket.Close()

            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] MiddleMan Connection from [{1}:{2}] closed", Format(TimeOfDay, "hh:mm:ss"), IP, MMPort)
            Console.ForegroundColor = System.ConsoleColor.Gray

            Me.DisposeMiddleMan()
        End Sub

        Public Sub SendMiddleManClient(ByVal data() As Byte)
            Try
                Dim i As Integer = Socket.Send(data, 0, data.Length, SocketFlags.None)

            Catch Ex As Exception
                Console.ForegroundColor = System.ConsoleColor.Red
                Console.WriteLine("[{0}] MiddleMan Connection from [{1}:{2}] do not exist - ERROR!!!", Format(TimeOfDay, "hh:mm:ss"), IP, MMPort)
                Console.ForegroundColor = System.ConsoleColor.Gray
                Socket.Close()
            End Try
        End Sub


        Public Sub OnMiddleManData(ByVal data() As Byte)
            Console.WriteLine("OnMiddleManData")
            Select Case data(0)

                Case Else
                    Console.ForegroundColor = System.ConsoleColor.Red
                    Console.WriteLine("[{0}] [{1}:{2}] Unknown MiddleMan Opcode 0x{3}", Format(TimeOfDay, "hh:mm:ss"), IP, MMPort, data(0))
                    Console.ForegroundColor = System.ConsoleColor.Gray
            End Select
        End Sub


        Public Sub DisposeMiddleMan() Implements System.IDisposable.Dispose
            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] MiddleMan Connection from [{1}:{2}] deleted", Format(TimeOfDay, "hh:mm:ss"), IP, MMPort)
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Sub


    End Class

End Module