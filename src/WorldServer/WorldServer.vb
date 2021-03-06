﻿'Rift .NET Reloaded -- An OpenSource Server Emulator for World of Warcraft Classic Alpha 0.5.3 (3368) written in VB.Net
'Copyright (c) 2013 noVo aka. takeoYasha www.easy-emu.de

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
Imports System.Text


Public Module WS_Main
    Public WS As WorldServerClass
    Public Log As New BaseWriter
    Public PacketHandlers As New Dictionary(Of OpCodes, HandlePacket)
    Delegate Sub HandlePacket(ByRef Packet As PacketReader, ByRef Client As WorldServerClass)


    Class WorldServerClass
        Implements IDisposable

        Public _flagStopListen As Boolean = False
        Public Account As AccountObject = Nothing
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
            Catch ex As Exception
                Console.WriteLine("[{0}] [{1}:{2}] Error in: {3}", Format(TimeOfDay, "HH:mm:ss"), WSIP, WSPort, Environment.NewLine & ex.ToString)
            End Try
        End Sub


        Public Sub AcceptConnectionWorld()
            Do While Not _flagStopListen
                Thread.Sleep(100)
                If ConnectionWorld.Pending() Then

                    Dim WorldClient As New WorldServerClass 'Create a new World for every new Connection
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

            If Not IsNothing(Me.Character) Then
                Globals.WorldMgr.DeleteSession(Me.Character.GUID)
            End If

            Console.ForegroundColor = System.ConsoleColor.DarkGray
            Console.WriteLine("[{0}] World Connection from [{1}:{2}] closed", Format(TimeOfDay, "HH:mm:ss"), WSIP, WSPort)
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Sub


        Public Sub SendWorldClient(ByVal packet As PacketWriter)
            If packet Is Nothing Then Throw New ApplicationException("World Packet doesn't contain data!")

            SyncLock Me
                Try
                    Dim buffer As Byte() = packet.ReadDataToSend()

                    'Send Data async
                    WorldServerSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, New AsyncCallback(AddressOf AsyncSendWorldClientCallback), WorldServerSocket)

                    Console.WriteLine("[{0}] [{1}:{2}] >> OpCode {3}({4}), Length:{5}", Format(TimeOfDay, "HH:mm:ss"), WSIP, WSPort, packet.Opcode.ToString, Val(packet.Opcode).ToString, packet.Size.ToString)

#If DEBUG Then
                    PacketLog.DumpPacket(buffer, ">>")
#End If
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

                    If Me.Character IsNot Nothing Then
                        Dim charGuid As ULong = Me.Character.GUID
                        If Globals.WorldMgr.Sessions.ContainsKey(charGuid) Then
                            Globals.WorldMgr.Sessions(charGuid) = Me
                        Else
                            Globals.WorldMgr.AddSession(charGuid, Me)
                        End If
                    End If

                    If PacketHandlers.ContainsKey(PacketBuffer.Opcode) = True Then
                        PacketHandlers(PacketBuffer.Opcode).Invoke(PacketBuffer, Me)
                    End If

                Else
                    Console.WriteLine("[{0}] [{1}:{2}] << Unknown OpCode: {3}, Length:{4}", Format(TimeOfDay, "HH:mm:ss"), WSIP, WSPort, PacketBuffer.Opcode, PacketBuffer.Size)
                End If

#If DEBUG Then
                PacketLog.DumpPacket(data, "<<")
#End If
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