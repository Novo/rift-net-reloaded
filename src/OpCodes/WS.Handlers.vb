Public Module WS_Handlers

    Public Sub IntializeWorldPacketHandlers()
        'PacketHandlers(OpCodes.CMSG_DONT_KNOW) = CType(AddressOf OnUnhandledPacket, HandlePacket)

        PacketHandlers(OpCodes.CMSG_PING) = CType(AddressOf On_CMSG_PING, HandlePacket)
        PacketHandlers(OpCodes.CMSG_AUTH_SESSION) = CType(AddressOf On_CMSG_AUTH_SESSION, HandlePacket)
        PacketHandlers(OpCodes.CMSG_CHAR_ENUM) = CType(AddressOf On_CMSG_CHAR_ENUM, HandlePacket)
        PacketHandlers(OpCodes.CMSG_CHAR_CREATE) = CType(AddressOf On_CMSG_CHAR_CREATE, HandlePacket)
        PacketHandlers(OpCodes.CMSG_CHAR_DELETE) = CType(AddressOf On_CMSG_CHAR_DELETE, HandlePacket)
        PacketHandlers(OpCodes.CMSG_PLAYER_LOGIN) = CType(AddressOf On_CMSG_PLAYER_LOGIN, HandlePacket)
        PacketHandlers(OpCodes.CMSG_NAME_QUERY) = CType(AddressOf On_CMSG_NAME_QUERY, HandlePacket)
        PacketHandlers(OpCodes.CMSG_LOGOUT_REQUEST) = CType(AddressOf On_CMSG_LOGOUT_REQUEST, HandlePacket)
        PacketHandlers(OpCodes.CMSG_MESSAGECHAT) = CType(AddressOf On_CMSG_MESSAGECHAT, HandlePacket)
        PacketHandlers(OpCodes.CMSG_QUERY_TIME) = CType(AddressOf On_CMSG_QUERY_TIME, HandlePacket)
        PacketHandlers(OpCodes.CMSG_CANCEL_TRADE) = CType(AddressOf On_CMSG_CANCEL_TRADE, HandlePacket)

        PacketHandlers(OpCodes.CMSG_WORLD_TELEPORT) = CType(AddressOf On_CMSG_WORLD_TELEPORT, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_WORLDPORT_ACK) = CType(AddressOf On_MSG_MOVE_WORLDPORT_ACK, HandlePacket)

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
        PacketHandlers(OpCodes.MSG_MOVE_SET_RUN_MODE) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_SET_WALK_MODE) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_START_SWIM) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_STOP_SWIM) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_SET_FACING) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_SET_PITCH) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_ROOT) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_UNROOT) = CType(AddressOf On_HandleMovementStatus, HandlePacket)
        PacketHandlers(OpCodes.MSG_MOVE_HEARTBEAT) = CType(AddressOf On_HandleMovementStatus, HandlePacket)

        PacketHandlers(OpCodes.CMSG_REPORT_SCREENSHOT) = CType(AddressOf OnUnhandledPacket, HandlePacket)
        PacketHandlers(OpCodes.CMSG_ZONEUPDATE) = CType(AddressOf OnUnhandledPacket, HandlePacket) 'ReadUInt32("Zone Id");
    End Sub


    Public Sub OnUnhandledPacket(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Console.WriteLine("[{0}] [{1}:{2}] << Unhandled OpCode: {3}({4}), Length:{5}", Format(TimeOfDay, "HH:mm:ss"), Client.WSIP, Client.WSPort, packet.Opcode, Val(packet.Opcode).ToString, packet.Size)
    End Sub


End Module