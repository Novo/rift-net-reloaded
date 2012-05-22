'WS.Handlers.vb
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

Public Module WS_Handlers

    Public Sub IntializeWorldPacketHandlers()
        Console.Write("[{0}] Initialize Packet OpCode definitions", Format(TimeOfDay, "HH:mm:ss"))
        'PacketHandlers(OpCodes.CMSG_DONT_KNOW) = CType(AddressOf OnUnhandledPacket, HandlePacket)

        ' /* Ping/Pong */
        PacketHandlers(OpCodes.CMSG_PING) = CType(AddressOf On_CMSG_PING, HandlePacket)

        ' /* Start a new authenticated session */
        PacketHandlers(OpCodes.CMSG_AUTH_SESSION) = CType(AddressOf On_CMSG_AUTH_SESSION, HandlePacket)

        ' /* Character handling */
        PacketHandlers(OpCodes.CMSG_CHAR_ENUM) = CType(AddressOf On_CMSG_CHAR_ENUM, HandlePacket)
        PacketHandlers(OpCodes.CMSG_CHAR_CREATE) = CType(AddressOf On_CMSG_CHAR_CREATE, HandlePacket)
        PacketHandlers(OpCodes.CMSG_CHAR_DELETE) = CType(AddressOf On_CMSG_CHAR_DELETE, HandlePacket)

        ' /* Character enters world */
        PacketHandlers(OpCodes.CMSG_PLAYER_LOGIN) = CType(AddressOf On_CMSG_PLAYER_LOGIN, HandlePacket)

        ' /* What is the name of GUID? */
        PacketHandlers(OpCodes.CMSG_NAME_QUERY) = CType(AddressOf On_CMSG_NAME_QUERY, HandlePacket)

        ' /* Cancel any running trades */
        PacketHandlers(OpCodes.CMSG_CANCEL_TRADE) = CType(AddressOf On_CMSG_CANCEL_TRADE, HandlePacket)

        ' /* Clients wants to / doesnt want to log out */
        PacketHandlers(OpCodes.CMSG_LOGOUT_REQUEST) = CType(AddressOf On_CMSG_LOGOUT_REQUEST, HandlePacket)
        'PacketHandlers(OpCodes.CMSG_LOGOUT_CANCEL) = CType(AddressOf On_CMSG_LOGOUT_CANCEL, HandlePacket)

        ' /* Movement start + stop - redirected to other clients */
        PacketHandlers(OpCodes.MSG_MOVE_START_FORWARD) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_START_BACKWARD) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_STOP) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_START_STRAFE_LEFT) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_START_STRAFE_RIGHT) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_STOP_STRAFE) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_JUMP) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_START_TURN_LEFT) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_START_TURN_RIGHT) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_STOP_TURN) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_START_PITCH_UP) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_START_PITCH_DOWN) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_STOP_PITCH) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_SET_FACING) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_SET_PITCH) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        'MSG_MOVE_COLLIDE_REDIRECT
        'MSG_MOVE_COLLIDE_STUCK
        PacketHandlers(OpCodes.MSG_MOVE_SET_RUN_MODE) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_SET_WALK_MODE) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_START_SWIM) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_STOP_SWIM) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_ROOT) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_UNROOT) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_WORLDPORT_ACK) = CType(AddressOf On_MSG_MOVE_WORLDPORT_ACK, HandlePacket)

        ' /* Movement - update internal coordinates */
        PacketHandlers(OpCodes.MSG_MOVE_HEARTBEAT) = CType(AddressOf On_HandleMovementStatus, HandlePacket)

        ' /* Changing stand state */
        ' CMSG_STANDSTATECHANGE

        ' /* Chat / emotes */
        PacketHandlers(OpCodes.CMSG_MESSAGECHAT) = CType(AddressOf On_CMSG_MESSAGECHAT, HandlePacket)


        PacketHandlers(OpCodes.CMSG_QUERY_TIME) = CType(AddressOf On_CMSG_QUERY_TIME, HandlePacket)
        PacketHandlers(OpCodes.CMSG_WORLD_TELEPORT) = CType(AddressOf On_CMSG_WORLD_TELEPORT, HandlePacket)
        PacketHandlers(OpCodes.CMSG_REPORT_SCREENSHOT) = CType(AddressOf OnUnhandledPacket, HandlePacket)

        ' /* Unhandled / empty OpCodes */
        PacketHandlers(OpCodes.CMSG_ZONEUPDATE) = CType(AddressOf OnUnhandledPacket, HandlePacket) 'ReadUInt32("Zone Id");

        Console.WriteLine("... [done]")
    End Sub


    Public Sub OnUnhandledPacket(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Console.WriteLine("[{0}] [{1}:{2}] << Unhandled OpCode: {3}({4}), Length:{5}", Format(TimeOfDay, "HH:mm:ss"), Client.WSIP, Client.WSPort, packet.Opcode, Val(packet.Opcode).ToString, packet.Size)
    End Sub


End Module