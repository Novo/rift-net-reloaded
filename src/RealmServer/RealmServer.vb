'RealmServer.vb
'
'Rift .NET Reloaded -- An OpenSource Server Emulator for World of Warcraft Classic Alpha 0.5.3 (3368) written in VB.Net
'Copyright (c) 2012 noVo aka. takeoYasha

'This program is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.

'You should have received a copy of the GNU General Public License
'along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
        Private ConnectionRealm As TcpListener
        Private lstHostRealm As Net.IPAddress = Net.IPAddress.Parse(Config.RSHost)


        Public Sub start()
            Console.WriteLine("[{0}] Realm Server Starting...", Format(TimeOfDay, "HH:mm:ss"))
            Try
                ConnectionRealm = New TcpListener(lstHostRealm, RSPort)
                ConnectionRealm.Start()
                Dim RSListenThread As Thread
                RSListenThread = New Thread(AddressOf AcceptConnectionRealm)
                RSListenThread.Name = "Realm Server, Listening"
                RSListenThread.Start()

                Console.WriteLine("[{0}] Realm Server Listening on {1} on port {2}", Format(TimeOfDay, "HH:mm:ss"), lstHostRealm, RSPort)
            Catch ex As Exception
                Console.WriteLine("[{0}] [{1}:{2}] Error in: {3}", Format(TimeOfDay, "HH:mm:ss"), RSIP, RSPort, Environment.NewLine & ex.ToString)
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
            Console.WriteLine("[{0}] Incoming Realm connection from [{1}:{2}]", Format(TimeOfDay, "HH:mm:ss"), RSIP, RSPort)
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

#If DEBUG Then
                    PacketLog.DumpPacket(buffer, ">>")
#End If

                Catch Err As Exception
                    Console.ForegroundColor = System.ConsoleColor.Red
                    Console.WriteLine("[{0}] Realm Connection from [{1}:{2}] do not exist - ERROR!!!", Format(TimeOfDay, "HH:mm:ss"), RSIP, RSPort)
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

                Console.WriteLine("[{0}:{1}] Realm Data sent {2} bytes", RSIP, RSPort, bytesSent)

                RealmSocket.Close()

                Console.ForegroundColor = System.ConsoleColor.DarkGray
                Console.WriteLine("[{0}] Realm Connection from [{1}:{2}] closed", Format(TimeOfDay, "HH:mm:ss"), RSIP, RSPort)
                Console.ForegroundColor = System.ConsoleColor.Gray

                Me.DisposeRealm()

            Catch socketException As SocketException
                Console.WriteLine("Realm Connection from [{0}:{1}] cause error {3}{4}", RSIP, RSPort, socketException.Message.ToString, vbNewLine)
            End Try

        End Sub


        Public Sub OnRealmData(ByVal data() As Byte)
            'not used in Alpha ...
            Console.ForegroundColor = System.ConsoleColor.Red
            Console.WriteLine("[{0}] [{1}:{2}] Unknown Realm Opcode 0x{3}", Format(TimeOfDay, "HH:mm:ss"), RSIP, RSPort, data(0))
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Sub


        Public Sub DisposeRealm() Implements System.IDisposable.Dispose
            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] Realm Connection from [{1}:{2}] deleted", Format(TimeOfDay, "HH:mm:ss"), RSIP, RSPort)
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Sub


    End Class

End Module