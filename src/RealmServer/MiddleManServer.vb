'MiddleManServer.vb
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

Public Module MS_Main
    Public MS As MiddleManServerClass

    Class MiddleManServerClass
        Implements IDisposable

        Public _flagStopListen As Boolean = False
        Private MiddleManSocket As Socket = Nothing
        Private MMIP As Net.IPAddress = Net.IPAddress.Parse("0.0.0.0")
        Private MMPort As Int32 = Config.MMPort
        Private ConnectionMiddleMan As TcpListener
        Private lstHostMiddleMan As Net.IPAddress = Net.IPAddress.Parse(Config.MMHost)


        Public Sub Start()
            Console.WriteLine("[{0}] MiddleMan Server Starting...", Format(TimeOfDay, "HH:mm:ss"))
            Try
                ConnectionMiddleMan = New TcpListener(lstHostMiddleMan, MMPort)
                ConnectionMiddleMan.Start()

                Dim MSListenThread As Thread
                MSListenThread = New Thread(AddressOf AcceptConnectionMiddleMan)
                MSListenThread.Name = "MiddleMan Server, Listening"
                MSListenThread.Start()

                Console.WriteLine("[{0}] MiddleMan Server Listening on {1} on port {2}", Format(TimeOfDay, "HH:mm:ss"), lstHostMiddleMan, MMPort)
            Catch ex As Exception
                Console.WriteLine("[{0}] [{1}:{2}] Error in: {3}", Format(TimeOfDay, "HH:mm:ss"), MMIP, MMPort, Environment.NewLine & ex.ToString)
            End Try
        End Sub


        Public Sub AcceptConnectionMiddleMan()
            Do While Not _flagStopListen
                Thread.Sleep(100)
                If ConnectionMiddleMan.Pending() Then
                    Dim MiddleManClient As New MiddleManServerClass
                    MiddleManClient.MiddleManSocket = ConnectionMiddleMan.AcceptSocket

                    Dim NewThread As New Thread(AddressOf MiddleManClient.ProcessMiddleMan)
                    NewThread.Start()
                End If
            Loop
        End Sub


        Public Sub ProcessMiddleMan()
            MMIP = CType(MiddleManSocket.RemoteEndPoint, IPEndPoint).Address
            MMPort = CType(MiddleManSocket.RemoteEndPoint, IPEndPoint).Port

            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] MiddleMan Incoming connection from [{1}:{2}]", Format(TimeOfDay, "HH:mm:ss"), MMIP, MMPort)
            Console.ForegroundColor = System.ConsoleColor.Gray

            Dim response As New PacketWriter()
            response.WriteBytes(System.Text.Encoding.ASCII.GetBytes("127.0.0.1:8086"))
            response.WriteUInt8(0)

            SendMiddleManClient(response)
        End Sub

        Public Sub SendMiddleManClient(ByVal packet As PacketWriter)

            If packet Is Nothing Then Throw New ApplicationException("MiddleMan Packet doesn't contain data!")
            SyncLock Me
                Try
                    Dim buffer As Byte() = packet.ReadDataToSend(True)

                    'Send Data sync:
                    Dim bytesSent As Integer = 0
                    'bytesSent = Socket.Send(buffer, 0, buffer.Length, SocketFlags.None)
                    'Console.WriteLine("[{0}:{1}] Realm Data sent {2} bytes, opcode={3}", IP, Port, i, packet.Opcode)

                    'Send Data async
                    MiddleManSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, New AsyncCallback(AddressOf AsyncSendMiddleManClientCallback), MiddleManSocket)

#If DEBUG Then
                    PacketLog.DumpPacket(buffer, ">>")
#End If

                Catch ex As Exception
                    Console.WriteLine("[{0}] MiddleMan Connection from [{1}:{2}] do not exist - ERROR!{3}", Format(TimeOfDay, "HH:mm:ss"), MMIP, MMPort, Environment.NewLine & ex.ToString)
                    MiddleManSocket.Close()
                End Try
            End SyncLock

        End Sub


        Public Sub AsyncSendMiddleManClientCallback(ByVal result As IAsyncResult)

            Try
                Dim MiddleManSocket As Socket = TryCast(result.AsyncState, Socket)
                Dim bytesSent As Integer = 0
                bytesSent = MiddleManSocket.EndSend(result)

                'Console.WriteLine("[{0}:{1}] MiddleMan Data sent {2} bytes, opcode={3}", MMIP, MMPort, bytesSent, packet.Opcode)
                Console.WriteLine("[{0}:{1}] MiddleMan Data sent {2} bytes", MMIP, MMPort, bytesSent)

                MiddleManSocket.Close()

                Console.ForegroundColor = System.ConsoleColor.DarkGray
                Console.WriteLine("[{0}] MiddleMan Connection from [{1}:{2}] closed", Format(TimeOfDay, "HH:mm:ss"), MMIP, MMPort)
                Console.ForegroundColor = System.ConsoleColor.Gray

                Me.DisposeMiddleMan()

            Catch socketException As SocketException
                Console.WriteLine("MiddleMan Connection from [{0}:{1}] cause error {3}{4}", MMIP, MMPort, socketException.Message.ToString, vbNewLine)
            End Try

        End Sub


        Public Sub OnMiddleManData(ByVal data() As Byte)
            'not used, just a redirection ...
            Select Case data(0)

                Case Else
                    Console.ForegroundColor = System.ConsoleColor.Red
                    Console.WriteLine("[{0}] [{1}:{2}] Unknown MiddleMan Opcode 0x{3}", Format(TimeOfDay, "HH:mm:ss"), MMIP, MMPort, data(0))
                    Console.ForegroundColor = System.ConsoleColor.Gray
            End Select
        End Sub


        Public Sub DisposeMiddleMan() Implements System.IDisposable.Dispose
            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] MiddleMan Connection from [{1}:{2}] deleted", Format(TimeOfDay, "HH:mm:ss"), MMIP, MMPort)
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Sub


    End Class

End Module